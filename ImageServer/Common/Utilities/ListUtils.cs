using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Common.Utilities
{
    /// <summary>
    /// Utility class for working with <see cref="List{T}"/> and <see cref="IList{T}"/>
    /// </summary>
    public static class ListUtils
    {
        /// <summary>
        /// Converts an <see cref="IList{T}"/> into <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Convert<T>(IList<T> list)
        {
            if (list == null)
                return null;

            return new List<T>(list);
        }
    }
}
