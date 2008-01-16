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
                    protocol.Procedure = step.Procedure;
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
