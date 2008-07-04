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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Modality
{
	public abstract class ModalityOperation
	{
		protected void UpdateCheckInStep(Procedure rp, bool procedureAborted, IWorkflow workflow)
		{
			// Note: auto check-in is disabled
			//AutoCheckIn(rp);

			// Note: auto check-out behaviour may need to be disabled in future - ideally it should be done explicitly by the user
			AutoCheckOut(rp);
		}

		private static void AutoCheckIn(Procedure rp)
		{
			bool allMpsScheduled = rp.ModalityProcedureSteps.TrueForAll(
				delegate(ModalityProcedureStep mps) { return mps.State == ActivityStatus.SC; });

			if (!allMpsScheduled)
			{
				// check-in this procedure, since some mps has started
				rp.ProcedureCheckIn.CheckIn();
			}
		}

		private static void AutoCheckOut(Procedure rp)
		{
			bool allMpsTerminated = rp.ModalityProcedureSteps.TrueForAll(
				delegate(ModalityProcedureStep mps) { return mps.IsTerminated; });

			if (allMpsTerminated)
			{
				// auto check-out
				rp.ProcedureCheckIn.CheckOut();
			}
		}
	}

	public class StartModalityProcedureStepsOperation : ModalityOperation
	{
		public ModalityPerformedProcedureStep Execute(IList<ModalityProcedureStep> modalitySteps, Staff technologist, IWorkflow workflow, IPersistenceContext context)
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
				mps.Start(technologist);
				mps.AddPerformedStep(mpps);

				UpdateCheckInStep(mps.Procedure, false, workflow);
			}

			// Create Documentation Step for each RP that has an MPS started by this service call
			foreach (ModalityProcedureStep step in modalitySteps)
			{
				if (step.Procedure.DocumentationProcedureStep == null)
				{
					ProcedureStep docStep = new DocumentationProcedureStep(step.Procedure);
					docStep.Start(technologist);
					context.Lock(docStep, DirtyState.New);
				}
			}

			return mpps;
		}
	}

	public class DiscontinueModalityProcedureStepOperation : ModalityOperation
	{
		public void Execute(ModalityProcedureStep mps, bool procedureAborted, IWorkflow workflow)
		{
			mps.Discontinue();
			UpdateCheckInStep(mps.Procedure, procedureAborted, workflow);
		}
	}

	public class CompleteModalityPerformedProcedureStepOperation : ModalityOperation
	{
		public void Execute(ModalityPerformedProcedureStep mpps, IWorkflow workflow)
		{
			// complete mpps
			mpps.Complete();

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
						mps.Complete();

					UpdateCheckInStep(mps.Procedure, false, workflow);
				}
			}
		}
	}

	public class DiscontinueModalityPerformedProcedureStepOperation : ModalityOperation
	{
		public void Execute(ModalityPerformedProcedureStep mpps, IWorkflow workflow)
		{
			mpps.Discontinue();

			foreach (ModalityProcedureStep mps in mpps.Activities)
			{
				// Any MPS can have multiple MPPS's, so discontinue the MPS only if all MPPS's are discontinued
				if (mps.PerformedSteps.Count > 1)
				{
					bool allMppsDiscontinued = CollectionUtils.TrueForAll<PerformedProcedureStep>(mps.PerformedSteps,
							delegate(PerformedProcedureStep pps) { return pps.State == PerformedStepStatus.DC; });

					if (allMppsDiscontinued)
						mps.Discontinue();
				}
			}
		}
	}

}
