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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Workflow.Modality
{
	public abstract class ModalityOperation
	{
		/// <summary>
		/// Helper method implements some fuzzy logic to try and determine whether the procedure
		/// should be checked-in, and check it in if necessary.
		/// </summary>
		/// <param name="rp"></param>
		/// <param name="timestamp"></param>
		protected void TryAutoCheckIn(Procedure rp, DateTime? timestamp)
		{
			if(rp.ProcedureCheckIn.IsPreCheckIn)
			{
				bool allMpsScheduled = rp.ModalityProcedureSteps.TrueForAll(
					delegate(ModalityProcedureStep mps) { return mps.State == ActivityStatus.SC; });

				if (!allMpsScheduled)
				{
					// check-in this procedure, since some mps has started
					rp.ProcedureCheckIn.CheckIn(timestamp);
				}
			}
		}

		/// <summary>
		/// Helper method implements some fuzzy logic to try and determine whether the procedure
		/// should be checked-out, and check it out if necessary.
		/// </summary>
		/// <param name="rp"></param>
		/// <param name="timestamp"></param>
		protected void TryAutoCheckOut(Procedure rp, DateTime? timestamp)
		{
			if (rp.ProcedureCheckIn.IsCheckedIn)
			{
				bool allMpsTerminated = rp.ModalityProcedureSteps.TrueForAll(
						delegate(ModalityProcedureStep mps) { return mps.IsTerminated; });

				if (allMpsTerminated)
				{
					// auto check-out
					rp.ProcedureCheckIn.CheckOut(timestamp);
				}
			}
		}

		protected void TryAutoTerminateProcedureSteps(Procedure procedure, DateTime? time, IWorkflow workflow)
		{
			foreach (ModalityProcedureStep mps in procedure.ModalityProcedureSteps)
			{
				// if the MPS is not terminated and has some MPPS
				if(!mps.IsTerminated && !mps.PerformedSteps.IsEmpty)
				{
					bool allMppsDiscontinued = CollectionUtils.TrueForAll<PerformedProcedureStep>(mps.PerformedSteps,
							delegate(PerformedProcedureStep pps) { return pps.State == PerformedStepStatus.DC; });
					bool allMppsTerminated = CollectionUtils.TrueForAll<PerformedProcedureStep>(mps.PerformedSteps,
							delegate(PerformedProcedureStep pps) { return pps.IsTerminated; });

					if (allMppsDiscontinued)
					{
						// discontinue MPS, since all MPPS are discontinued
						mps.Discontinue(time);
					}
					else if (allMppsTerminated)
					{
						// all MPPS are terminated, and at least one MPPS must be completed, so complete MPS
						mps.Complete(time);
					}
				}
			}
		}
	}

	public class StartModalityProcedureStepsOperation : ModalityOperation
	{
		public ModalityPerformedProcedureStep Execute(IList<ModalityProcedureStep> modalitySteps, DateTime? startTime, Staff technologist, IWorkflow workflow)
		{
			if (modalitySteps.Count == 0)
				throw new WorkflowException("At least one procedure step is required.");

			// validate that each mps being started is being performed on the same modality
			if (!CollectionUtils.TrueForAll(modalitySteps,
				delegate(ModalityProcedureStep step) { return step.Modality.Equals(modalitySteps[0].Modality); }))
			{
				throw new WorkflowException("Procedure steps cannot be started together because they are not on the same modality.");
			}

			// create an mpps
			ModalityPerformedProcedureStep mpps = new ModalityPerformedProcedureStep(technologist, startTime);
			workflow.AddEntity(mpps);

			foreach (ModalityProcedureStep mps in modalitySteps)
			{
				mps.Start(technologist, startTime);
				mps.AddPerformedStep(mpps);

				//note: this feature was disabled by request (see #2138) - they want to enforce explicit check-in
				//AutoCheckIn(mps.Procedure, startTime);
			}

			// Create Documentation Step for each RP that has an MPS started by this service call
			foreach (ModalityProcedureStep step in modalitySteps)
			{
				if (step.Procedure.DocumentationProcedureStep == null)
				{
					ProcedureStep docStep = new DocumentationProcedureStep(step.Procedure);
					docStep.Start(technologist, startTime);
					workflow.AddEntity(docStep);
				}
			}

			return mpps;
		}
	}

	public class DiscontinueModalityProcedureStepOperation : ModalityOperation
	{
		public void Execute(ModalityProcedureStep mps, DateTime? discontinueTime, IWorkflow workflow)
		{
			mps.Discontinue(discontinueTime);
			TryAutoCheckOut(mps.Procedure, discontinueTime);
		}
	}

	public abstract class TerminateModalityPerformedProcedureStepOperation : ModalityOperation
	{
		public void Execute(ModalityPerformedProcedureStep mpps, DateTime? time, IWorkflow workflow)
		{
			TerminatePerformedProcedureStep(mpps, time);

			ModalityProcedureStep oneMps = CollectionUtils.FirstElement<ProcedureStep>(mpps.Activities).As<ModalityProcedureStep>();
			Order order = oneMps.Procedure.Order;

			// try to complete any mps that have all mpps completed
			foreach (Procedure rp in order.Procedures)
			{
				if (rp.Status != ProcedureStatus.IP)
					continue;

				TryAutoTerminateProcedureSteps(rp, time, workflow);
				TryAutoCheckOut(rp, time);
			}
		}

		protected abstract void TerminatePerformedProcedureStep(ModalityPerformedProcedureStep mpps, DateTime? time);

	}

	public class CompleteModalityPerformedProcedureStepOperation : TerminateModalityPerformedProcedureStepOperation
	{
		protected override void TerminatePerformedProcedureStep(ModalityPerformedProcedureStep mpps, DateTime? time)
		{
			mpps.Complete(time);
		}
	}

	public class DiscontinueModalityPerformedProcedureStepOperation : TerminateModalityPerformedProcedureStepOperation
	{
		protected override void TerminatePerformedProcedureStep(ModalityPerformedProcedureStep mpps, DateTime? time)
		{
			mpps.Discontinue(time);
		}
	}
}
