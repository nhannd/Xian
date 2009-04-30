#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Abstract base class for all search criteria classes.
    /// </summary>
    public abstract class SearchCriteria : ICloneable
    {
        private readonly string _key;
        private readonly Dictionary<string, SearchCriteria> _subCriteria;

		#region Constructors

		/// <summary>
		/// Constructs a search-criteria with a key.
		/// </summary>
		/// <param name="key"></param>
		protected SearchCriteria(string key)
		{
			_key = key;
			_subCriteria = new Dictionary<string, SearchCriteria>();
		}

		/// <summary>
		/// Constructs a top-level search criteria.
		/// </summary>
		protected SearchCriteria()
			: this((string)null)
		{
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="other"></param>
		protected SearchCriteria(SearchCriteria other)
		{
			_key = other._key;
			_subCriteria = new Dictionary<string, SearchCriteria>();

			foreach (KeyValuePair<string, SearchCriteria> kvp in other._subCriteria)
			{
				_subCriteria.Add(kvp.Key, (SearchCriteria)kvp.Value.Clone());
			}
		}

		#endregion

		#region Public API

		/// <summary>
		/// Gets the sub-criteria collection.
		/// </summary>
		public IDictionary<string, SearchCriteria> SubCriteria
		{
			get { return _subCriteria; }
		}

		/// <summary>
		/// Gets a value indicating if this criteria instance is empty, that is,
		/// it does not specify any conditions.
		/// </summary>
		public virtual bool IsEmpty
		{
			get
			{
				foreach (SearchCriteria criteria in _subCriteria.Values)
				{
					if (!criteria.IsEmpty)
						return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Gets a text dump of this criteria object that is useful for debugging purposes.
		/// </summary>
		/// <remarks>
		/// This property is provided because the criteria objects are particularly difficult to work
		/// with in the debugger when there are multiple levels of nested criteria.
		/// </remarks>
		public string[] __Dump
		{
			get
			{
				return Dump(GetKey());
			}
		}

		/// <summary>
        /// Gets the key, or null if this is a top-level criteria.
        /// </summary>
		/// <remarks>
		/// This is intentionally implemented as a method, rather than a property, so that there is no chance that a sub-class
		/// property will conflict.
        /// </remarks>
        /// <returns></returns>
        public string GetKey()
        {
            return _key;
        }

		/// <summary>
		/// Creates a new object that is a copy of the current instance, including only the sub-criteria
		/// that are included by the specified filter.  The filter is applied recursively to sub-criteria.
		/// </summary>
		/// <param name="subCriteriaFilter"></param>
		/// <returns></returns>
		public SearchCriteria Clone(Predicate<SearchCriteria> subCriteriaFilter)
		{
			// this implementation is not particularly efficient, but it was the simplest 
			// way to do it given the default Clone() overload
			// we clone the entire criteria object, then recursively remove any sub-criteria
			// that don't satisfy the filter
			SearchCriteria copy = (SearchCriteria) this.Clone();
			FilterSubCriteria(subCriteriaFilter);
			return copy;
		}

		///<summary>
    	///Creates a new object that is a copy of the current instance.
    	///</summary>
    	///
    	///<returns>
    	///A new object that is a copy of this instance.
    	///</returns>
    	///<filterpriority>2</filterpriority>
    	public abstract object Clone();

        #endregion

		#region Protected API

		/// <summary>
		/// Gets an array of strings where each string is a text representation of a search condition
		/// that is defined by this criteria instance.
		/// </summary>
		/// <param name="prefix"></param>
		/// <returns></returns>
		protected virtual string[] Dump(string prefix)
		{
			List<string> lines = new List<string>();
			foreach (KeyValuePair<string, SearchCriteria> pair in _subCriteria)
			{
				if (!pair.Value.IsEmpty)
				{
					string p = prefix == null ? "" : prefix + ".";
					lines.AddRange(pair.Value.Dump(p + pair.Key));
				}
			}
			return lines.ToArray();
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Recursively removes sub-criteria from this instance that do not satisfy the filter condition.
		/// </summary>
		/// <param name="subCriteriaFilter"></param>
		private void FilterSubCriteria(Predicate<SearchCriteria> subCriteriaFilter)
		{
			List<string> keys = new List<string>(_subCriteria.Keys);
			foreach (string key in keys)
			{
				SearchCriteria subCriteria = _subCriteria[key];
				if (!subCriteriaFilter(subCriteria))
				{
					// remove sub-criteria
					_subCriteria.Remove(key);
				}
				else
				{
					// retain immediate sub-criteria, but apply filter recursively
					subCriteria.FilterSubCriteria(subCriteriaFilter);
				}
			}
		}

		#endregion
	}
}
