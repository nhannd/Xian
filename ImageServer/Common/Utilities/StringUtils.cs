#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
        public static bool AreEqual(string x, string y, StringComparison options)
        {
            if (String.IsNullOrEmpty(x))
            {
                return String.IsNullOrEmpty(y);   
            }
            else
                return x.Equals(y, options);
        }

        /// <summary>
        /// Returns the last part of the string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <param name="prepend"></param>
        /// <returns></returns>
        public static String Last(String value, int length, String prepend)
        {
            if (String.IsNullOrEmpty(value))
                return value;

            if (value.Length > length)
            {
                String last = value.Substring(value.Length - length);
                if (String.IsNullOrEmpty(prepend))
                    return last;
                else
                    return prepend + last;
            }
            else
                return value;
                
        }
    }
}
