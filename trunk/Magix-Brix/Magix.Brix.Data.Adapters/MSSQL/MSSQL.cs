/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;
using Magix.Brix.Types;
using Magix.Brix.Data.Internal;
using Magix.Brix.Loader;

namespace Magix.Brix.Data.Adapters.MSSQL
{
    /**
     * Microsoft SQL Server Database Adapter, which probably works with 2005 and later, for
     * Magix-Brix. Contains all MS SQL specific logic needed to use Magix-Brix 
     * together with MS SQL.
     */
    public class MSSQL : StdSQLDataAdapter, IPersistViewState
    {
        private SqlConnection _connection;
        private static bool _hasInitialised;
        private Dictionary<string, Type> _cacheOfTypes;
        private static Dictionary<string, ConstructorInfo> _ctors = new Dictionary<string, ConstructorInfo>();
        private static Dictionary<string, MethodInfo> _setIdMethods = new Dictionary<string, MethodInfo>();
        private static Dictionary<string, List<Tuple<PropertyInfo, Tuple<ActiveFieldAttribute, MethodInfo>>>> _props = new Dictionary<string, List<Tuple<PropertyInfo, Tuple<ActiveFieldAttribute, MethodInfo>>>>();
        private static Dictionary<string, MethodInfo> _setMethods = new Dictionary<string, MethodInfo>();

        public override void Open(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            if (!_hasInitialised)
                InitializeSchema();
        }

        private void InitializeSchema()
        {
            using (Stream stream = Assembly.GetAssembly(GetType())
                .GetManifestResourceStream("Magix.Brix.Data.Adapters.MSSQL.Schema.sql"))
            {
                if (stream != null)
                {
                    TextReader reader = new StreamReader(stream);
                    string sql = reader.ReadToEnd();
                    SqlCommand cmd = new SqlCommand(sql, _connection);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    throw new ApplicationException("Couldn't find DDL resource in assembly. Something is *seriously* wrong ...!");
                }
            }
            _hasInitialised = true;
        }

        public override int CountWhere(Type type, params Criteria[] args)
        {
            string where = CreateCriteriasForDocument(type, null, args);
            SqlCommand cmd = new SqlCommand("select count(*) from " + TablePrefix + "Documents as d" + where, _connection);
            return (int)cmd.ExecuteScalar();
        }

        private bool ObjectExists(int id)
        {
            SqlCommand cmdExists = new SqlCommand(
                string.Format("select count(*) from " + TablePrefix + "Documents where ID={0}", id),
                _connection);
            return (int)cmdExists.ExecuteScalar() != 0;
        }

        private object CreateObject(Type type)
        {
            if (!_ctors.ContainsKey(type.FullName))
            {
                ConstructorInfo ctor = type.GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    new Type[] { },
                    null);

                // Checking to see if our class has a public default CTOR
                if (ctor == null)
                {
                    throw new ApplicationException(
                        "Cannot have a Brix Document which doesn't have a default constructor, type name is '" +
                        type.FullName +
                        "'. Modify class to have at least one public constructor, taking no arguments ...!");
                }
                _ctors[type.FullName] = ctor;
            }

            // Invoke the public default CTOR
            return _ctors[type.FullName].Invoke(null);
        }

        private void SetObjectID(object retVal, Type type, int id)
        {
            if (!_setIdMethods.ContainsKey(type.FullName))
            {
                _setIdMethods[type.FullName] = type.GetProperty(
                    "ID",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .GetSetMethod(true);

            }
            _setIdMethods[type.FullName].Invoke(retVal, new object[] { id });
        }

        protected override object SelectObjectByID(Type type, int id)
        {
            // Must "de-reference" PluginLoader first, to make sure we've loaded our types ...
            object x = PluginLoader.Instance;

            // Checking to see if object exists...
            if (!ObjectExists(id))
                return null;

            // Creating object...
            object retVal = CreateObject(type);

            // Settings ID of object...
            SetObjectID(retVal, type, id);

            // Populating native fields of object...
            PopulateFields(type, id, retVal);

            // Then the 'shared objects' ...
            PopulateComposition(type, id, retVal);

            // Now returning our fetched object ...
            return retVal;
        }

        private void PopulateComposition(Type type, int id, object retVal)
        {
            if (!_props.ContainsKey(type.FullName))
            {
                CacheCompositionMethods(type);
            }
            foreach (Tuple<PropertyInfo, Tuple<ActiveFieldAttribute, MethodInfo>> idxProp in _props[type.FullName])
            {
                // Serializable property
                string where = " FK_Document=" + id + " and Name='" + Helpers.PropertyName(idxProp.Left) + "'";
                switch (idxProp.Left.PropertyType.FullName)
                {
                    case "System.Boolean":
                    case "System.DateTime":
                    case "System.Decimal":
                    case "System.Int32":
                    case "System.String":
                    case "System.Byte[]":
                        break;
                    default:
                        if (idxProp.Left.PropertyType.FullName.IndexOf("Magix.Brix.Types.LazyList") == 0)
                        {
                            // LazyList...
                            Type typeOfList = idxProp.Left.PropertyType;
                            Type typeOfListGenericArgument = idxProp.Left.PropertyType.GetGenericArguments()[0];
                            LazyHelper helper = new LazyHelper(typeOfListGenericArgument, id, idxProp.Right.Left.IsOwner, idxProp.Left.Name);
                            FunctorGetItems del = helper.GetItems;
                            if (!_ctors.ContainsKey(idxProp.Left.PropertyType.FullName))
                            {
                                _ctors[idxProp.Left.PropertyType.FullName] = typeOfList.GetConstructors()[0];
                            }
                            object tmp = _ctors[idxProp.Left.PropertyType.FullName].Invoke(new object[] { del });
                            idxProp.Right.Right.Invoke(retVal, new[] { tmp });
                        }
                        else if (idxProp.Left.PropertyType.FullName.IndexOf("System.Collections.Generic.List") == 0)
                        {
                            // NOT LazyList, but still List...!
                            Type typeOfListGenericArgument = idxProp.Left.PropertyType.GetGenericArguments()[0];
                            List<object> tmpValues = idxProp.Right.Left.IsOwner ?
                                new List<object>(
                                    Instance.Select(
                                        typeOfListGenericArgument, 
                                        idxProp.Left.Name, 
                                        Criteria.ParentId(id))) :
                                new List<object>(
                                    Instance.Select(
                                        typeOfListGenericArgument, 
                                        idxProp.Left.Name, 
                                        Criteria.ExistsIn(id)));
                            MethodInfo addMethod = idxProp.Left.PropertyType.GetMethod("Add");
                            object listContent = idxProp.Left.GetGetMethod(true)
                                .Invoke(retVal, new object[] { });
                            foreach (object idxTmpValue in tmpValues)
                            {
                                addMethod.Invoke(listContent, new[] { idxTmpValue });
                            }
                        }
                        else
                        {
                            if (idxProp.Right.Left.BelongsTo)
                            {
                                object tmp = SelectFirst(
                                    idxProp.Left.PropertyType,
                                    null,
                                    Criteria.HasChild(id));
                                idxProp.Right.Right.Invoke(retVal, new[] { tmp });
                            }
                            else if (idxProp.Right.Left.IsOwner)
                            {
                                object tmp = SelectFirst(
                                    idxProp.Left.PropertyType,
                                    idxProp.Left.Name,
                                    Criteria.ParentId(id));
                                idxProp.Right.Right.Invoke(retVal, new[] { tmp });
                            }
                            else
                            {
                                string propertyName = idxProp.Left.Name;
                                if (!string.IsNullOrEmpty(idxProp.Right.Left.RelationName))
                                    propertyName = idxProp.Right.Left.RelationName;
                                object tmp = SelectFirst(
                                    idxProp.Left.PropertyType, 
                                    propertyName, 
                                    Criteria.ExistsIn(id));
                                idxProp.Right.Right.Invoke(retVal, new[] { tmp });
                            }
                        }
                        continue; // Possibly composition
                }
            }
        }

        private static void CacheCompositionMethods(Type type)
        {
            List<PropertyInfo> props = new List<PropertyInfo>(
                    type.GetProperties(
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic));
            _props[type.FullName] = new List<Tuple<PropertyInfo, Tuple<ActiveFieldAttribute, MethodInfo>>>();
            foreach (PropertyInfo idxProp in props)
            {
                ActiveFieldAttribute[] attrs =
                    idxProp.GetCustomAttributes(typeof(ActiveFieldAttribute), true) as ActiveFieldAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    _props[type.FullName].Add(
                        new Tuple<PropertyInfo, Tuple<ActiveFieldAttribute, MethodInfo>>(
                            idxProp,
                            new Tuple<ActiveFieldAttribute, MethodInfo>(
                                attrs[0],
                                idxProp.GetSetMethod())));
                }
            }
        }

        private void PopulateFields(Type type, int id, object retVal)
        {
            // Looping through and getting all properties, which are NOT lists types ...
            foreach (string idxTableName in new string[] { 
                "BLOBS", 
                "Bools", 
                "Dates", 
                "Decimals", 
                "Ints", 
                "LongStrings", 
                "Strings" })
            {
                string fullTableName = this.TablePrefix + "Property" + idxTableName;
                string selectStatement = string.Format(@"
select Name, Value from {0} where FK_Document={1}",
                    fullTableName,
                    id);
                SqlCommand cmd = new SqlCommand(selectStatement, _connection);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader != null && reader.Read())
                    {
                        string name = reader[0] as string;
                        name = name.Substring(4);
                        object value = reader[1];

                        if (!_setMethods.ContainsKey(type.FullName + "." + name))
                        {
                            // Finding the property in our class
                            PropertyInfo propInfo = type.GetProperty(
                                name,
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                            if (propInfo == null)
                                continue;

                            // Checking to see if our property has the attribute it needs ...
                            if (propInfo.GetCustomAttributes(typeof(ActiveFieldAttribute), true) != null)
                            {
                                // Settings the value of the property
                                MethodInfo setMethod = propInfo.GetSetMethod(true);
                                _setMethods[type.FullName + "." + name] = setMethod;
                            }
                            else
                                continue;
                        }
                        _setMethods[type.FullName + "." + name].Invoke(retVal, new object[] { value });
                    }
                }
            }
        }
          
        public override object SelectFirst(Type type, string propertyName, params Criteria[] args)
        {
            string where = CreateSelectStatementForDocument(type, propertyName, args);
            SqlCommand cmd = new SqlCommand(where, _connection);
            int retValID;
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader == null || !reader.Read())
                    return null;
                retValID = reader.GetInt32(0);
            }
            return SelectByID(type, retValID);
        }

        public override IEnumerable<object> Select(Type type, string propertyName, params Criteria[] args)
        {
            string where = CreateSelectStatementForDocument(type, propertyName, args);
            SqlCommand cmd = new SqlCommand(where, _connection);
            List<int> retValIDs = new List<int>();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader != null && reader.Read())
                {
                    retValIDs.Add(reader.GetInt32(0));
                }
            }
            foreach(int idxRetValID in retValIDs)
            {
                yield return SelectByID(type, idxRetValID);
            }
        }

        public override IEnumerable<object> Select()
        {
            SqlCommand cmd = new SqlCommand("select ID, TypeName from " + TablePrefix + "Documents as d", _connection);
            List<Tuple<int, Type>> retValIDs = new List<Tuple<int, Type>>();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        if (_cacheOfTypes == null)
                        {
                            _cacheOfTypes = new Dictionary<string, Type>();
                            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                            {
                                foreach (Type idxType in assembly.GetTypes())
                                {
                                    _cacheOfTypes[idxType.FullName] = idxType;
                                }
                            }
                        }
                        string typeFullName = reader.GetString(1).Substring(3);
                        if (_cacheOfTypes.ContainsKey(typeFullName))
                        {
                            Type type = _cacheOfTypes[typeFullName];
                            retValIDs.Add(new Tuple<int, Type>(reader.GetInt32(0), type));
                        }
                    }
                    
                }
            }
            foreach (Tuple<int, Type> idxRetValID in retValIDs)
            {
                yield return SelectByID(idxRetValID.Right, idxRetValID.Left);
            }
        }

        protected override void DeleteObject(int id)
        {
            using (SqlTransaction transaction = _connection.BeginTransaction())
            {
                DeleteWithTransaction(id, transaction);
                transaction.Commit();
            }
        }

        protected void DeleteObject(int id, SqlTransaction transaction)
        {
            DeleteWithTransaction(id, transaction);
        }

        private void DeleteWithTransaction(int id, SqlTransaction transaction)
        {
            DeleteChildren(id, transaction);
            DeleteComposition(id, transaction);
            SqlCommand cmd = new SqlCommand(
                string.Format("delete from " + TablePrefix + "Documents where ID=" + id),
                _connection,
                transaction);
            cmd.ExecuteNonQuery();
        }

        private void DeleteComposition(int id, SqlTransaction transaction)
        {
            SqlCommand cmdChild = new SqlCommand(
                string.Format("delete from " + TablePrefix + "Documents2Documents where Document1ID={0} or Document2ID={0}", id),
                _connection,
                transaction);
            cmdChild.ExecuteNonQuery();
        }

        private void DeleteChildren(int id, SqlTransaction transaction)
        {
            List<int> childDocuments = new List<int>();
            SqlCommand cmdChild = new SqlCommand(
                string.Format("select ID from " + TablePrefix + "Documents where Parent={0}", id),
                _connection,
                transaction);
            using (SqlDataReader reader = cmdChild.ExecuteReader())
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        childDocuments.Add(reader.GetInt32(0));
                    }
                    
                }
            }
            foreach (int idxChild in childDocuments)
            {
                DeleteWithTransaction(idxChild, transaction);
            }
        }

        public override void Save(object value)
        {
            TransactionalObject trs = value as TransactionalObject;
            if(trs.Transaction != null)
            {
                // Recursive save ...
                SaveWithTransaction(
                    value, 
                    trs.Transaction as SqlTransaction, 
                    trs.ParentDocument, 
                    trs.ParentPropertyName);
            }
            else
            {
                using (SqlTransaction transaction = _connection.BeginTransaction())
                {
                    SaveWithTransaction(value, transaction, -1, null);
                    transaction.Commit();
                }
            }
        }

        private int SaveWithTransaction(
            object value, 
            SqlTransaction transaction, 
            int parentId, 
            string parentPropertyName)
        {
            Type type = value.GetType();
            int id = (int)type.GetProperty(
                "ID",
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public)
                .GetGetMethod()
                .Invoke(value, null);
            bool isUpdate = id != 0;
            PropertyInfo[] props =
                    type.GetProperties(
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic);
            if (isUpdate)
            {
                if (parentId == -1)
                {
                    // We do NOT want to tamper with the parent parts of the object unless
                    // we are given a valid parentId explicitly since it might be saving of
                    // a child that belongs to another object in the first place...
                    SqlCommand cmd = new SqlCommand(
                        string.Format("update " + TablePrefix + "Documents set Modified=getdate() where ID={0}",
                            id), _connection, transaction);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand cmd = new SqlCommand(
                        string.Format("update " + TablePrefix + "Documents set Modified=getdate(), Parent={1}, ParentPropertyName='{2}' where ID={0}",
                            id,
                            parentId,
                            parentPropertyName), _connection, transaction);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                SqlCommand cmd = new SqlCommand(
                    string.Format(
                        "insert into " + TablePrefix + "Documents (TypeName, Created, Modified, Parent, ParentPropertyName) values ('doc{0}', getdate(), getdate(), {1}, {2});select @@Identity;", 
                        type.FullName,
                        parentId == -1 ? "NULL" : parentId.ToString(),
                        parentPropertyName == null ? "null" : "'" + parentPropertyName + "'", _connection, transaction), 
                    _connection, 
                    transaction);
                id = (int)((decimal)cmd.ExecuteScalar());
                type.GetProperty(
                    "ID",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic |
                    BindingFlags.Public)
                    .GetSetMethod(true)
                    .Invoke(value, new object[] { id });
            }
            if (isUpdate)
            {
                // We need to iterate through everything and delete "old values" before saving new values...
                foreach (string idxTableName in new[] { "PropertyBLOBS", "PropertyBools", "PropertyDates", "PropertyDates", "PropertyDecimals", "PropertyInts", "PropertyLongStrings", "PropertyStrings" })
                {
                    string sql = string.Format("delete from {0} where FK_Document={1}", idxTableName, id);
                    SqlCommand cmd = new SqlCommand(sql, _connection, transaction);
                    cmd.ExecuteNonQuery();
                }
            }
            List<int> listOfIDsOfChildren = new List<int>();
            List<string> listOfParentPropertyNamesToNotDelete = new List<string>();
            foreach (PropertyInfo idxProp in props)
            {
                ActiveFieldAttribute[] attrs =
                    idxProp.GetCustomAttributes(typeof(ActiveFieldAttribute), true) as ActiveFieldAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    string tableName;
                    object valueOfProperty = idxProp.GetGetMethod(true).Invoke(value, null);
                    if (valueOfProperty == null)
                    {
                        if (!attrs[0].IsOwner)
                        {
                            string relationshipName = idxProp.Name;
                            if (!string.IsNullOrEmpty(attrs[0].RelationName))
                                relationshipName = attrs[0].RelationName;
                            string toDeleteRelationShip = string.Format("delete from Documents2Documents where (Document1ID = {0} or Document2ID = {0}) and PropertyName='{1}'",
                                id,
                                relationshipName);
                            SqlCommand cmdDeleteRelationShip = new SqlCommand(toDeleteRelationShip, _connection, transaction);
                            cmdDeleteRelationShip.ExecuteNonQuery();
                        }
                        continue;
                    }
                    switch (idxProp.PropertyType.FullName)
                    {
                        case "System.Boolean":
                            tableName = "PropertyBools";
                            break;
                        case "System.DateTime":
                            if (((DateTime)valueOfProperty) == DateTime.MinValue)
                                continue; // "NULL" value...
                            tableName = "PropertyDates";
                            break;
                        case "System.Decimal":
                            tableName = "PropertyDecimals";
                            break;
                        case "System.Int32":
                            tableName = "PropertyInts";
                            break;
                        case "System.String":
                            tableName = "PropertyStrings";
                            break;
                        case "System.Byte[]":
                            tableName = "PropertyBLOBS";
                            break;
                        default:
                            IEnumerable enumerable = valueOfProperty as IEnumerable;
                            if (enumerable == null)
                            {
                                if (attrs[0].BelongsTo)
                                {
                                    int currentParentId = (int)valueOfProperty.GetType().GetProperty(
                                        "ID",
                                        BindingFlags.Instance |
                                        BindingFlags.NonPublic |
                                        BindingFlags.Public)
                                        .GetGetMethod()
                                        .Invoke(valueOfProperty, null);

                                    SqlCommand cmdAddAsParent = new SqlCommand(
                                        string.Format(
                                        "update " +
                                        TablePrefix +
                                        "Documents set Modified=getdate(), Parent={1}, ParentPropertyName='{2}' where ID={0}",
                                            id,
                                            currentParentId,
                                            attrs[0].RelationName), _connection, transaction);
                                    cmdAddAsParent.ExecuteNonQuery();
                                }
                                else if (attrs[0].IsOwner)
                                {
                                    TransactionalObject trs = valueOfProperty as TransactionalObject;
                                    trs.Transaction = transaction;
                                    trs.ParentDocument = id;
                                    trs.ParentPropertyName = idxProp.Name;
                                    try
                                    {
                                        trs.Save();
                                        listOfIDsOfChildren.Add(trs.ID);
                                    }
                                    finally
                                    {
                                        trs.Reset();
                                    }
                                    int childId = trs.ID;
                                    listOfIDsOfChildren.Add(childId);
                                }
                                else
                                {
                                    string propertyRelationshipName = idxProp.Name;
                                    if (!string.IsNullOrEmpty(attrs[0].RelationName))
                                        propertyRelationshipName = attrs[0].RelationName;
                                    int childId = (int)valueOfProperty.GetType().GetProperty(
                                        "ID",
                                        BindingFlags.Instance |
                                        BindingFlags.NonPublic |
                                        BindingFlags.Public)
                                        .GetGetMethod()
                                        .Invoke(valueOfProperty, null);

                                    SqlCommand deleteFromD2D = new SqlCommand(
                                        string.Format("delete from " + TablePrefix + "Documents2Documents where Document1ID={0} and PropertyName='{1}'",
                                            id,
                                            propertyRelationshipName), 
                                        _connection, 
                                        transaction);
                                    deleteFromD2D.ExecuteNonQuery();

                                    SqlCommand cmdContains = new SqlCommand(
                                        string.Format(
                                            "insert into " + TablePrefix + "Documents2Documents (Document1ID, Document2ID, PropertyName) values ({0}, {1}, '{2}')",
                                            id,
                                            childId,
                                            propertyRelationshipName),
                                        _connection,
                                        transaction);
                                    cmdContains.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                PropertyInfo listRetrieved = 
                                    enumerable.GetType().GetProperty(
                                        "ListRetrieved", 
                                        BindingFlags.Instance | 
                                        BindingFlags.Public | 
                                        BindingFlags.NonPublic);
                                if (listRetrieved != null)
                                {
                                    // LazyList...
                                    if (!(bool)listRetrieved.GetGetMethod(true).Invoke(enumerable, null))
                                    {
                                        listOfParentPropertyNamesToNotDelete.Add(idxProp.Name);
                                    }
                                    else
                                    {
                                        if(attrs[0].BelongsTo)
                                        {
                                            foreach (object idxChild in enumerable)
                                            {
                                                int currentParentId = (int)idxChild.GetType().GetProperty(
                                                    "ID",
                                                    BindingFlags.Instance |
                                                    BindingFlags.NonPublic |
                                                    BindingFlags.Public)
                                                    .GetGetMethod()
                                                    .Invoke(idxChild, null);

                                                SqlCommand cmdAddAsParent = new SqlCommand(
                                                    string.Format(
                                                    "update " +
                                                    TablePrefix +
                                                    "Documents set Modified=getdate(), Parent={1}, ParentPropertyName='{2}' where ID={0}",
                                                        id,
                                                        currentParentId,
                                                        attrs[0].RelationName), _connection, transaction);
                                                cmdAddAsParent.ExecuteNonQuery();
                                            }
                                        }
                                        else if (attrs[0].IsOwner)
                                        {
                                            foreach (object idxChild in enumerable)
                                            {
                                                TransactionalObject trs = idxChild as TransactionalObject;
                                                trs.Transaction = transaction;
                                                trs.ParentDocument = id;
                                                trs.ParentPropertyName = idxProp.Name;
                                                try
                                                {
                                                    trs.Save();
                                                    listOfIDsOfChildren.Add(trs.ID);
                                                }
                                                finally
                                                {
                                                    trs.Reset();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // Delete old relationships whith this documentid and this property name
                                            SqlCommand sqlDeleteRelationRecords = new SqlCommand(
                                                string.Format("delete from " + TablePrefix + "Documents2Documents where Document1ID={0} and PropertyName='{1}'",
                                                    id,
                                                    idxProp.Name), _connection, transaction);
                                            sqlDeleteRelationRecords.ExecuteNonQuery();

                                            foreach (object idxChild in enumerable)
                                            {
                                                // TODO: Should we really save the related document here...?
                                                // Or should we only save the releationship...?
                                                // If we only save the relationship, we might get
                                                // "dangling pointers", and if we save everything
                                                // we overspend resources, plus that we do save something
                                                // which is only linked, which might feel unintuitive...?
                                                TransactionalObject trs = idxChild as TransactionalObject;
                                                trs.Transaction = transaction;
                                                trs.ParentDocument = -1;
                                                trs.ParentPropertyName = idxProp.Name;
                                                try
                                                {
                                                    trs.Save();
                                                    listOfIDsOfChildren.Add(trs.ID);
                                                }
                                                finally
                                                {
                                                    trs.Reset();
                                                }
                                                int documentId = trs.ID;

                                                SqlCommand cmdContains = new SqlCommand(
                                                    string.Format(
                                                        "insert into " + TablePrefix + "Documents2Documents (Document1ID, Document2ID, PropertyName) values ({0}, {1}, '{2}')",
                                                        id,
                                                        documentId,
                                                        idxProp.Name),
                                                    _connection,
                                                    transaction);
                                                cmdContains.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // NOT LazyList...
                                    if (attrs[0].IsOwner)
                                    {
                                        foreach (object idxChild in enumerable)
                                        {
                                            TransactionalObject trs = idxChild as TransactionalObject;
                                            trs.Transaction = transaction;
                                            trs.ParentDocument = id;
                                            trs.ParentPropertyName = idxProp.Name;
                                            try
                                            {
                                                trs.Save();
                                                listOfIDsOfChildren.Add(trs.ID);
                                            }
                                            finally
                                            {
                                                trs.Reset();
                                            }
                                            int childId = trs.ID;
                                            listOfIDsOfChildren.Add(childId);
                                        }
                                    }
                                    else
                                    {
                                        // Delete old relationships whith this documentid and this property name
                                        SqlCommand sqlDeleteRelationRecords = new SqlCommand(
                                            string.Format("delete from " + TablePrefix + "Documents2Documents where Document1ID={0} and PropertyName='{1}'",
                                                id,
                                                idxProp.Name), _connection, transaction);
                                        sqlDeleteRelationRecords.ExecuteNonQuery();

                                        foreach (object idxChild in enumerable)
                                        {
                                            // TODO: Should we really save the related document here...?
                                            // Or should we only save the releationship...?
                                            // If we only save the relationship, we might get
                                            // "dangling pointers", and if we save everything
                                            // we overspend resources, plus that we do save something
                                            // which is only linked, which might feel unintuitive...?
                                            TransactionalObject trs = idxChild as TransactionalObject;
                                            trs.Transaction = transaction;
                                            trs.ParentDocument = -1;
                                            trs.ParentPropertyName = idxProp.Name;
                                            try
                                            {
                                                trs.Save();
                                                listOfIDsOfChildren.Add(trs.ID);
                                            }
                                            finally
                                            {
                                                trs.Reset();
                                            }
                                            int documentId = trs.ID;

                                            SqlCommand cmdContains = new SqlCommand(
                                                string.Format(
                                                    "insert into " + TablePrefix + "Documents2Documents (Document1ID, Document2ID, PropertyName) values ({0}, {1}, '{2}')",
                                                    id,
                                                    documentId,
                                                    idxProp.Name),
                                                _connection,
                                                transaction);
                                            cmdContains.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            continue;
                    }
                    string sql = string.Format(
                         "insert into {0} (FK_Document, Value, Name) values({1}, @value, '{2}')",
                         tableName,
                         id,
                         Helpers.PropertyName(idxProp));
                    SqlCommand cmd = new SqlCommand(sql, _connection, transaction);
                    cmd.Parameters.Add(new SqlParameter("@value", valueOfProperty));
                    cmd.ExecuteNonQuery();
                }
            }
            string whereDelete = "";
            bool first = true;
            foreach (int idx in listOfIDsOfChildren)
            {
                if (!first)
                    whereDelete += ",";
                first = false;
                whereDelete += idx.ToString();
            }
            string andNotIn = string.IsNullOrEmpty(whereDelete) ? 
                "" : 
                string.Format(" and ID not in({0})", whereDelete);
            whereDelete = "";
            first = true;
            foreach (string idx in listOfParentPropertyNamesToNotDelete)
            {
                if (!first)
                    whereDelete += ",";
                first = false;
                whereDelete += "'" + idx + "'";
            }
            string andNotInParentPropertyNames = 
                string.IsNullOrEmpty(whereDelete) 
                    ? "" 
                    : string.Format(" and ParentPropertyName not in({0})", whereDelete);
            SqlCommand cmdDeleteAllInfants = new SqlCommand(
                string.Format("select ID from " + TablePrefix + "Documents where Parent={0}{1}{2}", id, andNotIn, andNotInParentPropertyNames),
                _connection,
                transaction);
            List<int> idsToDelete = new List<int>();
            using (SqlDataReader reader = cmdDeleteAllInfants.ExecuteReader())
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        int idxChildId = reader.GetInt32(0);
                        idsToDelete.Add(idxChildId);
                    }
                }
            }
            foreach (int idxItemToDelete in idsToDelete)
            {
                DeleteObject(idxItemToDelete, transaction);
            }
            return id;
        }

        public override void Close()
        {
            _connection.Close();
        }

        public void Save(string sessionId, string pageUrl, string content)
        {
            // Deleting *OLD* ViewState from table...
            string key = sessionId + "|" + pageUrl;
            SqlCommand sql = new SqlCommand("delete from " + TablePrefix + "ViewStateStorage where ID='" + key + "' or Created < @dateNow", _connection);
            sql.Parameters.Add(new SqlParameter("@dateNow", DateTime.Now.AddHours(-4)));
            sql.ExecuteNonQuery();

            // Saving new value
            sql = new SqlCommand(
                "insert into " + TablePrefix + "ViewStateStorage (ID, Content, Created) values (@id, @content, @created)",
                _connection);
            sql.Parameters.Add(new SqlParameter("@id", key));
            sql.Parameters.Add(new SqlParameter("@content", content));
            sql.Parameters.Add(new SqlParameter("@created", DateTime.Now));
            sql.ExecuteNonQuery();
        }

        public string Load(string sessionId, string pageUrl)
        {
            // Retrieving ViewState
            string key = sessionId + "|" + pageUrl;
            SqlCommand sql = new SqlCommand("select content from " + TablePrefix + "ViewStateStorage where ID=@id", _connection);
            sql.Parameters.Add(new SqlParameter("@id", key));
            string retVal = sql.ExecuteScalar() as string;
            return retVal;
        }
    }
}
