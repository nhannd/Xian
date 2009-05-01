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

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Holds arguments passed to a worklist item broker to perform a search.
	/// </summary>
	public class WorklistItemSearchArgs
	{
		private readonly bool _includeDegeneratePatientItems;
		private readonly bool _includeDegenerateProcedureItems;
		private readonly int _threshold;
		private readonly WorklistItemSearchCriteria[] _searchCriteria;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="searchCriteria"></param>
		/// <param name="includeDegeneratePatientItems"></param>
		/// <param name="includeDegenerateProcedureItems"></param>
		/// <param name="threshold"></param>
		public WorklistItemSearchArgs(
			WorklistItemSearchCriteria[] searchCriteria,
			bool includeDegeneratePatientItems,
			bool includeDegenerateProcedureItems,
			int threshold)
		{
			_includeDegeneratePatientItems = includeDegeneratePatientItems;
			_includeDegenerateProcedureItems = includeDegenerateProcedureItems;
			_threshold = threshold;
			_searchCriteria = searchCriteria;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="searchCriteria"></param>
		/// <param name="includeDegeneratePatientItems"></param>
		/// <param name="includeDegenerateProcedureItems"></param>
		public WorklistItemSearchArgs(
			WorklistItemSearchCriteria[] searchCriteria,
			bool includeDegeneratePatientItems,
			bool includeDegenerateProcedureItems)
			:this(searchCriteria, includeDegeneratePatientItems, includeDegenerateProcedureItems, 0)
		{
		}

		/// <summary>
		/// Gets a value indicating whether to include results for patients that meet the criteria but do not have any procedures.
		/// </summary>
		public bool IncludeDegeneratePatientItems
		{
			get { return _includeDegeneratePatientItems; }
		}

		/// <summary>
		/// Gets a value indicating whether to include results for procedures that meet the criteria but do not have an active procedure step.
		/// </summary>
		public bool IncludeDegenerateProcedureItems
		{
			get { return _includeDegenerateProcedureItems; }
		}

		/// <summary>
		/// Gets the maximum number of items that the search may return.
		/// </summary>
		public int Threshold
		{
			get { return _threshold; }
		}

		/// <summary>
		/// Gets the search criteria.
		/// </summary>
		public WorklistItemSearchCriteria[]  SearchCriteria
		{
			get { return _searchCriteria; }
		}
	}
}
