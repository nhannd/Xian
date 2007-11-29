#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
    public class Worklists
    {
        public abstract class ReportingWorklist : Worklist
        {
            protected IReportingWorklistBroker GetBroker(IPersistenceContext context)
            {
                return GetBroker<IReportingWorklistBroker>(context);
            }

            public override string Description
            {
                get { return "Reporting"; }
            }

            public override string Name
            {
                get { return "Reporting"; }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingDraftWorklist")]
        public class Draft : ReportingWorklist
        {
            private ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
                criteria.ReportingProcedureStep.Scheduling.Performer.Staff.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetWorklist(typeof(InterpretationStep), GetQueryConditions(currentUserStaff), null);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetWorklistCount(typeof(InterpretationStep), GetQueryConditions(currentUserStaff), null);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingInTranscriptionWorklist")]
        public class InTranscription : ReportingWorklist
        {
            private ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
                criteria.ReportingProcedureStep.Scheduling.Performer.Staff.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetWorklist(typeof(TranscriptionStep), GetQueryConditions(currentUserStaff), null);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetWorklistCount(typeof(TranscriptionStep), GetQueryConditions(currentUserStaff), null);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingToBeVerifiedWorklist")]
        public class ToBeVerified : ReportingWorklist
        {
            private ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
                criteria.ReportingProcedureStep.Scheduling.Performer.Staff.EqualTo(currentUserStaff);
                criteria.ReportPart.Interpreter.EqualTo(currentUserStaff);
                criteria.ReportPart.Supervisor.IsNull();
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            private ReportingWorklistItemSearchCriteria[] GetResidentQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
                criteria.ReportPart.Interpreter.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                if (currentUserStaff.Type == StaffType.PRAR)
                    return (IList)GetBroker(context).GetWorklist(typeof(VerificationStep), GetResidentQueryConditions(currentUserStaff), null);
                else
                    return (IList)GetBroker(context).GetWorklist(typeof(VerificationStep), GetQueryConditions(currentUserStaff), null);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                if (currentUserStaff.Type == StaffType.PRAR)
                    return GetBroker(context).GetWorklistCount(typeof(VerificationStep), GetResidentQueryConditions(currentUserStaff), null);
                else
                    return GetBroker(context).GetWorklistCount(typeof(VerificationStep), GetQueryConditions(currentUserStaff), null);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingVerifiedWorklist")]
        public class Verified : ReportingWorklist
        {
            private ReportingWorklistItemSearchCriteria[] GetResidentQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.CM });
                criteria.ReportPart.Interpreter.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            private ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.CM });
                criteria.ReportPart.Verifier.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                if (currentUserStaff.Type == StaffType.PRAR)
                    return (IList)GetBroker(context).GetWorklist(typeof(PublicationStep), GetResidentQueryConditions(currentUserStaff), null);
                else
                    return (IList)GetBroker(context).GetWorklist(typeof(PublicationStep), GetQueryConditions(currentUserStaff), null);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                if (currentUserStaff.Type == StaffType.PRAR)
                    return GetBroker(context).GetWorklistCount(typeof(PublicationStep), GetResidentQueryConditions(currentUserStaff), null);
                else
                    return GetBroker(context).GetWorklistCount(typeof(PublicationStep), GetQueryConditions(currentUserStaff), null);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingReviewResidentReport")]
        public class ReviewResidentReport : ReportingWorklist
        {
            private ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
                criteria.ReportPart.Supervisor.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetWorklist(typeof(VerificationStep), GetQueryConditions(currentUserStaff), null);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetWorklistCount(typeof(VerificationStep), GetQueryConditions(currentUserStaff), null);
            }
        }

        //[ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingToBeApprovedWorklist")]
        //public class ToBeApproved : ReportingWorklist
        //{
        //    private ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
        //    {
        //        ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
        //        criteria.ReportingProcedureStep.State.EqualTo(ActivityStatus.IP);
        //        criteria.Protocol.Status.EqualTo(ProtocolStatus.AA);
        //        return new ReportingWorklistItemSearchCriteria[] { criteria };
        //    }

        //    public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
        //    {
        //        return (IList)GetBroker(context).GetWorklist(typeof(ProtocolProcedureStep), GetQueryConditions(currentUserStaff), null);
        //    }

        //    public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
        //    {
        //        return GetBroker(context).GetWorklistCount(typeof(ProtocolProcedureStep), GetQueryConditions(currentUserStaff), null);
        //    }
        //}

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingDraftProtocolWorklist")]
        public class DraftProtocol : ReportingWorklist
        {
            private ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.EqualTo(ActivityStatus.IP);
                criteria.ReportingProcedureStep.Performer.Staff.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetWorklist(typeof(ProtocolAssignmentStep), GetQueryConditions(currentUserStaff), null);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetWorklistCount(typeof(ProtocolAssignmentStep), GetQueryConditions(currentUserStaff), null);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingCompletedProtocolWorklist")]
        public class CompletedProtocol : ReportingWorklist
        {
            private ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.EqualTo(ActivityStatus.CM);
                criteria.ReportingProcedureStep.Performer.Staff.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetWorklist(typeof(ProtocolAssignmentStep), GetQueryConditions(currentUserStaff), null);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetWorklistCount(typeof(ProtocolAssignmentStep), GetQueryConditions(currentUserStaff), null);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingSuspendedProtocolWorklist")]
        public class SuspendedProtocol : ReportingWorklist
        {
            private ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.EqualTo(ActivityStatus.DC);
                criteria.Protocol.Status.EqualTo(ProtocolStatus.SU);
                criteria.ReportingProcedureStep.Performer.Staff.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetWorklist(typeof(ProtocolAssignmentStep), GetQueryConditions(currentUserStaff), null);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetWorklistCount(typeof(ProtocolAssignmentStep), GetQueryConditions(currentUserStaff), null);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingRejectedProtocolWorklist")]
        public class RejectedProtocol : ReportingWorklist
        {
            private ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.EqualTo(ActivityStatus.DC);
                criteria.Protocol.Status.EqualTo(ProtocolStatus.RJ);
                criteria.ReportingProcedureStep.Performer.Staff.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetWorklist(typeof(ProtocolAssignmentStep), GetQueryConditions(currentUserStaff), null);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetWorklistCount(typeof(ProtocolAssignmentStep), GetQueryConditions(currentUserStaff), null);
            }
        }
    }
}
