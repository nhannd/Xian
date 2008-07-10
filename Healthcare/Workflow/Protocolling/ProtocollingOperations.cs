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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Protocolling
{
	public class ProtocollingOperations
	{
		public abstract class ProtocollingOperation
		{
			public virtual bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
			{
				return false;
			}

			public virtual bool CanExecute(Order order, Staff currentUserStaff)
			{
				return false;
			}

			protected static T InprogressProcedureStep<T>(Procedure rp) where T : ProcedureStep
			{
				return CurrentProcedureStep<T>(rp, ActivityStatus.IP);
			}

			protected static T ScheduledProcedureStep<T>(Procedure rp) where T : ProcedureStep
			{
				return CurrentProcedureStep<T>(rp, ActivityStatus.SC);
			}

			protected static T CurrentProcedureStep<T>(Procedure rp, ActivityStatus status) where T : ProcedureStep
			{
				ProcedureStep uncastProcedureStep = CollectionUtils.SelectFirst<ProcedureStep>(
					rp.ProcedureSteps,
					delegate(ProcedureStep ps) { return ps.Is<T>() && ps.State == status; });

				return uncastProcedureStep != null ? uncastProcedureStep.Downcast<T>() : null;
			}
		}

		public class StartProtocolOperation : ProtocollingOperation
		{
			public void Execute(Order order, Staff protocolPerformer, bool canPerformerAcceptProtocols, out bool protocolClaimed)
			{
				protocolClaimed = false;

				foreach (Procedure rp in order.Procedures)
				{
					ProtocolAssignmentStep assignmentStep = ScheduledProcedureStep<ProtocolAssignmentStep>(rp);

					if (assignmentStep != null)
					{
						// Scheduled assignment step exists (i.e. Protocol has not been claimed), so claim it
						assignmentStep.Start(protocolPerformer);

						// Current user should be set as the supervisor if the protocol is awaiting approval 
						// and the user is able to accept protocols
						if (assignmentStep.Protocol.Status == ProtocolStatus.AA)
						{
							if (canPerformerAcceptProtocols)
							{
								assignmentStep.Protocol.Supervisor = protocolPerformer;
							}
							else
							{
								// User is unable to approve
								//throw new RequestValidationException(SR.ExceptionNoProtocolAssignmentStep);
								throw new Exception("TODO: user unable to approve");
							}
						}
						// otherwise, it's just a new protocol
						else
						{
							assignmentStep.Protocol.Author = protocolPerformer;
						}

						protocolClaimed = true;
					}
					else
					{
						// No scheduled assignment step, so possibly already claimed
						assignmentStep = InprogressProcedureStep<ProtocolAssignmentStep>(rp);

						// In-progress assignment step started by someone else
						if (assignmentStep != null && assignmentStep.PerformingStaff != protocolPerformer)
						{
							// So not available to this user to start
							//throw new RequestValidationException(SR.ExceptionNoProtocolAssignmentStep);
							throw new Exception("TODO: protocol already started");
						}

						// otherwise, it's been claimed by this user, so do nothing
					}
				}
			}

			public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
			{
				return step.State == ActivityStatus.SC;
			}
		}

		public class DiscardProtocolOperation : ProtocollingOperation
		{
			public void Execute(Order order)
			{
				foreach (Procedure rp in order.Procedures)
				{
					// Discontinue claimed/in-progress protocol step
					ProtocolAssignmentStep existingAssignmentStep = InprogressProcedureStep<ProtocolAssignmentStep>(rp);

					if (existingAssignmentStep != null)
					{
						existingAssignmentStep.Discontinue();
						if (existingAssignmentStep.Protocol.Status == ProtocolStatus.AA)
						{
							existingAssignmentStep.Protocol.Supervisor = null;
						}
						else
						{
							existingAssignmentStep.Protocol.Author = null;
						}

						// Replace with new step scheduled step
						ProtocolAssignmentStep replacementAssignmentStep = new ProtocolAssignmentStep(existingAssignmentStep.Protocol);
						rp.AddProcedureStep(replacementAssignmentStep);

						replacementAssignmentStep.Schedule(DateTime.Now);
					}
				}
			}

			public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
			{
				return step.State == ActivityStatus.IP;
			}
		}

		public class AcceptProtocolOperation : ProtocollingOperation
		{
			public void Execute(Order order)
			{
				foreach (Procedure rp in order.Procedures)
				{
					ProtocolAssignmentStep assignmentStep = InprogressProcedureStep<ProtocolAssignmentStep>(rp);

					if (assignmentStep != null)
					{
						assignmentStep.Complete();
						assignmentStep.Protocol.Accept();
					}
				}
			}

			public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
			{
				return step.State == ActivityStatus.IP;
			}
		}

		public class RejectProtocolOperation : ProtocollingOperation
		{
			public void Execute(Order order, ProtocolRejectReasonEnum reason)
			{
				foreach (Procedure rp in order.Procedures)
				{
					ProtocolAssignmentStep assignmentStep = InprogressProcedureStep<ProtocolAssignmentStep>(rp);

					if (assignmentStep != null)
					{
						assignmentStep.Discontinue();
						assignmentStep.Protocol.Reject(reason);

						ProtocolResolutionStep resolutionStep = new ProtocolResolutionStep(assignmentStep.Protocol);
						rp.AddProcedureStep(resolutionStep);
					}
				}
			}

			public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
			{
				return step.State == ActivityStatus.IP;
			}
		}

		public class ResubmitProtocolOperation : ProtocollingOperation
		{
			public void Execute(Order order, Staff resolvingStaff)
			{
				foreach (Procedure rp in order.Procedures)
				{
					ProtocolResolutionStep resolutionStep = ScheduledProcedureStep<ProtocolResolutionStep>(rp);

					if (resolutionStep != null)
					{
						resolutionStep.Complete(resolvingStaff);
						resolutionStep.Protocol.Resolve();
						ProtocolAssignmentStep assignmentStep = new ProtocolAssignmentStep(resolutionStep.Protocol);
						rp.AddProcedureStep(assignmentStep);
					}
				}
			}

			public override bool CanExecute(Order order, Staff currentUserStaff)
			{
				foreach (Procedure rp in order.Procedures)
				{
					ProtocolResolutionStep resolutionStep = ScheduledProcedureStep<ProtocolResolutionStep>(rp);

					if (resolutionStep != null)
					{
						if (resolutionStep.State == ActivityStatus.SC)
							return true;
					}
				}

				return false;
			}
		}

		public class SubmitForApprovalOperation : ProtocollingOperation
		{
			public void Execute(Order order)
			{
				foreach (Procedure rp in order.Procedures)
				{
					ProtocolAssignmentStep assignmentStep = InprogressProcedureStep<ProtocolAssignmentStep>(rp);

					if (assignmentStep != null)
					{
						assignmentStep.Complete();
						assignmentStep.Protocol.SubmitForApproval();

						// Replace with new step scheduled step
						ProtocolAssignmentStep approvalStep = new ProtocolAssignmentStep(assignmentStep.Protocol);
						rp.AddProcedureStep(approvalStep);

						approvalStep.Schedule(DateTime.Now);
					}
				}
			}

			public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
			{
				return step.State == ActivityStatus.IP;
			}
		}
	}
}