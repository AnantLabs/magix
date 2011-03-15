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
     * Microsoft SQL Server Database Adapter, which probably works with 2005 and any later, for
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
        private MSTransaction _transaction;

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
                    SqlCommand cmd = CreateSqlCommand(sql);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    throw new ApplicationException(
                        @"Couldn't find DDL resource in assembly. Something 
is *seriously* wrong with your Data Adapter ...!");
                }
            }
            _hasInitialised = true;
        }

        public override Transaction BeginTransaction()
        {
            if (_transaction != null)
            {
                throw new ApplicationException(
                    "Cannot open transaction, when there's already an existing transaction open");
            }
            _transaction = new MSTransaction(_connection);
            return _transaction;
        }

        private SqlTransaction GetTransaction()
        {
            if (_transaction != null)
                return _transaction.Trans as SqlTransaction;
            return null;
        }

        private SqlCommand CreateSqlCommand(string sql)
        {
            return new SqlCommand(sql, _connection, GetTransaction());
        }

        public override int CountWhere(Type type, params Criteria[] args)
        {
            string where = CreateCriteriasForDocument(type, null, args);
            SqlCommand cmd = CreateSqlCommand(
                "select count(*) from " + 
                TablePrefix + 
                "Documents as d" + where);
            return (int)cmd.ExecuteScalar();
        }

        private bool ObjectExists(int id, Type type)
        {
            SqlCommand cmdExists = CreateSqlCommand(
                string.Format(
                    "select count(*) from " + TablePrefix + "Documents where ID={0} " +
                    "and TypeName='{1}'", 
                    id, 
                    Helpers.TypeName(type)));
            return (int)cmdExists.ExecuteScalar() != 0;
        }

        private object CreateObject(Type type)
        {
            if (!_ctors.ContainsKey(type.FullName))
            {
                ConstructorInfo ctor = type.GetConstructor(
                    BindingFlags.NonPublic | 
                    BindingFlags.Public | 
                    BindingFlags.Instance,
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
                    BindingFlags.Instance | 
                    BindingFlags.Public | 
                    BindingFlags.NonPublic)
                    .GetSetMethod(true);

            }
            _setIdMethods[type.FullName].Invoke(retVal, new object[] { id });
        }

        protected override object SelectObjectByID(Type type, int id)
        {
            // Must "de-reference" PluginLoader first, to make sure we've loaded our types ...
            object x = PluginLoader.Instance;

            // Checking to see if object exists...
            if (!ObjectExists(id, type))
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
            foreach (Tuple<PropertyInfo, Tuple<ActiveFieldAttribute, MethodInfo>> idxProp 
                in _props[type.FullName])
            {
                // Serializable property
                string where = " FK_Document=" + id + " and Name='" + Helpers.PropertyName(idxProp.Left) + "'";
                switch (idxProp.Left.PropertyType.FullName)
                {
                    case "System.Boolean":
                    case "System.DateTime":
                    case "System.Decimal":
                    case "System.Int32":
                    case "System.Guid":
                    case "System.String":
                    case "System.Byte[]":
                        break;
                    default:
                        if (idxProp.Left.PropertyType.FullName.IndexOf("Magix.Brix.Types.LazyList") == 0)
                        {
                            PopulateLazyList(id, retVal, idxProp);
                        }
                        else if (idxProp.Left.PropertyType.FullName.IndexOf("System.Collections.Generic.List") == 0)
                        {
                            PopulateActiveList(id, retVal, idxProp);
                        }
                        else
                        {
                            PopulateSingleObject(id, retVal, idxProp);
                        }
                        continue; // Possibly composition
                }
            }
        }

        private void PopulateSingleObject(
            int id, 
            object retVal, 
            Tuple<PropertyInfo, Tuple<ActiveFieldAttribute, MethodInfo>> idxProp)
        {
            object tmp = null;
            if (idxProp.Right.Left.BelongsTo)
            {
                if (string.IsNullOrEmpty(idxProp.Right.Left.RelationName))
                {
                    tmp = SelectFirst(
                        idxProp.Left.PropertyType,
                        idxProp.Right.Left.RelationName,
                        Criteria.HasChild(id));
                }
                else
                {
                    tmp = SelectFirst(
                        idxProp.Left.PropertyType,
                        idxProp.Right.Left.RelationName,
                        Criteria.ExistsIn(id, true));
                }
            }
            else if (idxProp.Right.Left.IsOwner)
            {
                tmp = SelectFirst(
                    idxProp.Left.PropertyType,
                    idxProp.Left.Name,
                    Criteria.ParentId(id));
            }
            else
            {
                string propertyName = idxProp.Left.Name;
                tmp = SelectFirst(
                    idxProp.Left.PropertyType,
                    propertyName,
                    Criteria.ExistsIn(id));
            }
            idxProp.Right.Right.Invoke(retVal, new[] { tmp });
        }

        private static void PopulateActiveList(
            int id, 
            object retVal, 
            Tuple<PropertyInfo, Tuple<ActiveFieldAttribute, MethodInfo>> idxProp)
        {
            // NOT LazyList, but still List...!
            Type typeOfListGenericArgument = 
                idxProp.Left.PropertyType.GetGenericArguments()[0];

            List<object> tmpValues = null;
            if (idxProp.Right.Left.BelongsTo)
            {
                if (string.IsNullOrEmpty(idxProp.Right.Left.RelationName))
                {
                    throw new ApplicationException(
                        "There's a significant error in your ActiveType class hierarchy. List of children, defined" +
                        "as BelongsTo, without RelationName indicates several parents, which is logically impossible...");
                }
                else
                {
                    // Parent owns the relationship, but it's a ManyToX relationship...
                    tmpValues = new List<object>(
                        Instance.Select(
                        typeOfListGenericArgument,
                        idxProp.Right.Left.RelationName,
                        Criteria.ExistsIn(id, true)));
                }
            }
            else
            {
                if (idxProp.Right.Left.IsOwner)
                {
                    tmpValues = new List<object>(
                        Instance.Select(
                            typeOfListGenericArgument,
                            idxProp.Left.Name,
                            Criteria.ParentId(id)));
                }
                else
                {
                    tmpValues = new List<object>(
                        Instance.Select(
                            typeOfListGenericArgument,
                            idxProp.Left.Name,
                            Criteria.ExistsIn(id)));
                }
            }
            MethodInfo addMethod = idxProp.Left.PropertyType.GetMethod("Add");
            object listContent = idxProp.Left.GetGetMethod(true)
                .Invoke(retVal, new object[] { });
            foreach (object idxTmpValue in tmpValues)
            {
                addMethod.Invoke(listContent, new[] { idxTmpValue });
            }
        }

        private static void PopulateLazyList(
            int id, 
            object retVal, 
            Tuple<PropertyInfo, Tuple<ActiveFieldAttribute, MethodInfo>> idxProp)
        {
            // LazyList...
            Type typeOfList = idxProp.Left.PropertyType;
            Type typeOfListGenericArgument = idxProp.Left.PropertyType.GetGenericArguments()[0];
            LazyHelper helper = new LazyHelper(
                typeOfListGenericArgument, 
                id, 
                idxProp.Right.Left.IsOwner, 
                idxProp.Right.Left.BelongsTo, 
                idxProp.Left.Name,
                idxProp.Right.Left.RelationName);
            FunctorGetItems del = helper.GetItems;
            if (!_ctors.ContainsKey(idxProp.Left.PropertyType.FullName))
            {
                _ctors[idxProp.Left.PropertyType.FullName] = typeOfList.GetConstructors()[0];
            }
            object tmp = _ctors[idxProp.Left.PropertyType.FullName].Invoke(new object[] { del });
            idxProp.Right.Right.Invoke(retVal, new[] { tmp });
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
                                idxProp.GetSetMethod(true))));
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
                "Guids", 
                "LongStrings", 
                "Strings" })
            {
                string fullTableName = this.TablePrefix + "Property" + idxTableName;
                string selectStatement = string.Format(@"
select Name, Value from {0} where FK_Document={1}",
                    fullTableName,
                    id);
                SqlCommand cmd = CreateSqlCommand(selectStatement);
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
            SqlCommand cmd = CreateSqlCommand(where);
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
            SqlCommand cmd = CreateSqlCommand(where);
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
            SqlCommand cmd = CreateSqlCommand(
                "select ID, TypeName from " + TablePrefix + "Documents as d");
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
            DeleteWithTransaction(id);
        }

        private void DeleteWithTransaction(int id)
        {
            DeleteChildren(id);
            DeleteComposition(id);
            SqlCommand cmd = CreateSqlCommand(
                string.Format("delete from " + TablePrefix + "Documents where ID=" + id));
            cmd.ExecuteNonQuery();
        }

        private void DeleteComposition(int id)
        {
            SqlCommand cmdChild = CreateSqlCommand(
                string.Format(
                    "delete from " + 
                    TablePrefix + 
                    "Documents2Documents where Document1ID={0} or Document2ID={0}", 
                    id));
            cmdChild.ExecuteNonQuery();
        }

        private void DeleteChildren(int id)
        {
            List<int> childDocuments = new List<int>();
            SqlCommand cmdChild = CreateSqlCommand(
                string.Format("select ID from " + TablePrefix + "Documents where Parent={0}", id));
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
                DeleteWithTransaction(idxChild);
            }
        }

        public override void Save(object value)
        {
            TransactionalObject trs = value as TransactionalObject;
            if(trs != null)
            {
                // Recursive save ...
                SaveWithTransaction(
                    value, 
                    trs.ParentDocument, 
                    trs.ParentPropertyName);
            }
            else
            {
                SaveWithTransaction(value, -1, null);
            }
        }

        private int SaveWithTransaction(
            object value, 
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

            id = PrepareForSerialization(
                value, 
                parentId, 
                parentPropertyName, 
                type, 
                id, 
                isUpdate);

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
                        case "System.Guid":
                            tableName = "PropertyGuids";
                            break;
                        case "System.Byte[]":
                            tableName = "PropertyBLOBS";
                            break;
                        default:
                            if (valueOfProperty == null)
                            {
                                DeleteNewlyRemovedChildObjects(
                                    id, 
                                    idxProp, 
                                    attrs);
                            }
                            else
                            {
                                SerializeComplexChildren(
                                    id, 
                                    listOfIDsOfChildren, 
                                    listOfParentPropertyNamesToNotDelete, 
                                    idxProp, 
                                    attrs, 
                                    valueOfProperty);
                            }
                            continue;
                    }
                    if (valueOfProperty != null)
                    {
                        string sql = string.Format(
                             "insert into {0} (FK_Document, Value, Name) values({1}, @value, '{2}')",
                             tableName,
                             id,
                             Helpers.PropertyName(idxProp));
                        SqlCommand cmd = CreateSqlCommand(sql);
                        cmd.Parameters.Add(new SqlParameter("@value", valueOfProperty));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            CleanUpAfterSerializing(
                id, 
                listOfIDsOfChildren, 
                listOfParentPropertyNamesToNotDelete);
            return id;
        }

        private int PrepareForSerialization(
            object value, 
            int parentId, 
            string parentPropertyName, 
            Type type, 
            int id, 
            bool isUpdate)
        {
            if (isUpdate)
            {
                if (parentId == -1)
                {
                    // We do NOT want to tamper with the parent parts of the object unless
                    // we are given a valid parentId explicitly since it might be saving of
                    // a child that belongs to another object in the first place...
                    SqlCommand cmd = CreateSqlCommand(
                        string.Format(
                            "update " + TablePrefix + "Documents set Modified=getdate() where ID={0}",
                            id));
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand cmd = CreateSqlCommand(
                        string.Format(
                            "update " + 
                                TablePrefix + 
                                "Documents set Modified=getdate(), Parent={1}, ParentPropertyName='{2}' where ID={0}",
                            id,
                            parentId,
                            parentPropertyName));
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                SqlCommand cmd = CreateSqlCommand(
                    string.Format(
                        "insert into " + TablePrefix + "Documents (TypeName, Created, Modified, Parent, ParentPropertyName) values ('{0}', getdate(), getdate(), {1}, {2});select @@Identity;",
                        Helpers.TypeName(type),
                        parentId == -1 ? "NULL" : parentId.ToString(),
                        parentPropertyName == null ? "null" : "'" + parentPropertyName + "'"));
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
                foreach (string idxTableName in new[] { "PropertyBLOBS", "PropertyBools", "PropertyDates", "PropertyDates", "PropertyDecimals", "PropertyInts", "PropertyLongStrings", "PropertyStrings", "PropertyGuids" })
                {
                    string sql = string.Format("delete from {0} where FK_Document={1}", idxTableName, id);
                    SqlCommand cmd = CreateSqlCommand(sql);
                    cmd.ExecuteNonQuery();
                }
            }
            return id;
        }

        private void CleanUpAfterSerializing(
            int id, 
            List<int> listOfIDsOfChildren, 
            List<string> listOfParentPropertyNamesToNotDelete)
        {
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
            SqlCommand cmdDeleteAllInfants = CreateSqlCommand(
                string.Format(
                    "select ID from {3}Documents where Parent={0}{1}{2}", 
                    id, 
                    andNotIn, 
                    andNotInParentPropertyNames,
                    TablePrefix));
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
                DeleteObject(idxItemToDelete);
            }
        }

        private void SerializeComplexChildren(
            int id, 
            List<int> listOfIDsOfChildren, 
            List<string> listOfParentPropertyNamesToNotDelete, 
            PropertyInfo idxProp, 
            ActiveFieldAttribute[] attrs, 
            object valueOfProperty)
        {
            IEnumerable enumerable = valueOfProperty as IEnumerable;
            if (enumerable == null)
            {
                SerializeSingleComplexChild(
                    id, 
                    listOfIDsOfChildren, 
                    idxProp, 
                    attrs, 
                    valueOfProperty);
            }
            else
            {
                SerializeMultipleComplexChildren(
                    id, 
                    listOfIDsOfChildren, 
                    listOfParentPropertyNamesToNotDelete, 
                    idxProp, 
                    attrs, 
                    valueOfProperty, 
                    enumerable);
            }
        }

        private void SerializeMultipleComplexChildren(
            int id, 
            List<int> listOfIDsOfChildren, 
            List<string> listOfParentPropertyNamesToNotDelete, 
            PropertyInfo idxProp, 
            ActiveFieldAttribute[] attrs, 
            object valueOfProperty, 
            IEnumerable enumerable)
        {
            PropertyInfo listRetrieved =
                enumerable.GetType().GetProperty(
                    "ListRetrieved",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);
            if (listRetrieved != null)
            {
                SerializeLazyList(
                    id, 
                    listOfIDsOfChildren, 
                    listOfParentPropertyNamesToNotDelete, 
                    idxProp, 
                    attrs, 
                    valueOfProperty, 
                    enumerable, 
                    listRetrieved);
            }
            else
            {
                SerializeActiveList(
                    id, 
                    listOfIDsOfChildren, 
                    idxProp, 
                    attrs, 
                    enumerable);
            }
        }

        private void SerializeActiveList(
            int id, 
            List<int> listOfIDsOfChildren, 
            PropertyInfo idxProp, 
            ActiveFieldAttribute[] attrs, 
            IEnumerable enumerable)
        {
            // NOT LazyList...
            if (attrs[0].BelongsTo)
            {
                // We do nothing here ...!
                // The other side controls our relationship some how ...
            }
            else if (attrs[0].IsOwner)
            {
                foreach (object idxChild in enumerable)
                {
                    TransactionalObject trs = idxChild as TransactionalObject;
                    trs.ParentDocument = id;
                    trs.ParentPropertyName = idxProp.Name;
                    trs.Save();
                    listOfIDsOfChildren.Add(trs.ID);
                    int childId = trs.ID;
                    listOfIDsOfChildren.Add(childId);
                }
            }
            else
            {
                // Delete old relationships whith this documentid and this property name
                SqlCommand sqlDeleteRelationRecords = CreateSqlCommand(
                    string.Format("delete from " + TablePrefix + "Documents2Documents where Document1ID={0} and PropertyName='{1}'",
                        id,
                        idxProp.Name));
                sqlDeleteRelationRecords.ExecuteNonQuery();

                foreach (object idxChild in enumerable)
                {
                    TransactionalObject trs = idxChild as TransactionalObject;
                    int documentId = trs.ID;

                    SqlCommand cmdContains = CreateSqlCommand(
                        string.Format(
                            "insert into " + TablePrefix + "Documents2Documents (Document1ID, Document2ID, PropertyName) values ({0}, {1}, '{2}')",
                            id,
                            documentId,
                            idxProp.Name));
                    cmdContains.ExecuteNonQuery();
                }
            }
        }

        private void SerializeLazyList(
            int id, 
            List<int> listOfIDsOfChildren, 
            List<string> listOfParentPropertyNamesToNotDelete, 
            PropertyInfo idxProp, 
            ActiveFieldAttribute[] attrs, 
            object valueOfProperty, 
            IEnumerable enumerable, 
            PropertyInfo listRetrieved)
        {
            // LazyList...
            if (!(bool)listRetrieved.GetGetMethod(true).Invoke(enumerable, null))
            {
                // We want to exclude this entire list if it hasn't been retrieved yet in any ways ...!
                listOfParentPropertyNamesToNotDelete.Add(idxProp.Name);
            }
            else
            {
                if (attrs[0].BelongsTo)
                {
                    // Do nothing here ...
                    // The other side controls our relationship ...!
                }
                else if (attrs[0].IsOwner)
                {
                    foreach (object idxChild in enumerable)
                    {
                        TransactionalObject trs = idxChild as TransactionalObject;
                        trs.ParentDocument = id;
                        trs.ParentPropertyName = idxProp.Name;
                        trs.Save();
                        listOfIDsOfChildren.Add(trs.ID);
                    }
                }
                else
                {
                    // Delete old relationships whith this documentid and this property name
                    SqlCommand sqlDeleteRelationRecords = CreateSqlCommand(
                        string.Format("delete from " + TablePrefix +
                            "Documents2Documents where Document1ID={0} and PropertyName='{1}'",
                            id,
                            idxProp.Name));
                    sqlDeleteRelationRecords.ExecuteNonQuery();

                    foreach (object idxChild in enumerable)
                    {
                        TransactionalObject trs = idxChild as TransactionalObject;
                        int documentId = trs.ID;

                        SqlCommand cmdContains = CreateSqlCommand(
                            string.Format(
                                "insert into " + TablePrefix +
                                "Documents2Documents (Document1ID, Document2ID, PropertyName) values ({0}, {1}, '{2}')",
                                id,
                                documentId,
                                idxProp.Name));
                        cmdContains.ExecuteNonQuery();
                    }
                }
            }
        }

        private void SerializeSingleComplexChild(
            int id, 
            List<int> listOfIDsOfChildren, 
            PropertyInfo idxProp, 
            ActiveFieldAttribute[] attrs, 
            object valueOfProperty)
        {
            if (attrs[0].BelongsTo)
            {
                // Other side controls our relationship, and hence we need not do anything here ...
            }
            else if (attrs[0].IsOwner)
            {
                TransactionalObject trs = valueOfProperty as TransactionalObject;
                trs.ParentDocument = id;
                trs.ParentPropertyName = idxProp.Name;
                trs.Save();
                listOfIDsOfChildren.Add(trs.ID);
                int childId = trs.ID;
                listOfIDsOfChildren.Add(childId);
            }
            else // if (!attrs[0].IsOwner && BelongsTo == false)
            {
                string propertyRelationshipName = idxProp.Name;
                int childId = (int)valueOfProperty.GetType().GetProperty(
                    "ID",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic |
                    BindingFlags.Public)
                    .GetGetMethod()
                    .Invoke(valueOfProperty, null);

                SqlCommand deleteFromD2D = CreateSqlCommand(
                    string.Format("delete from " + TablePrefix + "Documents2Documents where Document1ID={0} and PropertyName='{1}'",
                        id,
                        propertyRelationshipName));
                deleteFromD2D.ExecuteNonQuery();

                SqlCommand cmdContains = CreateSqlCommand(
                    string.Format(
                        "insert into " + TablePrefix + "Documents2Documents (Document1ID, Document2ID, PropertyName) values ({0}, {1}, '{2}')",
                        id,
                        childId,
                        propertyRelationshipName));
                cmdContains.ExecuteNonQuery();
            }
        }

        private void DeleteNewlyRemovedChildObjects(
            int id, 
            PropertyInfo idxProp, 
            ActiveFieldAttribute[] attrs)
        {
            // Potentially newly lost reference or child...
            if (attrs[0].BelongsTo && string.IsNullOrEmpty(attrs[0].RelationName))
            {
                // Nothing needs to be done, the other side is controlling our relationship...
            }
            else if (attrs[0].BelongsTo /*&& hence RelationName is not empty*/)
            {
                // Nothing needs to be done, the other side is controlling our relationship...
                // This is, or should be at least, in fact a child object of the other ...!

                // Here we could theoretically do an update since it requires nothing but removal from
                // the Documents2Documents table, and hence touches nothing but the relationship itself
                // but which could be unintuitive since there might exist other references to the controlling
                // side of the equation where similar things did not occur, but which is serialized
                // later onwards ...
            }
            else if (attrs[0].IsOwner)
            {
                string relationshipName = idxProp.Name;
                string toDeleteRelationShip = 
                    string.Format("delete from Documents where Parent = {0} and ParentPropertyName='{1}'",
                    id,
                    relationshipName);
                SqlCommand cmdDeleteRelationShip = CreateSqlCommand(
                    toDeleteRelationShip);
                cmdDeleteRelationShip.ExecuteNonQuery();
            }
            else if (!attrs[0].IsOwner)
            {
                string relationshipName = idxProp.Name;
                string toDeleteRelationShip = 
                    string.Format("delete from Documents2Documents where Document1ID = {0} and PropertyName='{1}'",
                    id,
                    relationshipName);
                SqlCommand cmdDeleteRelationShip = CreateSqlCommand(
                    toDeleteRelationShip);
                cmdDeleteRelationShip.ExecuteNonQuery();
            }
        }

        public override void Close()
        {
            _connection.Close();
        }

        public void Save(string sessionId, string pageUrl, string content)
        {
            // Deleting *OLD* ViewState from table...
            string key = sessionId + "|" + pageUrl;
            SqlCommand sql = CreateSqlCommand(
                "delete from " + TablePrefix + "ViewStateStorage where ID='" + key + "' or Created < @dateNow");
            sql.Parameters.Add(new SqlParameter("@dateNow", DateTime.Now.AddHours(-4)));
            sql.ExecuteNonQuery();

            // Saving new value
            sql = CreateSqlCommand(
                "insert into " + TablePrefix + "ViewStateStorage (ID, Content, Created) values (@id, @content, @created)");
            sql.Parameters.Add(new SqlParameter("@id", key));
            sql.Parameters.Add(new SqlParameter("@content", content));
            sql.Parameters.Add(new SqlParameter("@created", DateTime.Now));
            sql.ExecuteNonQuery();
        }

        public string Load(string sessionId, string pageUrl)
        {
            // Retrieving ViewState
            string key = sessionId + "|" + pageUrl;
            SqlCommand sql = CreateSqlCommand(
                "select content from " + TablePrefix + "ViewStateStorage where ID=@id");
            sql.Parameters.Add(new SqlParameter("@id", key));
            string retVal = sql.ExecuteScalar() as string;
            return retVal;
        }
    }
}
