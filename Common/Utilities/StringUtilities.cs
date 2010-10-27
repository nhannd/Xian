#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// A static string helper class.
	/// </summary>
	public static class StringUtilities
	{
		/// <summary>
		/// A delegate used by <see cref="StringUtilities"/> to format output strings.
		/// </summary>
		public delegate string FormatDelegate<T>(T value);

		/// <summary>
		/// Combines the input <paramref name="values"/> into a string, separated by <paramref name="separator"/>,
		/// using the given <paramref name="formatSpecifier"/> to format each entry in the string.
		/// </summary>
		/// <remarks>
		/// <typeparam name="T">Must implement <see cref="IFormattable"/>.</typeparam>
		/// </remarks>
		public static string Combine<T>(IEnumerable<T> values, string separator, string formatSpecifier)
			where T : IFormattable
		{
			return Combine(values, separator, formatSpecifier, null);
		}

		/// <summary>
		/// Combines the input <paramref name="values"/> into a string, separated by <paramref name="separator"/>,
		/// using the given <paramref name="formatSpecifier"/> to format each entry in the string.
		/// </summary>
		/// <remarks>
		/// <typeparam name="T">Must implement <see cref="IFormattable"/>.</typeparam>
		/// </remarks>
		public static string Combine<T>(IEnumerable<T> values, string separator, string formatSpecifier, IFormatProvider formatProvider)
			where T : IFormattable
		{
			return Combine(values, separator,
				delegate(T value)
				{
					if (String.IsNullOrEmpty(formatSpecifier))
						return value.ToString();

					return value.ToString(formatSpecifier, formatProvider);
				});
		}

		/// <summary>
		/// Combines the input <paramref name="values"/> into a string separated by the <paramref name="separator"/>.
		/// </summary>
		/// <remarks>
		/// Empty values are skipped.
		/// </remarks>
		public static string Combine<T>(IEnumerable<T> values, string separator)
		{
			return Combine(values, separator, true);
		}

		/// <summary>
		/// Combines the input <paramref name="values"/> into a string separated by the <paramref name="separator"/>;
		/// empty values are skipped when <paramref name="skipEmptyValues"/> is true.
		/// </summary>
		public static string Combine<T>(IEnumerable<T> values, string separator, bool skipEmptyValues)
		{
			return Combine(values, separator, null, skipEmptyValues);
		}

		/// <summary>
		/// Combines the input <paramref name="values"/> into a string separated by the <paramref name="separator"/> 
		/// and formatted using <paramref name="formatDelegate"/>.
		/// </summary>
		public static string Combine<T>(IEnumerable<T> values, string separator, FormatDelegate<T> formatDelegate)
		{
			return Combine(values, separator, formatDelegate, true);
		}

		/// <summary>
		/// Combines the input <paramref name="values"/> into a string separated by <paramref name="separator"/> 
		/// and formatted using <paramref name="formatDelegate"/>; empty values are skipped when <paramref name="skipEmptyValues"/> is true.
		/// </summary>
		public static string Combine<T>(IEnumerable<T> values, string separator, FormatDelegate<T> formatDelegate, bool skipEmptyValues)
		{
			if (values == null)
				return "";

			if (separator == null)
				separator = "";

			StringBuilder builder = new StringBuilder();
			int count = 0;
			foreach (T value in values)
			{
				string stringValue;
				if (formatDelegate == null)
					stringValue = (value == null) ? "" : value.ToString();
				else
					stringValue = formatDelegate(value) ?? "";

				if (String.IsNullOrEmpty(stringValue) && skipEmptyValues)
					continue;

				if (count++ > 0)
					builder.Append(separator);

				builder.Append(stringValue);
			}

			return builder.ToString();
		}

        /// <summary>
        /// Splits any string into sub-strings using the specified <paramref name="delimiters"/>, 
        /// ignoring delimiters inside double quotes.
        /// </summary>
        /// <remarks>
		/// This is different from the <b>String.Split</b> methods 
		/// as we ignore delimiters inside double quotes.
		/// </remarks>
        /// <param name="text">The string to split.</param>
        /// <param name="delimiters">The characters to split on.</param>
        /// <returns></returns>
        public static string[] SplitQuoted(string text, string delimiters)
        {
            ArrayList res = new ArrayList();

            StringBuilder tokenBuilder = new StringBuilder();
            bool insideQuote = false;

            foreach (char c in text.ToCharArray())
            {
                if (!insideQuote && delimiters.Contains(c.ToString()))
                {
                    res.Add(tokenBuilder.ToString());
                    tokenBuilder.Length = 0;
                }
                else if (c.Equals('\"'))
                {
                    insideQuote = !insideQuote;
                }
                else
                {
                    tokenBuilder.Append(c);
                }
            }

            // add the last token
            res.Add(tokenBuilder.ToString());

            return (string[])res.ToArray(typeof(string)); ;
        }

        /// <summary>
        /// Converts an empty string to a null string, otherwise returns the argument unchanged.
        /// </summary>
        public static string NullIfEmpty(string s)
        {
            return string.IsNullOrEmpty(s) ? null : s;
        }

        /// <summary>
        /// Converts a null argument to an empty string, otherwise returns the argument unchanged.
        /// </summary>
        public static string EmptyIfNull(string s)
        {
            return s ?? "";
        }
    }
}
