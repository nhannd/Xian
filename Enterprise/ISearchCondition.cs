using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Defines the public inteface for a search condition.  Note that not all methods
    /// will make sense for every possible type T.  All methods
    /// will throw an exception if a null argument is passed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISearchCondition<T>
    {
        /// <summary>
        /// Specifies that the condition variable be equal to the specified value.
        /// </summary>
        /// <param name="val"></param>
        void EqualTo(T val);

        /// <summary>
        /// Specifies that the condition variable not be equal to the specified value.
        /// </summary>
        /// <param name="val"></param>
        void NotEqualTo(T val);

        /// <summary>
        /// Specifies that the condition variable be "like" the specified value.
        /// Note that this test is only valid when T is a string.  The specified value must
        /// contain at least one % character to act as a wildcard.
        /// </summary>
        /// <param name="val"></param>
        void Like(T val);

        /// <summary>
        /// Specifies that the condition variable be "not like" the specified value.
        /// Note that this test is only valid when T is a string.  The specified value must
        /// contain at least one % character to act as a wildcard.
        /// </summary>
        /// <param name="val"></param>
        void NotLike(T val);

        /// <summary>
        /// Specifies that the condition variable be between the specified values.
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        void Between(T lower, T upper);

        /// <summary>
        /// Specifies that the condition variable be contained in the specified set of values.
        /// </summary>
        /// <param name="values"></param>
        void In(T[] values);

        /// <summary>
        /// Specifies that the condition variable be less than the specified value.
        /// </summary>
        /// <param name="val"></param>
        void LessThan(T val);

        /// <summary>
        /// Specifies that the condition variable be less than or equal to the specified value.
        /// </summary>
        /// <param name="val"></param>
        void LessThanOrEqualTo(T val);

        /// <summary>
        /// Specifies that the condition variable be more than the specified value.
        /// </summary>
        /// <param name="val"></param>
        void MoreThan(T val);

        /// <summary>
        /// Specifies that the condition variable be more than or equal to the specified value.
        /// </summary>
        /// <param name="val"></param>
        void MoreThanOrEqualTo(T val);

        /// <summary>
        /// Specifies that the condition variable be null, assuming T is a type that supports the notion of null.
        /// </summary>
        void IsNull();

        /// <summary>
        /// Specifies that the condition variable be non-null, assuming T is a type that supports the notion of null.
        /// </summary>
        void IsNotNull();

        /// <summary>
        /// Specifies that the condition variable be used to sort the results in ascending order.
        /// </summary>
        /// <param name="position">Specifies the priority of this sort constraint relative to other sort constraints</param>
        void SortAsc(int position);

        /// <summary>
        /// Specifies that the condition variable be used to sort the results in descending order.
        /// </summary>
        /// <param name="position">Specifies the priority of this sort constraint relative to other sort constraints</param>
        void SortDesc(int position);
    }
}
