using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Extends the IList generic interface with a set of functional-style operations.
    /// </summary>
    /// <typeparam name="T">List element type</typeparam>
    public interface IFunctionalList<T> : IList<T>
    {
        /// <summary>
        /// Returns a new list containing the elements of this list that satisfy the specified predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>A new list</returns>
        IFunctionalList<T> Select(Predicate<T> predicate);

        /// <summary>
        /// Returns a new list that contains only the elements of this list that do not satisfy the specified predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IFunctionalList<T> Reject(Predicate<T> predicate);

        /// <summary>
        /// Returns the first element in this list that satisfies the specified predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T SelectFirst(Predicate<T> predicate);

        /// <summary>
        /// Returns true if this list contains an element that satisfies the specified predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Contains(Predicate<T> predicate);

        /// <summary>
        /// Returns true if all elements in this list satisfy the specified predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool TrueForAll(Predicate<T> predicate);

        /// <summary>
        /// Maps this list into a new list of the specified type, using the specified map function.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="mapFunction"></param>
        /// <returns></returns>
        IFunctionalList<TResult> Map<TResult>(Converter<T, TResult> mapFunction);

        /// <summary>
        /// Does the specified action for each item in the list.
        /// </summary>
        /// <param name="doFunction"></param>
        void ForEach(Action<T> doFunction);
    }
}
