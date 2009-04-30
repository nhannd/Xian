#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Brokers
{
	public interface IWorklistBroker : IEntityBroker<Worklist, WorklistSearchCriteria>
	{
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
