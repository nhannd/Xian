#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Holds arguments passed to a worklist item broker to perform a search.
	/// </summary>
	public class WorklistItemSearchArgs
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="procedureStepClasses"></param>
		/// <param name="searchCriteria"></param>
		/// <param name="projection"></param>
		/// <param name="includeDegeneratePatientItems"></param>
		/// <param name="includeDegenerateProcedureItems"></param>
		/// <param name="threshold"></param>
		public WorklistItemSearchArgs(
			Type[] procedureStepClasses,
			WorklistItemSearchCriteria[] searchCriteria,
			WorklistItemProjection projection,
			bool includeDegeneratePatientItems,
			bool includeDegenerateProcedureItems,
			int threshold)
		{
			ProcedureStepClasses = procedureStepClasses;
			IncludeDegeneratePatientItems = includeDegeneratePatientItems;
			IncludeDegenerateProcedureItems = includeDegenerateProcedureItems;
			Threshold = threshold;
			SearchCriteria = searchCriteria;
			Projection = projection;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="procedureStepClasses"></param>
		/// <param name="searchCriteria"></param>
		/// <param name="projection"></param>
		/// <param name="includeDegeneratePatientItems"></param>
		/// <param name="includeDegenerateProcedureItems"></param>
		public WorklistItemSearchArgs(
			Type[] procedureStepClasses,
			WorklistItemSearchCriteria[] searchCriteria,
			WorklistItemProjection projection,
			bool includeDegeneratePatientItems,
			bool includeDegenerateProcedureItems)
			: this(procedureStepClasses, searchCriteria, projection, includeDegeneratePatientItems, includeDegenerateProcedureItems, 0)
		{
		}

		/// <summary>
		/// Gets a value indicating whether to include results for patients that meet the criteria but do not have any procedures.
		/// </summary>
		public bool IncludeDegeneratePatientItems { get; private set; }

		/// <summary>
		/// Gets a value indicating whether to include results for procedures that meet the criteria but do not have an active procedure step.
		/// </summary>
		public bool IncludeDegenerateProcedureItems { get; private set; }

		/// <summary>
		/// Gets the maximum number of items that the search may return.
		/// </summary>
		public int Threshold { get; private set; }

		/// <summary>
		/// Gets the procedure step classes that are considered in the search.
		/// </summary>
		public Type[] ProcedureStepClasses { get; private set; }

		/// <summary>
		/// Gets the search criteria.
		/// </summary>
		public WorklistItemSearchCriteria[] SearchCriteria { get; private set; }

		/// <summary>
		/// Gets the projection.
		/// </summary>
		public WorklistItemProjection Projection { get; private set; }
	}
}
