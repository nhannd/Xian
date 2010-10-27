#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Healthcare.Hibernate.Brokers.QueryBuilders
{
	/// <summary>
	/// Specialization of <see cref="QueryBuilderArgs"/> for building search queries.
	/// </summary>
	public class SearchQueryArgs : QueryBuilderArgs
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="procedureStepClass"></param>
		/// <param name="criteria"></param>
		/// <param name="projection"></param>
		public SearchQueryArgs(Type procedureStepClass, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection)
			: base(new [] { procedureStepClass }, criteria, projection, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="procedureStepClasses"></param>
		/// <param name="criteria"></param>
		/// <param name="projection"></param>
		public SearchQueryArgs(Type[] procedureStepClasses, WorklistItemSearchCriteria[] criteria, WorklistItemProjection projection)
			: base(procedureStepClasses, criteria, projection, null)
		{
		}
	}
}
