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
	}

	public class StartModalityProcedureStepsOperation : ModalityOperation
	{
		public ModalityPerformedProcedureStep Execute(IList<ModalityProcedureStep> modalitySteps, DateTime? startTime, Staff technologist, IWorkflow workflow, IPersistenceContext context)
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
			ModalityPerformedProcedureStep mpps = new ModalityPerformedProcedureStep();
			context.Lock(mpps, DirtyState.New);

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
					context.Lock(docStep, DirtyState.New);
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

	public class CompleteModalityPerformedProcedureStepOperation : ModalityOperation
	{
		public void Execute(ModalityPerformedProcedureStep mpps, DateTime? completedTime, IWorkflow workflow)
		{
			// complete mpps
			mpps.Complete(completedTime);

			ModalityProcedureStep oneMps = CollectionUtils.FirstElement<ModalityProcedureStep>(mpps.Activities);
			Order order = oneMps.Procedure.Order;

			// try to complete any mps that have all mpps completed
			foreach (Procedure rp in order.Procedures)
			{
				foreach (ModalityProcedureStep mps in rp.ModalityProcedureSteps)
				{
					bool allPerformedStepsDone =
						!mps.PerformedSteps.IsEmpty
						&& CollectionUtils.TrueForAll<PerformedProcedureStep>(mps.PerformedSteps,
							delegate(PerformedProcedureStep pps) { return pps.IsTerminated; });

					if (!mps.IsTerminated && allPerformedStepsDone)
					{
						mps.Complete(completedTime);
					}

					TryAutoCheckOut(mps.Procedure, completedTime);
				}
			}
		}
	}

	public class DiscontinueModalityPerformedProcedureStepOperation : ModalityOperation
	{
		public void Execute(ModalityPerformedProcedureStep mpps, DateTime? discontinuedTime, IWorkflow workflow)
		{
			mpps.Discontinue(discontinuedTime);

			foreach (ModalityProcedureStep mps in mpps.Activities)
			{
				// Any MPS can have multiple MPPS's, so discontinue the MPS only if all MPPS's are discontinued
				if (mps.PerformedSteps.Count > 1)
				{
					bool allMppsDiscontinued = CollectionUtils.TrueForAll<PerformedProcedureStep>(mps.PerformedSteps,
							delegate(PerformedProcedureStep pps) { return pps.State == PerformedStepStatus.DC; });

					if (allMppsDiscontinued)
					{
						mps.Discontinue(discontinuedTime);
					}
				}

				TryAutoCheckOut(mps.Procedure, discontinuedTime);
			}
		}
	}

}
