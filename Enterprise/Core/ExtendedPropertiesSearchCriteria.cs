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
