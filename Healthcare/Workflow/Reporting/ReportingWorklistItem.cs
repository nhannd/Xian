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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
	public class ReportingWorklistItem : WorklistItem
	{

		private static readonly Dictionary<WorklistItemField, UpdateWorklistItemDelegate> _fieldUpdaters
			= new Dictionary<WorklistItemField, UpdateWorklistItemDelegate>();

		static ReportingWorklistItem()
		{
			// need to careful about checking for null values here, because a ReportingWorklistItem may not 
			// have any report information (if it represents a scheduled interpretation step)
			_fieldUpdaters.Add(WorklistItemField.Report,
				(item, value) => ((ReportingWorklistItem)item).ReportRef = (EntityRef)value);

			_fieldUpdaters.Add(WorklistItemField.ReportPartIndex,
				(item, value) => ((ReportingWorklistItem)item).ReportPartIndex = value == null ? -1 : (int)value);

			_fieldUpdaters.Add(WorklistItemField.ReportPartHasErrors,
				(item, value) => ((ReportingWorklistItem)item).HasErrors = value == null ? false : (bool)value);

			_fieldUpdaters.Add(WorklistItemField.ReportPartPreliminaryTime,
				(item, value) => item.Time = (DateTime?)value);

			_fieldUpdaters.Add(WorklistItemField.ReportPartCompletedTime,
				(item, value) => item.Time = (DateTime?)value);
		}



		/// <summary>
		/// Default constructor required for dyanmic instantiation.
		/// </summary>
		public ReportingWorklistItem()
		{
			ReportPartIndex = -1;
		}

		/// <summary>
		/// Gets the report ref.
		/// </summary>
		public EntityRef ReportRef { get; internal set; }

		/// <summary>
		/// Gets the report part index, or -1 if there is no report part.
		/// </summary>
		public int ReportPartIndex { get; internal set; }

		/// <summary>
		/// Gets a value indicating if transcription has flagged report for errors.
		/// </summary>
		public bool HasErrors { get; internal set; }

		protected override UpdateWorklistItemDelegate GetFieldUpdater(WorklistItemField field)
		{
			UpdateWorklistItemDelegate updater;
			return _fieldUpdaters.TryGetValue(field, out updater) ? updater : base.GetFieldUpdater(field);
		}
	}
}
