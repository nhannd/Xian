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
using System.Security.Cryptography;
using System.Text;
using ClearCanvas.Common.Utilities;
using NHibernate.Mapping;

namespace ClearCanvas.Enterprise.Hibernate.Ddl
{
	/// <summary>
	/// Defines an element in a relational database model.
	/// </summary>
    public abstract class ElementInfo
	{
		#region Utilities

		/// <summary>
		/// Creates a unique name for the element by combining the prefix, table, and element strings
		/// along with a generated hash that is a function of the table and element.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="table"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		protected static string MakeName(string prefix, string table, string element)
		{
			// use MD5 to obtain a 32-character hex string that is a unique function of the table and element
			var bytes = Encoding.UTF8.GetBytes(table + element);
			var hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
			var code = BitConverter.ToString(hash).Replace("-", "");

			// return a value that is at most 64 chars long
			return prefix + Truncate(element, 64 - prefix.Length - code.Length) + code;
		}

		/// <summary>
		/// Creates a unique name for the element by combining the prefix, table, and element strings
		/// along with a generated hash that is a function of the table and element.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="table"></param>
		/// <param name="columns"></param>
		/// <returns></returns>
		protected static string MakeName(string prefix, string table, IEnumerable<Column> columns)
		{
			// concat column names into single string
			var columnNames = StringUtilities.Combine(columns, "", c => c.Name);

			return MakeName(prefix, table, columnNames);
		}

		/// <summary>
		/// Creates a unique name for the element by combining the prefix, table, and element strings
		/// along with a generated hash that is a function of the table and element.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="table"></param>
		/// <param name="columns"></param>
		/// <returns></returns>
		protected static string MakeName(string prefix, string table, IEnumerable<ColumnInfo> columns)
		{
			// concat column names into single string
			var columnNames = StringUtilities.Combine(columns, "", c => c.Name);

			return MakeName(prefix, table, columnNames);
		}

		#endregion


		/// <summary>
		/// Gets the unique identity of the element.
		/// </summary>
		/// <remarks>
		/// The identity string must uniquely identify the element within a given set of elements, but need not be globally unique.
		/// </remarks>
        public abstract string Identity { get; }

		/// <summary>
		/// Compares elements by identity.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
        public override bool Equals(object obj)
        {
            var that = obj as ElementInfo;
            if (that == null)
                return false;
            return this.GetType() == that.GetType() && this.Identity == that.Identity;
        }

		/// <summary>
		/// Gets a hash code based on element identity.
		/// </summary>
		/// <returns></returns>
        public override int GetHashCode()
        {
            return this.Identity.GetHashCode();
        }

		private static string Truncate(string input, int len)
		{
			return (len >= input.Length) ? input : input.Substring(0, len);
		}
	}
}
