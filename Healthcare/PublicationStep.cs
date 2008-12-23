#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{


	/// <summary>
	/// PublicationStep entity
	/// </summary>
	public class PublicationStep : ReportingProcedureStep
	{
		private int _failureCount;
		private DateTime? _lastFailureTime;

		public PublicationStep()
		{
		}

		public PublicationStep(ReportingProcedureStep previousStep)
			: base(previousStep)
		{
			_failureCount = 0;
			_lastFailureTime = null;
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		public override string Name
		{
			get { return "Publication"; }
		}

		public virtual int FailureCount
		{
			get { return _failureCount; }
		}

		public virtual DateTime? LastFailureTime
		{
			get { return _lastFailureTime; }
		}

		public virtual void Fail()
		{
			_failureCount++;
			_lastFailureTime = Platform.Time;
		}

		protected override void OnStateChanged(ActivityStatus previousState, ActivityStatus newState)
		{
			// complete the report part when publication is complete
			if (newState == ActivityStatus.CM)
			{
				this.ReportPart.Complete();

				// if step corresponds to the initial report (not an addendum), mark procedure(s) as
				// being complete
				if (this.ReportPart.Index == 0)
				{
					foreach (Procedure procedure in this.AllProcedures)
					{
						procedure.Complete((DateTime) this.EndTime);
					}
				}
			}

			base.OnStateChanged(previousState, newState);
		}

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new PublicationStep(this);
		}
	}
}