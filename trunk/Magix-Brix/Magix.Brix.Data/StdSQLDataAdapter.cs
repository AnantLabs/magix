/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Configuration;
using System.Collections.Generic;

namespace Magix.Brix.Data.Internal
{
    /**
      * Common logic for all Database adapters that relies on standard SQL syntax. Wrappers
      * for creating SQL text for derived data adapters that uses RDBS that relies on standard
      * SQL syntax.
      */
    public abstract class StdSQLDataAdapter : Adapter
    {
        protected class LazyHelper
        {
            private readonly int _parentId;
            private readonly Type _type;
            private readonly bool _isOwner;
            private readonly bool _belongsTo;
            private readonly string _propertyName;
            private readonly string _relationName;

            public LazyHelper(Type type, int parentId, bool isOwner, bool belongsTo, string propertyName, string relationName)
            {
                _type = type;
                _parentId = parentId;
                _isOwner = isOwner;
                _propertyName = propertyName;
                _belongsTo = belongsTo;
                _relationName = relationName;
            }

            public IEnumerable GetItems()
            {
                if (_belongsTo)
                {
                    if (string.IsNullOrEmpty(_relationName))
                    {
                        return Instance.Select(
                            _type,
                            _propertyName,
                            Criteria.HasChild(_parentId));
                    }
                    else
                    {
                        return Instance.Select(
                            _type,
                            _relationName,
                            Criteria.ExistsIn(_parentId, true));
                    }
                }
                else
                {
                    if (_isOwner)
                    {
                        return Instance.Select(
                            _type, 
                            _propertyName, 
                            Criteria.ParentId(_parentId));
                    }
                    else
                    {
                        return Instance.Select(
                            _type, 
                            _propertyName, 
                            Criteria.ExistsIn(_parentId, false));
                    }
                }
            }
        }

        private static string _tablePrefix;

        protected virtual string TablePrefix
        {
            get
            {
                if (_tablePrefix == null)
                {
                    lock (this)
                    {
                        if (_tablePrefix == null)
                        {
                            _tablePrefix = ConfigurationManager.AppSettings["LegoTablePrefix"];
                            if (_tablePrefix == null)
                                _tablePrefix = "dbo.";
                            if (!_tablePrefix.Contains("."))
                                _tablePrefix += ".";
                        }
                    }
                }
                return _tablePrefix;
            }
        }

        protected string CreateSelectStatementForDocument(
            Type type, 
            string propertyName, 
            Criteria[] args)
        {
            string retVal = "";
            string join = "";
            string where = "";
            string order = "";

            // Used as signal objects ...
            CritRange range = null;
            SortOn sort = null;

            // We need to check to see if there exists both a SortOn and
            // a CritRange in our collection of Criteria, if there does, we 
            // need to take the SortColumn from our SortOn Criteria, and stuff into
            // our CritRange Criteria - plus in the future; Also add up ASC/DESC
            // depending upon if it's there or not ...
            List<Criteria> toConvertTo = new List<Criteria>(args);
            foreach (Criteria idx in toConvertTo)
            {
                if (idx is CritRange)
                    range = idx as CritRange;
                else if (idx is SortOn)
                    sort = idx as SortOn;
            }
            if (sort != null && range != null)
            {
                range.SortColumn = sort.Value.ToString();
                range.Ascending = sort.Ascending;

                // Now that we have copied the sorting from our
                // sorting Criteria, since Sort has priority on columns,
                // we can safely remove the Sort out of the collection ...
                toConvertTo.RemoveAll(
                    delegate(Criteria idx)
                    {
                        if (idx is SortOn)
                            return true;
                        return false;
                    });
                args = toConvertTo.ToArray();
            }

            // Now that we have removed out the Sort parameter, and merged it
            // with the Range parameter, we can safely reset the "sort" and "range" properties
            // to null again, to use them as signals again, later down in our code ...
            range = null;
            sort = null;

            foreach (Criteria idx in args)
            {
                if (idx is SortOn)
                {
                    SortOn tmp = idx as SortOn;
                    sort = tmp;
                    string propName = (string)tmp.Value;
                    bool ascending = tmp.Ascending;
                    PropertyInfo info = type.GetProperty(propName);
                    string tableName = "";
                    switch (info.PropertyType.FullName)
                    {
                        case "System.String":
                            tableName = "PropertyStrings";
                            break;
                        case "System.Boolean":
                            tableName = "PropertyBools";
                            break;
                        case "System.DateTime":
                            tableName = "PropertyDates";
                            break;
                        case "System.Guid":
                            tableName = "PropertyGuids";
                            break;
                        case "System.Int32":
                            tableName = "PropertyInts";
                            break;
                        case "System.Decimal":
                            tableName = "PropertyDecimals";
                            break;
                    }
                    join += ", " + tableName + " as prop";
                    where += " and prop.FK_Document = d.ID and prop.Name = 'prop" + propName + "'";
                    order += " order by prop.Value " + (ascending ? "asc" : "desc");
                }
                else if (idx is CritRange)
                {
                    range = idx as CritRange;
                }
            }
            if (range != null)
            {
                int start = range.Start;
                int end = range.End;
                string sortColumn = range.SortColumn;
                if (start >= end)
                    throw new ApplicationException(
                        @"End must be higher than Start in your select query ...");
                string tableOfRangeColumnName = "";
                if (sortColumn != "ID")
                {
                    switch (type.GetProperty(
                        sortColumn,
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic).PropertyType.FullName)
                    {
                        case "System.Boolean":
                            tableOfRangeColumnName = "PropertyBools";
                            break;
                        case "System.DateTime":
                            tableOfRangeColumnName = "PropertyDates";
                            break;
                        case "System.Guid":
                            tableOfRangeColumnName = "PropertyGuids";
                            break;
                        case "System.Decimal":
                            tableOfRangeColumnName = "PropertyDecimals";
                            break;
                        case "System.Int32":
                            tableOfRangeColumnName = "PropertyInts";
                            break;
                        case "System.String":
                            tableOfRangeColumnName = "PropertyStrings";
                            break;
                        default:
                            throw new ApplicationException(
                                "Cannot sort on columns of that type");
                    }
                }
                int delta = end - start;
                if (start == 0)
                {
                    if (sortColumn == "ID")
                    {
                        // Special treatment...
                        retVal += string.Format(@"
    select top {4} d.ID from {0}Documents as d 
            {1}{2}{3}
    order by {5}{6}",
                            TablePrefix,
                            join,
                            CreateCriteriasForDocument(type, propertyName, args),
                            where,
                            end,
                            sortColumn,
                        (range.Ascending ? "" : " DESC"));
                    }
                    else
                    {
                        retVal += string.Format(@"
    select top {4} d.ID from {0}Documents as d 
        inner join {0}{7} as prop on 
            prop.FK_Document=d.ID and prop.Name='prop{5}' {1}{2}{3}
    order by prop.Value{6}",
                            TablePrefix,
                            join,
                            CreateCriteriasForDocument(type, propertyName, args),
                            where,
                            end,
                            sortColumn,
                        (range.Ascending ? "" : " DESC"),
                        tableOfRangeColumnName);
                    }
                }
                else
                {
                    if (sortColumn == "ID")
                    {
                        // Special treatment...
                        retVal += string.Format(@"
select ID from (select top {6} ID from (
    select top {5} d.ID as ID from {0}Documents as d
    {1}{2}{3}
    order by {4}{7}) as Tbl1
order by {4}{8}) as Tbl2 order by {4}{7}",
                            TablePrefix,
                            join,
                            CreateCriteriasForDocument(type, propertyName, args),
                            where,
                            sortColumn,
                            end,
                            delta,
                            (range.Ascending ? "" : " desc"),
                            (range.Ascending ? " desc" : " "));
                    }
                    else
                    {
                        retVal += string.Format(@"
select ID, Value from (select top {6} ID, Value from (
    select top {5} d.ID as ID, prop.Value as Value from {0}Documents as d, {0}{9} as prop
    {1}{2}{3} and prop.Name='prop{4}' and prop.FK_Document=d.ID
    order by Value{7}) as Tbl1
order by Value{8}) as Tbl2 order by Value{7}",
                            TablePrefix,
                            join,
                            CreateCriteriasForDocument(type, propertyName, args),
                            where,
                            sortColumn,
                            end,
                            delta,
                            (range.Ascending ? "" : " desc"),
                            (range.Ascending ? " desc" : " "),
                            tableOfRangeColumnName);
                    }
                }
            }
            else
            {
                retVal += "select d.ID from " + TablePrefix + "Documents as d";
                retVal += join;
                retVal += CreateCriteriasForDocument(type, propertyName, args);
                retVal += where;
                retVal += order;
            }
            return retVal;
        }

        protected string CreateCriteriasForDocument(
            Type type, 
            string propertyName, 
            Criteria[] args)
        {
            string where = "";
            string order = "";
            if (type == null)
                where += " where ID!=NULL";
            else
                where += string.Format(" where TypeName='{0}'", Helpers.TypeName(type));
            if (propertyName != null)
            {
                if (!(args != null && args.Length > 0 && args[0] is ExistsInEquals))
                    where += " and ParentPropertyName='" + propertyName + "'";
            }
            List<PropertyInfo> props =
                new List<PropertyInfo>(
                    type.GetProperties(
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic));
            if (args != null)
            {
                foreach (Criteria idx in args)
                {
                    if (idx is SortOn)
                    {
                        ; // Intentionally drop over...
                    }
                    else if (idx is CritRange)
                    {
                        ; // Intentionally drop over...
                    }
                    else if (idx is ParentIdEquals)
                    {
                        where += " and Parent = " + idx.Value;
                    }
                    else if (idx is CritNoID)
                    {
                        where += " and ID != " + idx.Value;
                    }
                    else if (idx is HasChildId)
                    {
                        where += string.Format(" and exists(select ID from " + 
                            TablePrefix + 
                            "Documents d2 where d2.Parent=d.ID and d2.ID={0})", idx.Value);
                    }
                    else if (idx is ExistsInEquals)
                    {
                        ExistsInEquals exi = idx as ExistsInEquals;
                        string parentType = "";
                        if (propertyName != null)
                        {
                            parentType = string.Format(" and PropertyName='{0}'", propertyName);
                        }
                        if (exi.Reversed)
                        {
                            where += string.Format(
                                " and exists(select * from " +
                                TablePrefix +
                                "Documents2Documents d2 where (d2.Document2ID={0} and d2.Document1ID=d.ID){1})", idx.Value, parentType);
                        }
                        else
                        {
                            where += string.Format(
                                " and exists(select * from " +
                                TablePrefix +
                                "Documents2Documents d2 where (d2.Document1ID={0} and d2.Document2ID=d.ID){1})", idx.Value, parentType);
                        }
                    }
                    else
                    {
                        bool isInnerJoin = false;
                        string propertyFullName;
                        if (idx.PropertyName.Contains("."))
                        {
                            isInnerJoin = true;
                            Criteria criteria = idx;
                            PropertyInfo currentProperty = props.Find(
                                delegate(PropertyInfo idxProp)
                                {
                                    return idxProp.Name == criteria.PropertyName.Split('.')[0];
                                });
                            propertyFullName = currentProperty.PropertyType.GetProperty(
                                idx.PropertyName.Split('.')[1],
                                BindingFlags.Instance |
                                BindingFlags.NonPublic |
                                BindingFlags.Public).PropertyType.FullName;
                        }
                        else
                        {
                            Criteria criteria = idx;
                            PropertyInfo currentProperty = props.Find(
                                delegate(PropertyInfo idxProp)
                                {
                                    return idxProp.Name == criteria.PropertyName;
                                });
                            propertyFullName = currentProperty.PropertyType.FullName;
                        }
                        string tableName = "";
                        string sqlEscapedValue = "";
                        if(idx.Value == null)
                            sqlEscapedValue = "null";
                        switch (propertyFullName)
                        {
                            case "System.Boolean":
                                tableName = "PropertyBools";
                                if (idx.Value != null)
                                    sqlEscapedValue = ((bool)idx.Value) ? "1" : "0";
                                break;
                            case "System.DateTime":
                                tableName = "PropertyDates";
                                if (idx.Value != null)
                                    sqlEscapedValue = "'" + ((DateTime)idx.Value).ToString("yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture) + "'";
                                break;
                            case "System.Decimal":
                                tableName = "PropertyDecimals";
                                if (idx.Value != null)
                                    sqlEscapedValue = ((decimal)idx.Value).ToString(CultureInfo.InvariantCulture);
                                break;
                            case "System.Int32":
                                tableName = "PropertyInts";
                                if (idx.Value != null)
                                    sqlEscapedValue = idx.Value.ToString();
                                break;
                            case "System.Guid":
                                tableName = "PropertyGuids";
                                if (idx.Value != null)
                                    sqlEscapedValue = "'" + idx.Value.ToString() + "'";
                                break;
                            case "System.String":
                                tableName = "PropertyStrings";
                                if (idx.Value != null)
                                    sqlEscapedValue = "'" + idx.Value.ToString().Replace("'", "''").Replace("\r", "\\r").Replace("\n", "\\n") + "'";
                                break;
                        }

                        string whereAdd;
                        string docIdReference = isInnerJoin ? "d2.ID" : "d.ID";
                        string propertySqlName = isInnerJoin ? idx.PropertyName.Split('.')[1] : idx.PropertyName;
                        switch (idx.GetType().FullName)
                        {
                            case "Magix.Brix.Data.Equals":
                                whereAdd = string.Format(
                                    " and exists(select * from {0} as q{0} where q{0}.Value={1} and {3}=q{0}.FK_Document and q{0}.Name='{2}')",
                                    tableName,
                                    sqlEscapedValue,
                                    Helpers.PropertyName(propertySqlName),
                                    docIdReference);
                                break;
                            case "Magix.Brix.Data.NotEquals":
                                whereAdd = string.Format(
                                    " and exists(select * from {0} as q{0} where q{0}.Value<>{1} and {3}=q{0}.FK_Document and q{0}.Name='{2}')",
                                    tableName,
                                    sqlEscapedValue,
                                    Helpers.PropertyName(propertySqlName),
                                    docIdReference);
                                break;
                            case "Magix.Brix.Data.LikeEquals":
                                whereAdd = string.Format(
                                    " and exists(select * from {0} as q{0} where q{0}.Value like {1} and {3}=q{0}.FK_Document and q{0}.Name='{2}')",
                                    tableName,
                                    sqlEscapedValue,
                                    Helpers.PropertyName(propertySqlName),
                                    docIdReference);
                                break;
                            case "Magix.Brix.Data.LikeNotEquals":
                                whereAdd = string.Format(
                                    " and exists(select * from {0} as q{0} where q{0}.Value not like {1} and {3}=q{0}.FK_Document and q{0}.Name='{2}')",
                                    tableName,
                                    sqlEscapedValue,
                                    Helpers.PropertyName(propertySqlName),
                                    docIdReference);
                                break;
                            case "Magix.Brix.Data.LessThen":
                                whereAdd = string.Format(
                                    " and exists(select * from {0} as q{0} where q{0}.Value<{1} and {3}=q{0}.FK_Document and q{0}.Name='{2}')",
                                    tableName,
                                    sqlEscapedValue,
                                    Helpers.PropertyName(propertySqlName),
                                    docIdReference);
                                break;
                            case "Magix.Brix.Data.MoreThen":
                                whereAdd = string.Format(
                                    " and exists(select * from {0} as q{0} where q{0}.Value>{1} and {3}=q{0}.FK_Document and q{0}.Name='{2}')",
                                    tableName,
                                    sqlEscapedValue,
                                    Helpers.PropertyName(propertySqlName),
                                    docIdReference);
                                break;
                            default:
                                throw new ArgumentException("Unknown Criteria type '" + idx.GetType().FullName + "'");
                        }
                        if (isInnerJoin)
                        {
                            string whereInner = string.Format(@"
and ((exists(select d2.ID from " + TablePrefix + @"Documents as d2 where ParentPropertyName='{1}' and Parent=d.ID {0}))
or exists(
select d3.Document1ID from " + TablePrefix + @"Documents2Documents as d3 where PropertyName='{1}' and Document1ID=d.id {2}))

",
                                whereAdd,
                                idx.PropertyName.Split('.')[0],
                                whereAdd.Replace("d2.ID", "d3.Document2ID"));
                            whereAdd = whereInner;
                        }
                        where += whereAdd;
                    }
                }
            }
            return where + " " + order;
        }
    }
}
