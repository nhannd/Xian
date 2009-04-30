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
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// ReportPart component
	/// </summary>
	public partial class ReportPart
	{
		/// <summary>
		/// Constructor used by <see cref="Report"/> class.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="index"></param>
		internal ReportPart(Report owner, int index)
		{
			_report = owner;
			_index = index;
			_extendedProperties = new Dictionary<string, string>();
			_creationTime = Platform.Time;
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		/// <summary>
		/// Gets a value indicating whether this report part is an addendum.
		/// </summary>
		public virtual bool IsAddendum
		{
			get { return _index > 0; }
		}

		/// <summary>
		/// Gets a value indicating whether this report part is modifiable,
		/// which is true if the status is either <see cref="ReportPartStatus.D"/> or <see cref="ReportPartStatus.P"/>.
		/// </summary>
		public virtual bool IsModifiable
		{
			get { return _status == ReportPartStatus.D || _status == ReportPartStatus.P; }
		}

		/// <summary>
		/// Marks this report part as being preliminary.
		/// </summary>
		public virtual void MarkPreliminary()
		{
			if (_status != ReportPartStatus.D)
				throw new WorkflowException(string.Format("Cannot transition from {0} to P", _status));

			_preliminaryTime = Platform.Time;
			SetStatus(ReportPartStatus.P);
		}

		/// <summary>
		/// Marks this report part as being complete (status Final).
		/// </summary>
		public virtual void Complete()
		{
			if (_status == ReportPartStatus.X || _status == ReportPartStatus.F)
				throw new WorkflowException(string.Format("Cannot transition from {0} to F", _status));

			_completedTime = Platform.Time;
			SetStatus(ReportPartStatus.F);
		}

		/// <summary>
		/// Marks this report part as being cancelled (status Cancelled).
		/// </summary>
		public virtual void Cancel()
		{
			if (_status == ReportPartStatus.X || _status == ReportPartStatus.F)
				throw new WorkflowException(string.Format("Cannot transition from {0} to X", _status));

			_cancelledTime = Platform.Time;
			SetStatus(ReportPartStatus.X);
		}

		/// <summary>
		/// Removes transient properties related to rejected transcriptions.  These properties may no longer be valid if a report part
		/// is re-submitted for transcription.
		/// </summary>
		public virtual void ResetTranscription()
		{
			if (!this.IsModifiable)
				throw new WorkflowException("Cannot change transcription details for a completed report part.");

			_transcriptionRejectReason = null;
			_transcriptionSupervisor = null;
		}

		/// <summary>
		/// Helper method to change the status and also notify the parent report to change its status
		/// if necessary.
		/// </summary>
		/// <param name="status"></param>
		private void SetStatus(ReportPartStatus status)
		{
			_status = status;

			_report.UpdateStatus();
		}

		/// <summary>
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.
		/// </remarks>
		/// <param name="minutes"></param>
		protected internal virtual void TimeShift(int minutes)
		{
			_creationTime = _creationTime.AddMinutes(minutes);
			_preliminaryTime = _preliminaryTime.HasValue ? _preliminaryTime.Value.AddMinutes(minutes) : _preliminaryTime;
			_completedTime = _completedTime.HasValue ? _completedTime.Value.AddMinutes(minutes) : _completedTime;
			_cancelledTime = _cancelledTime.HasValue ? _cancelledTime.Value.AddMinutes(minutes) : _cancelledTime;
		}
	}
}