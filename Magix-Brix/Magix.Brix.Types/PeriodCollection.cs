/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Collections.Generic;

namespace Magix.Brix.Types
{
    /**
     * Level3: A collection class of Period types. Contains algebraic methods for 
     * OR, AND, XOR, NOT. Makes algebraic operations on collection of Period objects
     * very easy and intuitive. Can with for instance one line of code OR two
     * collections together to find the logically OR'ed result of these two different
     * collections. Very useful for manipulating dates and such
     */
    public class PeriodCollection : IList<Period>
    {
        List<Period> _list;

        /**
         * Level3: Default CTOR
         */
        public PeriodCollection()
        {
            _list = new List<Period>();
        }

        /**
         * Level3: Initializes the list with the given collection
         */
        public PeriodCollection(IEnumerable<Period> collection)
        {
            _list = new List<Period>(collection);
        }

        /**
         * Level3: Helper for making it possible to create a collection of Periods out
         * of a collection of other types of objects. E.g. if you've got a list
         * of 'Activities' which each contains start and end dates, you can easily
         * with a couple of lines of code transform that into a PeriodCollection
         * with the help of this delegate and the CreateCollection method
         */
        public delegate Period GetPeriodDelegate<T>(T value);

        /**
         * Level3: Takes an IEnumerable collection of type T and creates a 
         * PeriodCollection of it calling the given predicate for every object, which
         * again is expected to return a Period for each object of type T
         */
        public static PeriodCollection CreateCollection<T>(IEnumerable<T> collection, GetPeriodDelegate<T> retriever)
        {
            PeriodCollection retVal = new PeriodCollection();
            foreach (T idx in collection)
            {
                Period tmp = retriever(idx);
                if (tmp != Period.Empty)
                    retVal.Add(tmp);
            }
            return retVal;
        }

        /**
         * Level3: Returns the lowest start date of the collection
         */
        public DateTime Starts
        {
            get
            {
                PeriodCollection tmp = new PeriodCollection(this);
                tmp.Normalize();
                return tmp[0].Start;
            }
        }

        /**
         * Level3: Returns the highest end date of the collection
         */
        public DateTime Ends
        {
            get
            {
                PeriodCollection tmp = new PeriodCollection(this);
                tmp.Normalize();
                return tmp[tmp.Count - 1].End;
            }
        }

        /**
         * Level3: Sorts and runs through all period objects and checks for overlapping, eliminating
         * overlapping periods merging them into one and such. Kind of like the same
         * mathematical operation as normalizing a vector.
         * Crucial for most of the algebraic operations. This operation will most often
         * make the containing number of items less then it used to be before the operation.
         * Meaning two Periods that overlap will become one 'combined' period after this method
         * is done
         */
        public void Normalize()
        {
            // Sorting makes this operation far faster and could also be argued is a part of
            // the normalization process...
            Sort();

            List<Period> newList = new List<Period>();
            for (int idx = 0; idx < _list.Count; idx++)
            {
                Period tmp = _list[idx];
                while (idx + 1 < _list.Count &&
                    (Period.Intersects(tmp, _list[idx + 1]) || Period.Continues(tmp, _list[idx + 1])))
                {
                    tmp = Period.OR(tmp, _list[idx + 1]);
                    idx += 1;
                }
                newList.Add(tmp);
            }
            _list = newList;
        }

        #region [ -- Algabraic Methods -- ]

        /**
         * Level3: Returns the logically ORed lists back to caller. Does not in 
         * any ways change the given lists.
         */
        public static PeriodCollection OR(PeriodCollection left, PeriodCollection right)
        {
            // Merging the two lists into one resulting list
            PeriodCollection retVal = new PeriodCollection(left);
            retVal.AddRange(right);

            // Since ORing of two PeriodCollection objects logically is the same as 
            // Normalizing them, we just use the Normalize method...
            retVal.Normalize();
            return retVal;
        }

        /**
         * Level3: Logically ANDs two collections together returning the ANDed result. 
         * Basically a new collection of Period objects which consists of the 
         * date ranges that overlaps within both lists. Useful for finding out for 
         * instance when two period collections overlap
         */
        public static PeriodCollection AND(PeriodCollection left, PeriodCollection right)
        {
            // If one of the lists are empty the ANDed result will never have values...
            if (left.Count == 0 || right.Count == 0)
                return new PeriodCollection();

            // Creating two copies which we can normalize since
            // we don't want to change the given arguments.
            // Have methods NOT having side-effects...
            PeriodCollection tmpLeft = new PeriodCollection(left);
            tmpLeft.Normalize();

            PeriodCollection tmpRight = new PeriodCollection(right);
            tmpRight.Normalize();

            // Short circut parts for optimizations...
            // Checking for NO overlappings...
            if (tmpLeft[tmpLeft.Count - 1].End < tmpRight[0].Start)
                return new PeriodCollection();
            if (tmpRight[tmpRight.Count - 1].End < tmpLeft[0].Start)
                return new PeriodCollection();

            // OK, we do (probably) have overlapping and we must traverse items to check for overlapping
            // on individual items...
            PeriodCollection retVal = new PeriodCollection();

            foreach (Period idxLeft in tmpLeft)
            {
                foreach (Period idxRight in tmpRight)
                {
                    // Optimization - no need to look further since both lists are normalized and sorted...
                    if (idxRight.Start > idxLeft.End)
                        break;

                    // Items must intersect in order to be ANDable...
                    if (Period.Intersects(idxLeft, idxRight))
                    {
                        Period toAdd = Period.AND(idxLeft, idxRight);
                        retVal.Add(toAdd);
                    }
                }
            }
            return retVal;
        }

        /**
         * Level3: Returning the NOT operation of the this list. Basically all the places where there are
         * NO Periods within the collection of Periods. Kind of the "negative" of the collection.
         * Notice that since we do NOT threat a PeriodCollection as an "open end collection" but rather
         * as a min value of DateTime.MinValue and a max value of DateTime.MaxValue we do not get
         * the "elephant footstep" as we would otherwise be forced to have. Meaning that the
         * operation is 'reversible' by calling it twice.
         */
        public PeriodCollection NOT()
        {
            // Creating a copy which we can normalize since
            // we don't want to change the this argument.
            // Have methods NOT having side-effects...
            PeriodCollection tmpLeft = new PeriodCollection(this);
            tmpLeft.Normalize();

            PeriodCollection retVal = new PeriodCollection();

            for (int idx = 0; idx < tmpLeft.Count - 1; idx++)
            {
                DateTime start = tmpLeft[idx].End;
                DateTime end = tmpLeft[idx + 1].Start;
                retVal.Add(new Period(start, end));
            }

            if (tmpLeft.Count > 0)
            {
                if (tmpLeft[0].Start > DateTime.MinValue)
                    retVal.Insert(0, new Period(DateTime.MinValue, tmpLeft[0].Start));

                if (tmpLeft[tmpLeft.Count - 1].End < DateTime.MaxValue)
                    retVal.Add(new Period(tmpLeft[tmpLeft.Count - 1].End, DateTime.MaxValue));
            }
            return retVal;
        }

        /**
         * Level3: Returning the logical XOR of two given collections. The XOR result is the
         * place where ONE and ONE ONLY of the given collections have values. This will
         * normally increase the number of periods in your collection. Doesn't change
         * the incoming periods [left, right]
         */
        public static PeriodCollection XOR(PeriodCollection left, PeriodCollection right)
        {
            // Creating two normalized copies of collections
            PeriodCollection tmpLeft = new PeriodCollection(left);
            tmpLeft.Normalize();

            PeriodCollection tmpRight = new PeriodCollection(right);
            tmpRight.Normalize();

            // Temporary ANDing up these two normalized collections
            PeriodCollection andTmp = AND(tmpLeft, tmpRight);

            // Temporary NOTing this AND
            PeriodCollection notOfAnd = andTmp.NOT();

            // Temporary creating OR of originals
            PeriodCollection orOfOriginals = OR(tmpLeft, tmpRight);

            // ANDing the temporary NOT and OR together to create result
            PeriodCollection andOfNotAndOr = AND(notOfAnd, orOfOriginals);

            // Returning result
            return andOfNotAndOr;
        }

        /**
         * Level3: Returns the total amount of time in the this collection
         */
        public TimeSpan Sum()
        {
            Normalize();
            TimeSpan retVal = new TimeSpan();
            foreach (Period idx in this)
            {
                retVal += idx.Length;
            }
            return retVal;
        }

        /**
         * Level3: Trimming away everything that's not within given Period
         */
        public void Trim(Period period)
        {
            Normalize();

            // First trimming away everything that's totally not within borders
            List<Period> toBeRemoved = new List<Period>();
            foreach (Period idx in this)
            {
                if (!Period.Intersects(idx, period))
                    toBeRemoved.Add(idx);
            }

            foreach (Period idx in toBeRemoved)
                Remove(idx);

            // In case we emptied the collection
            if (Count == 0)
                return;

            // Making sure start and end are within given trimming period
            if (this[0].Start < period.Start)
                this[0] = new Period(period.Start, this[0].End);

            if (this[Count - 1].End > period.End)
                this[Count - 1] = new Period(this[Count - 1].Start, period.End);
        }

        #endregion

        #region [ -- List Helper Methods -- ]

        // TODO: Create one of these 'delegate fillers' here ... [see delegate above]
        /**
         * Level3: Adds the given range of periods into the collection
         */
        public void AddRange(IEnumerable<Period> collection)
        {
            foreach (Period idx in collection)
            {
                Add(idx);
            }
        }

        /**
         * Level3: Takes a predicate and sort the list accordingly
         */
        public void Sort(Comparison<Period> comparison)
        {
            _list.Sort(comparison);
        }

        /**
         * Level3: Sorts according to a predicate that sorts first prioritized after 
         * start of periods and then according to end of periods if start of periods 
         * are the same. Used in the Normalize method
         */
        public void Sort()
        {
            Sort(
                delegate(Period left, Period right)
                {
                    if (left.Start < right.Start)
                        return -1;

                    if (left.Start > right.Start)
                        return 1;

                    return left.End.CompareTo(right.End);
                });
        }

        /**
         * Level3: Takes a predicate and returns the first Period that matches the predicate
         */
        public Period Find(Predicate<Period> predicate)
        {
            return _list.Find(predicate);
        }

        /**
         * Level3: Returns a list of Periods that matches the predicate
         */
        public List<Period> FindAll(Predicate<Period> predicate)
        {
            return _list.FindAll(predicate);
        }

        /**
         * Level3: Removes all the periods that matches the predicate
         */
        public void RemoveAll(Predicate<Period> predicate)
        {
            _list.RemoveAll(predicate);
        }

        #endregion

        #region [ -- IList<Period> Members -- ]

        /**
         * Level3: Returns the index of the given item, or -1 if no found
         */
        public int IndexOf(Period item)
        {
            return _list.IndexOf(item);
        }

        /**
         * Level3: Inserts the given period at the given index
         */
        public void Insert(int index, Period item)
        {
            _list.Insert(index, item);
        }

        /**
         * Level3: Removes the period at the given index
         */
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        /**
         * Level3: Returns the period at the given index. Will throw if out of bounds.
         * Setter will replace the existing period at the given index.
         */
        public Period this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        /**
         * Level3: Appends to the back of the list the given period.
         */
        public void Add(Period item)
        {
            _list.Add(item);
        }

        /**
         * Level3: Completely clears all periods from the list
         */
        public void Clear()
        {
            _list.Clear();
        }

        /**
         * Level3: Returns true if the given period is found in the list
         */
        public bool Contains(Period item)
        {
            return _list.Contains(item);
        }

        /**
         * Level4: Copy the list of periods into the given 
         * array starting at arrayIndex in the collection
         */
        public void CopyTo(Period[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /**
         * Level3: Returns the number of items in the collection
         */
        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /**
         * Level3: Removes the given period from the collection
         */
        public bool Remove(Period item)
        {
            return _list.Remove(item);
        }

        /**
         * Level3: Enumerating support
         */
        public IEnumerator<Period> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }
}
