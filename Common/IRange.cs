using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Range object interface, defines a range of objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRange<T>
    {
        /// <summary>
        /// Extend the range by this amount
        /// </summary>
        /// <param name="amount"></param>
        void Extend(T amount);

        /// <summary>
        /// Shrink the range by this amount
        /// </summary>
        /// <param name="amount"></param>
        void Shrink(T amount);

        /// <summary>
        /// Retrieves the beginning of the range
        /// </summary>
        T Start
        {
            get; set;
        }

        /// <summary>
        /// Retrieves the end of the range
        /// </summary>
        T End
        { 
            get; set;
        }
    }
}
