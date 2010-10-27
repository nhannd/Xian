#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
	public class ExtendedPropertiesSearchCriteria : SearchCriteria
	{
		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public ExtendedPropertiesSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public ExtendedPropertiesSearchCriteria(string key)
			:base(key)
		{
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		protected ExtendedPropertiesSearchCriteria(ExtendedPropertiesSearchCriteria other)
			:base(other)
		{
		}

		public ISearchCondition<string> this[string key]
		{
			get
			{
				if (!this.SubCriteria.ContainsKey(key))
				{
					this.SubCriteria[key] = new SearchCondition<string>(key);
				}
				return (ISearchCondition<string>)this.SubCriteria[key];
			}
		}

		public override object Clone()
		{
			return new ExtendedPropertiesSearchCriteria(this);
		}
	}
}
