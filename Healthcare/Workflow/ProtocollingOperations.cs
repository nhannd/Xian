using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow
{
    public class ProtocollingOperations
    {
        public abstract class ProtocollingOperation
        {
            public abstract bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff);
        }

        public class SaveProtocolOperation : ProtocollingOperation
        {
            public void Execute(ProtocolProcedureStep step, Staff currentStaffUser, IPersistenceContext context)
            {
                if(step.Protocol != null)
                {
                    step.Protocol.Author = currentStaffUser;
                    //step.Protocol.Codes
                    //step.Protocol.Notes
                    //step.Protocol.ApprovalRequired
                }
                else
                {
                    Protocol protocol = new Protocol();
                    protocol.Procedure = step.RequestedProcedure;
                    //step.Protocol.Codes
                    //step.Protocol.Notes
                    //step.Protocol.ApprovalRequired

                    context.Lock(protocol);
                }
            }

            public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
            {
                if(step.Protocol == null)
                    return false;

                return true;
            }
        }

        public class AcceptProtocolOperation : ProtocollingOperation
        {
            public void Execute(ProtocolProcedureStep step, Staff currentUserStaff)
            {
                if(step.Performer != null)
                    step.Complete();
                else 
                    step.Complete(currentUserStaff);
            }

            public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
            {
                return step.State == ActivityStatus.SC || step.State == ActivityStatus.IP;
            }
        }

        public class RejectProtocolOperation : ProtocollingOperation
        {
            public void Execute(ProtocolProcedureStep step, Staff currentUserStaff)
            {
                step.Suspend();
            }

            public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
            {
                return step.State == ActivityStatus.SC || step.State == ActivityStatus.IP;
            }
        }

        public class SuspendProtocolOperation : ProtocollingOperation
        {
            public void Execute(ProtocolProcedureStep step, Staff currentUserStaff)
            {
                step.Suspend();
            }

            public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
            {
                return step.State == ActivityStatus.SC || step.State == ActivityStatus.IP;
            }
        }

        public class ResolveProtocolOperation : ProtocollingOperation
        {
            public void Execute(ProtocolProcedureStep step, Staff currentUserStaff)
            {
                step.Resume();
            }

            public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
            {
                return step.State == ActivityStatus.SU;
            }
        }

        public class ResolveAsCancelledProtocolOperation : ProtocollingOperation
        {
            public void Execute(ProtocolProcedureStep step, Staff currentUserStaff)
            {
                step.Discontinue();
            }

            public override bool CanExecute(ProtocolProcedureStep step, Staff currentUserStaff)
            {
                return step.State == ActivityStatus.SU;
            }
        }
    }
}
