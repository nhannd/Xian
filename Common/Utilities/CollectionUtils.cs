using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Provides a set of methods for performing functional-style operations on collections.
    /// </summary>
    public static class CollectionUtils
    {
        /// <summary>
        /// Selects all items in the target collection that match the specified predicate, returning
        /// them as a new collection of the specified type.
        /// </summary>
        /// <typeparam name="TItem">The type of items in the target collection</typeparam>
        /// <typeparam name="TResultCollection">The type of collection to return</typeparam>
        /// <param name="target">The collection to operate on</param>
        /// <param name="predicate">The predicate to test</param>
        /// <returns>A collection containing the subset of matching items from the target collection</returns>
        public static TResultCollection Select<TItem, TResultCollection>(IEnumerable<TItem> target, Predicate<TItem> predicate)
            where TResultCollection : ICollection<TItem>, new()
        {
            TResultCollection result = new TResultCollection();
            foreach (TItem item in target)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// Selects all items in the target collection that match the specified predicate.
        /// </summary>
        /// <typeparam name="TItem">The type of items in the target collection</typeparam>
        /// <param name="target">The collection to operate on</param>
        /// <param name="predicate">The predicate to test</param>
        /// <returns>A collection containing the subset of matching items from the target collection</returns>
        public static ICollection<TItem> Select<TItem>(IEnumerable target, Predicate<TItem> predicate)
        {
            List<TItem> result = new List<TItem>();
            foreach (TItem item in target)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// Selects all items in the target collection that match the specified predicate. This overload
        /// accepts an untyped collection, and returns an untyped collection.
        /// </summary>
        /// <param name="target">The collection to operate on</param>
        /// <param name="predicate">The predicate to test</param>
        /// <returns>A collection containing the subset of matching items from the target collection</returns>
        public static ICollection Select(IEnumerable target, Predicate<object> predicate)
        {
            ArrayList result = new ArrayList();
            foreach (object item in target)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// Excludes all items in the target collection that match the specified predicate, returning
        /// the rest of the items as a new collection of the specified type.
        /// </summary>
        /// <typeparam name="TItem">The type of items in the target collection</typeparam>
        /// <typeparam name="TResultCollection">The type of collection to return</typeparam>
        /// <param name="target">The collection to operate on</param>
        /// <param name="predicate">The predicate to test</param>
        /// <returns>A collection containing the subset of matching items from the target collection</returns>
        public static TResultCollection Reject<TItem, TResultCollection>(IEnumerable<TItem> target, Predicate<TItem> predicate)
            where TResultCollection : ICollection<TItem>, new()
        {
            return Select<TItem, TResultCollection>(target, delegate(TItem item) { return !predicate(item); });
        }

        /// <summary>
        /// Excludes all items in the target collection that match the specified predicate, returning
        /// the rest of the items as a new collection.
        /// </summary>
        /// <typeparam name="TItem">The type of items in the target collection</typeparam>
        /// <param name="target">The collection to operate on</param>
        /// <param name="predicate">The predicate to test</param>
        /// <returns>A collection containing the subset of matching items from the target collection</returns>
        public static ICollection<TItem> Reject<TItem>(IEnumerable target, Predicate<TItem> predicate)
        {
            return Select<TItem>(target, delegate(TItem item) { return !predicate(item); });
        }

        /// <summary>
        /// Excludes all items in the target collection that match the specified predicate, returning
        /// the rest of the items as a new collection.  This overload accepts an untyped collection,
        /// and returns an untyped collection.
        /// </summary>
        /// <param name="target">The collection to operate on</param>
        /// <param name="predicate">The predicate to test</param>
        /// <returns>A collection containing the subset of matching items from the target collection</returns>
        public static ICollection Reject(IEnumerable target, Predicate<object> predicate)
        {
            return Select(target, delegate(object item) { return !predicate(item); });
        }


        /// <summary>
        /// Returns the first item in the target collection that matches the specified predicate, or
        /// the default of TItem if no match is found.
        /// </summary>
        /// <typeparam name="TItem">The type of items in the target collection</typeparam>
        /// <param name="target">The collection to operate on</param>
        /// <param name="predicate">The predicate to test</param>
        /// <returns>The first matching item, or default(TItem) if no matches are found</returns>
        public static TItem SelectFirst<TItem>(IEnumerable<TItem> target, Predicate<TItem> predicate)
        {
            foreach (TItem item in target)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            return default(TItem);
        }

        /// <summary>
        /// Returns the first item in the target collection that matches the specified predicate, or
        /// the default of TItem if no match is found.
        /// </summary>
        /// <typeparam name="TItem">The type of items in the target collection</typeparam>
        /// <param name="target">The collection to operate on</param>
        /// <param name="predicate">The predicate to test</param>
        /// <returns>The first matching item, or default(TItem) if no matches are found</returns>
        public static TItem SelectFirst<TItem>(IEnumerable target, Predicate<TItem> predicate)
        {
            foreach (TItem item in target)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            return default(TItem);
        }

        /// <summary>
        /// Returns the first item in the target collection that matches the specified predicate, or
        /// null if no match is found.  This overload accepts an untyped collection.
        /// </summary>
        /// <param name="target">The collection to operate on</param>
        /// <param name="predicate">The predicate to test</param>
        /// <returns>The first matching item, or null if no matches are found</returns>
        public static object SelectFirst(IEnumerable target, Predicate<object> predicate)
        {
            foreach (object item in target)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Maps the specified collection onto a new collection according to the specified map function.
        /// Allows the type of the return collection to be specified.
        /// </summary>
        /// <typeparam name="TItem">The type of items in the target collection</typeparam>
        /// <typeparam name="TResultItem">The type of item returned by the map function</typeparam>
        /// <typeparam name="TResultCollection">The type of collection to return</typeparam>
        /// <param name="target">The collection to operate on</param>
        /// <param name="mapFunction">A delegate that performs the mapping</param>
        /// <returns>A new collection of the specified type, containing a mapped entry for each entry in the target collection</returns>
        public static TResultCollection Map<TItem, TResultItem, TResultCollection>(IEnumerable<TItem> target, Converter<TItem, TResultItem> mapFunction)
            where TResultCollection : ICollection<TResultItem>, new()
        {
            TResultCollection result = new TResultCollection();
            foreach (TItem item in target)
            {
                result.Add(mapFunction(item));
            }
            return result;
        }

        /// <summary>
        /// Maps the specified collection onto a new collection according to the specified map function.
        /// </summary>
        /// <typeparam name="TItem">The type of items in the target collection</typeparam>
        /// <typeparam name="TResultItem">The type of item returned by the map function</typeparam>
        /// <param name="target">The collection to operate on</param>
        /// <param name="mapFunction">A delegate that performs the mapping</param>
        /// <returns>A new collection containing a mapped entry for each entry in the target collection</returns>
        public static ICollection<TResultItem> Map<TItem, TResultItem>(IEnumerable target, Converter<TItem, TResultItem> mapFunction)
        {
            ICollection<TResultItem> result = new List<TResultItem>();
            foreach (TItem item in target)
            {
                result.Add(mapFunction(item));
            }
            return result;
        }

        /// <summary>
        /// Maps the specified collection onto a new collection according to the specified map function.
        /// This overload operates on an untyped collection and returns an untyped collection.
        /// </summary>
        /// <param name="target">The collection to operate on</param>
        /// <param name="mapFunction">A delegate that performs the mapping</param>
        /// <returns>A new collection containing a mapped entry for each entry in the target collection</returns>
        public static ICollection Map(IEnumerable target, Converter<object, object> mapFunction)
        {
            ArrayList result = new ArrayList();
            foreach (object item in target)
            {
                result.Add(mapFunction(item));
            }
            return result;
        }

        /// <summary>
        /// Performs the specified action for each item in the target collection.
        /// </summary>
        /// <typeparam name="TItem">The type of items in the target collection</typeparam>
        /// <param name="target">The collection to operate on</param>
        /// <param name="action">The action to perform</param>
        public static void ForEach<TItem>(IEnumerable target, Action<TItem> action)
        {
            foreach (TItem item in target)
            {
                action(item);
            }
        }

        /// <summary>
        /// Performs the specified action for each item in the target collection.
        /// This overload operates on an untyped collection.
        /// </summary>
        /// <param name="target">The collection to operate on</param>
        /// <param name="action">The action to perform</param>
        public static void ForEach(IEnumerable target, Action<object> action)
        {
            foreach (object item in target)
            {
                action(item);
            }
        }

        /// <summary>
        /// Returns true if any item in the target collection satisfies the specified predicate.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="target"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Contains<TItem>(IEnumerable<TItem> target, Predicate<TItem> predicate)
        {
            foreach (TItem item in target)
            {
                if (predicate(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if any item in the target collection satisfies the specified predicate.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="target"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Contains<TItem>(IEnumerable target, Predicate<TItem> predicate)
        {
            foreach (TItem item in target)
            {
                if (predicate(item))
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Returns true if any item in the target collection satisfies the specified predicate.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Contains(IEnumerable target, Predicate<object> predicate)
        {
            foreach (object item in target)
            {
                if (predicate(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if all items in the target collection satisfy the specified predicate.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool TrueForAll<TItem>(IEnumerable<TItem> target, Predicate<TItem> predicate)
        {
            foreach (TItem item in target)
            {
                if (!predicate(item))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Returns true if all items in the target collection satisfy the specified predicate.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool TrueForAll<TItem>(IEnumerable target, Predicate<TItem> predicate)
        {
            foreach (TItem item in target)
            {
                if (!predicate(item))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Returns true if all items in the target collection satisfy the specified predicate.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool TrueForAll(IEnumerable target, Predicate<object> predicate)
        {
            foreach (object item in target)
            {
                if (!predicate(item))
                    return false;
            }
            return true;
        }
    }
}
