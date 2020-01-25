using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InitiativeTracker.Data.Util
{
    public static class ListExtensions
    {
        /// <summary>
        /// Allows the invoking list to be indexed using any signed integer. Conceptually
        /// creates a "wrapping" list. For example, given int[] array = {0,1,2,3}
        /// array.WrapIndex(4) returns 0, array.WrapIndex(5) returns 1, array.WrapIndex(-1)
        /// returns 3, and so on.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T WrapIndex<T>(this IList<T> list, int index)
        {
            if(list.Count == 0) throw new IndexOutOfRangeException("list must contain at least one element");
            if (index < list.Count && index > -1) return list[index]; //most basic case, index already in range
            if (index >= list.Count && index > -1) return list[index % list.Count]; //index out of range and non-negative

            index = Math.Abs(index) - 1;

            if (index < list.Count) return list[(list.Count - 1) - index];

            //range of index % list.Count is 0 <= x <= list.Count - 1
            return list[(list.Count - 1) - (index % list.Count)];
        }

        /// <summary>
        /// Works the same as the regular WrapIndex(int index) but also sets the value of index to an
        /// equivalent number if it goes above list.Count - 1 or below 0. This can be used to prevent
        /// OverflowErrors in long-running loops or situations in which you have an indexing value
        /// capable of being incremented more than Int32.MaxValue times.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T WrapIndex<T>(this IList<T> list, ref int index)
        {
            if (list.Count == 0) throw new IndexOutOfRangeException("list must contain at least one element");
            if (index < list.Count && index > -1) return list[index]; //most basic case, index already in range
            if (index >= list.Count && index > -1)
            {
                index = index % list.Count;
                return list[index];
            }

            index = Math.Abs(index) - 1;

            if (index < list.Count)
            {
                index = (list.Count - 1) - index;
                return list[index];
            }

            //range of index % list.Count is 0 <= x <= list.Count - 1
            index = (list.Count - 1) - (index % list.Count);
            return list[index];
        }

        /// <summary>
        /// Used for setting a variable to a value. Otherwise works the same as WrapIndex(int index)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public static void WrapIndex<T>(this IList<T> list, int index, T value)
        {
            if (list.Count == 0) throw new IndexOutOfRangeException("list must contain at least one element");

            if (index < list.Count && index > -1)
            {
                list[index] = value;
                return;
            }

            if (index >= list.Count && index > -1)
            {
                list[index % list.Count] = value;
                return;
            }

            index = Math.Abs(index) - 1;

            if (index < list.Count)
            {
                list[(list.Count - 1) - index] = value;
                return;
            }

            list[(list.Count - 1) - (index % list.Count)] = value;
        }

        public static void WrapIndex<T>(this IList<T> list, ref int index, T value)
        {
            if (list.Count == 0) throw new IndexOutOfRangeException("list must contain at least one element");

            if (index < list.Count && index > -1)
            {
                list[index] = value;
                return;
            }

            if (index >= list.Count && index > -1)
            {
                index = index % list.Count;
                list[index] = value;
                return;
            }

            index = Math.Abs(index) - 1;

            if (index < list.Count)
            {
                index = (list.Count - 1) - index;
                list[index] = value;
                return;
            }

            index = (list.Count - 1) - (index % list.Count);
            list[index] = value;
        }
    }
}