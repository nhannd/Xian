#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
	public interface IWorklistBroker : IEntityBroker<Worklist, WorklistSearchCriteria>
	{
		/// <summary>
		/// Count the number of worklists owned by the specified owner.
		/// </summary>
		/// <param name="owner"></param>
		/// <returns></returns>
		int Count(WorklistOwner owner);

		/// <summary>
		/// Finds worklists assigned to specified staff group.
		/// </summary>
		/// <param name="staffGroup"></param>
		/// <returns></returns>
		IList<Worklist> Find(StaffGroup staffGroup);

		/// <summary>
		/// Finds worklists matching specified class names and assigned to specified staff.
		/// </summary>
		/// <param name="staff"></param>
		/// <param name="worklistClassNames"></param>
		/// <returns></returns>
		IList<Worklist> Find(Staff staff, IEnumerable<string> worklistClassNames);

		/// <summary>
		/// Finds worklists matching the specified name (which may contain wildcards) and class names.
		/// </summary>
		/// <param name="name">If empty, no name criteria is applied.</param>
		/// <param name="includeUserDefinedWorklists"></param>
		/// <param name="worklistClassNames"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		IList<Worklist> Find(string name, bool includeUserDefinedWorklists, IEnumerable<string> worklistClassNames, SearchResultPage page);

		/// <summary>
		/// Finds one worklist with the specified name and class name.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="worklistClassName"></param>
		/// <returns></returns>
		Worklist FindOne(string name, string worklistClassName);
	}
}
