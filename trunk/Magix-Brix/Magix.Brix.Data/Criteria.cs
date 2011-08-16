/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

namespace Magix.Brix.Data
{
    /**
     * Level3: Abstract base class for all data storage retrieval criterias.
     * Also contains several handy static constructors for easy
     * creation of data storage retrieval criterias.
     */
    public abstract class Criteria
    {
        private readonly string _propertyName;
        private readonly object _value;

        protected Criteria(string propertyName, object value)
        {
            _propertyName = propertyName;
            _value = value;
        }

        /**
         * Level3: Name of property associated with criteria.
         */
        public string PropertyName
        {
            get { return _propertyName; }
        }

        /**
         * Level3: Value that criteria associates with the given property
         * of your criteria.
         */
        public object Value
        {
            get { return _value; }
        }

        /**
         * Level3: Static constructor to create a criteria of type Equals.
         */
        public static Criteria Eq(string propertyName, object value)
        {
            return new Equals(propertyName, value);
        }

        /**
         * Level3: Static constructor to create a criteria of type NotEquals.
         */
        public static Criteria Ne(string propertyName, object value)
        {
            return new NotEquals(propertyName, value);
        }

        /**
         * Level3: Static constructor to create a criteria of type LikeEquals. Notice that
         * this is equal to the LIKE keyword from SQL, meaning you can use % and _ as control
         * characters to look for 'wild card combinations'. Laymen terms; % == * and _ == ?
         */
        public static Criteria Like(string propertyName, string value)
        {
            return new LikeEquals(propertyName, value);
        }

        /**
         * Level3: Static constructor to create a criteria of type CritID. Will create a Criteria
         * that will demand the ID of the object is id
         */
        public static Criteria Id(int id)
        {
            return new CritID(id);
        }

        /**
         * Level3: Static constructor to create a criteria of type CritNoID. Will create a Criteria
         * that will demand the ID of the object is NOT id
         */
        public static Criteria NotId(int id)
        {
            return new CritNoID(id);
        }

        /**
         * Level3: Static constructor to create a criteria of type LikeNotEquals. The opposite of
         * Like
         */
        public static Criteria NotLike(string propertyName, string value)
        {
            return new LikeNotEquals(propertyName, value);
        }

        /**
         * Level3: Static constructor to create a criteria of type LessThen. Depends upon the
         * column type, but normally a dash of reason can deduct this, regardless of the column
         * type. String are compared alphabetically, all other from a 'least is less' standpoint.
         * Will return only objects that are 'Less than' the given value
         */
        public static Criteria Lt(string propertyName, object value)
        {
            return new LessThen(propertyName, value);
        }

        /**
         * Level3: Static constructor to create a criteria of type MoreThen. 'Opposite' of Lt.
         * See Lt
         */
        public static Criteria Mt(string propertyName, object value)
        {
            return new MoreThen(propertyName, value);
        }

        /**
         * Level3: Static constructor to create a criteria of type ParentIdEquals. Will
         * enforce that the only objects being chosen are all belonging to a parent object,
         * with the given id
         */
        public static Criteria ParentId(int id)
        {
            return new ParentIdEquals(id);
        }

        /**
         * Level3: Static constructor to create a criteria of type ExistsInEquals.
         * Which will make sure there exists a relationship between the resulting object
         * and the object with the given id in such a way that the other object
         * are 'referencing' this one in one way or another. If you need to express
         * the 'opposite', as in 'return objects who are referencing id object', then
         * use the overloaded version of this method and pass in true as the reversed
         * parameter. This will ensure that only objects which themselves are referencing
         * the id object will be returned. For instance if a Customer has a LazyList of
         * Contacts, you could find all Contacts referenced in a specific customer
         * through using Contact.Select(Crieria.ExistsIn(customerId))
         * 
         * If you want to go the other way, as in finding all Customers that are referencing
         * a specific contact you'd have to go Customer.Select(Criteria.ExistsIn(contactIs, true))
         * 
         * The above works, as long as the 'IsOwner' parts of a relationship equals false. If it's
         * an 'IsOwner type of relationship', you'd rather using IsChild and IsParent methods
         */
        public static Criteria ExistsIn(int id)
        {
            return new ExistsInEquals(id, false);
        }

        /**
         * Level3: Static constructor to create a REVERSED criteria of type ExistsInEquals.
         * See the documentation to the overload for an explanation
         */
        public static Criteria ExistsIn(int id, bool reversed)
        {
            return new ExistsInEquals(id, reversed);
        }

        /**
         * Level3: Static constructor to create a criteria of type HasChildId. Will only
         * return objects that have the given id as their 'child objects' [meaning 'IsOwner' 
         * == true in its ActiveField declaration]
         */
        public static Criteria HasChild(int id)
        {
            return new HasChildId(id);
        }

        /**
         * Level3: Static constructor to create a criteria of type Sort. Will sort on the Column name
         */
        public static Criteria Sort(string colName)
        {
            return new SortOn(colName);
        }

        /**
         * Level3: Static constructor to create a criteria of type Sort. Will sort on the column
         * name. ascending decides whether or not ascending or descending
         */
        public static Criteria Sort(string colName, bool ascending)
        {
            return new SortOn(colName, ascending);
        }

        /**
         * Level3: Static constructor to create a criteria of type Range. Will first of all
         * sort acording to the sortColumn, either ascending or descending depending upon the 
         * 'ascending' parameter. Then it will return only the [start, end} result set from that
         * dataset. Executes actually __BLISTERING__ fast, even thought you 
         * probably wouldn't believe so ... ;)
         * 
         * Basically the 'foundation' for the Grid System in many ways ...
         */
        public static Criteria Range(int start, int end, string sortColumn, bool ascending)
        {
            return new CritRange(start, end, sortColumn, ascending);
        }
    }

    /*
     * A criteria that makes sure your returned object is the parent 
     * of the object with the given id
     */
    public class HasChildId : Criteria
    {
        public HasChildId(int id)
            : base(null, id)
        { }
    }

    /*
     * A criteria that sorts the result
     */
    public class SortOn : Criteria
    {
        private bool _ascending;

        public SortOn(string columnName)
            : this(columnName, true)
        { }

        public SortOn(string columnName, bool ascending)
            : base(null, columnName)
        {
            _ascending = ascending;
        }

        public bool Ascending
        {
            get { return _ascending; }
        }
    }

    public class CritRange : Criteria
    {
        private int _start;
        private int _end;
        private string _sortColumn;
        private bool _ascending;

        public CritRange(int start, int end, string sortColumn, bool ascending)
            : base(sortColumn, null)
        {
            _start = start;
            _end = end;
            _sortColumn = sortColumn;
            _ascending = ascending;
        }

        public int Start
        {
            get { return _start; }
        }

        public int End
        {
            get { return _end; }
        }

        public string SortColumn
        {
            get { return _sortColumn; }
            set { _sortColumn = value; }
        }

        public bool Ascending
        {
            get { return _ascending; }
            set { _ascending = value; }
        }
    }

    /*
     * A criteria that makes sure your object must exist within the object with the
     * given ID in order to be true. Notice that this one becomes true BOTH ways for
     * a IsOwner=false relationship since logically if it's not an owner there is an
     * "exists in" relationship between both of the objects both ways.
     * So if you have a LazyList which the object containing the list is not the 
     * owner of any child elements, then you can run ExistIn Criteria both ways
     * from either the "parent object" or the "child object" and they will both
     * return as true.
     */
    public class ExistsInEquals : Criteria
    {
        public ExistsInEquals(int id, bool reversed)
            : base(null, id)
        {
            Reversed = reversed;
        }

        public bool Reversed { get; set; }
    }

    /*
     * A criteria that only returns true if the object is a child (IsOwner=true)
     * of the object with the given ID.
     */
    public class ParentIdEquals : Criteria
    {
        public ParentIdEquals(int id)
            : base(null, id)
        { }
    }

    /*
     * Returns only true if the property with the given name has the given value.
     */
    public class Equals : Criteria
    {
        public Equals(string propertyName, object value)
            : base(propertyName, value)
        { }
    }

    /*
     * Returns only true if the property with the given name does NOT have the given value.
     */
    public class NotEquals : Criteria
    {
        public NotEquals(string propertyName, object value)
            : base(propertyName, value)
        { }
    }

    /*
     * Returns only true if the property with the given name contains the given value
     * string. Only usable for string types.
     */
    public class LikeEquals : Criteria
    {
        public LikeEquals(string propertyName, string value)
            : base(propertyName, value)
        { }
    }

    /*
     * Specific ID for instance ...
     */
    public class CritID : Criteria
    {
        public CritID(int id)
            : base(null, id)
        { }
    }

    /*
     * Specific ID for instance ...
     */
    public class CritNoID : Criteria
    {
        public CritNoID(int id)
            : base(null, id)
        { }
    }

    /*
     * Returns only true if the property with the given name does NOT contain the given value
     * string. Only usable for string types.
     */
    public class LikeNotEquals : Criteria
    {
        public LikeNotEquals(string propertyName, string value)
            : base(propertyName, value)
        { }
    }

    /*
     * Returns only true if the property with the given name have a value which is
     * "less" then the given value.
     */
    public class LessThen : Criteria
    {
        public LessThen(string propertyName, object value)
            : base(propertyName, value)
        { }
    }

    /*
     * Returns only true if the property with the given name have a value which is
     * "more" then the given value.
     */
    public class MoreThen : Criteria
    {
        public MoreThen(string propertyName, object value)
            : base(propertyName, value)
        { }
    }
}
