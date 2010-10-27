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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate.Hql
{
	/// <summary>
	/// Provides support for building HQL queries dynamically from <see cref="SearchCriteria"/> objects.
	/// </summary>
	/// <seealso cref="HqlQuery"/>
	public class HqlSort : HqlElement, IComparable<HqlSort>
	{
		/// <summary>
		/// Extracts a list of <see cref="HqlSort"/> objects from the specified <see cref="SearchCriteria"/>
		/// </summary>
		/// <param name="qualifier">The HQL qualifier to prepend to the sort variables</param>
		/// <param name="criteria">The search criteria object</param>
		/// <returns>A list of HQL sort object that are equivalent to the specified criteria</returns>
		public static HqlSort[] FromSearchCriteria(string qualifier, SearchCriteria criteria)
		{
			return FromSearchCriteria(qualifier, criteria, a => a);
		}

		/// <summary>
		/// Extracts a list of <see cref="HqlSort"/> objects from the specified <see cref="SearchCriteria"/>
		/// </summary>
		/// <param name="qualifier">The HQL qualifier to prepend to the sort variables</param>
		/// <param name="criteria">The search criteria object</param>
		/// <param name="remapHqlExprFunc"></param>
		/// <returns>A list of HQL sort object that are equivalent to the specified criteria</returns>
		public static HqlSort[] FromSearchCriteria(string qualifier, SearchCriteria criteria, Converter<string, string> remapHqlExprFunc)
		{
			var hqlSorts = new List<HqlSort>();
			if (criteria is SearchConditionBase)
			{
				var sc = (SearchConditionBase)criteria;
				if (sc.SortPosition > -1)
				{
					hqlSorts.Add(new HqlSort(remapHqlExprFunc(qualifier), sc.SortDirection, sc.SortPosition));
				}
			}
			else
			{
				// recur on subCriteria
				foreach (var subCriteria in criteria.EnumerateSubCriteria())
				{
					var subQualifier = string.Format("{0}.{1}", qualifier, subCriteria.GetKey());
					hqlSorts.AddRange(FromSearchCriteria(subQualifier, subCriteria, remapHqlExprFunc));
				}
			}

			return hqlSorts.ToArray();
		}

		private readonly string _hql;
		private readonly int _position;

		/// <summary>
		/// Constructs an <see cref="HqlSort"/> object.
		/// </summary>
		/// <param name="variable">The HQL variable on which to sort</param>
		/// <param name="ascending">Specifies whether the sort is ascending or descending</param>
		/// <param name="position">Specifies the relative priority of the sort condition</param>
		public HqlSort(string variable, bool ascending, int position)
		{
			_hql = string.Format("{0} {1}", variable, ascending ? "asc" : "desc");
			_position = position;
		}

		/// <summary>
		/// The HQL for this sort.
		/// </summary>
		public override string Hql
		{
			get { return _hql; }
		}

		/// <summary>
		/// The position of this sort in the order by clause.
		/// </summary>
		public int Position
		{
			get { return _position; }
		}

		#region IComparable<HqlSort> Members

		public int CompareTo(HqlSort other)
		{
			return this._position.CompareTo(other._position);
		}

		#endregion
	}
}
