using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Common.Utilities
{
    public static class StringUtils
    {
        /// <summary>
        /// Compares two strings, treat Null and Empty being the same.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool AreEqual(string x, string y)
        {
            if (String.IsNullOrEmpty(x))
            {
                return String.IsNullOrEmpty(y);   
            }
            else
                return x.Equals(y);
        }
    }
}
