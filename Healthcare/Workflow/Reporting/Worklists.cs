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
    [ExtensionPoint]
    public class ReportingWorklistExtensionPoint : ExtensionPoint<IWorklist>
    {
    }

    public class Worklists
    {
        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class ToBeReported : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetWorklist(typeof(InterpretationStep), ReportingToBeReportedWorklist.QueryConditions, null);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetWorklistCount(typeof(InterpretationStep), ReportingToBeReportedWorklist.QueryConditions, null);
            }
        }

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class Draft : WorklistBase<IReportingWorklistBroker>
        {
            public ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
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

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class InTranscription : WorklistBase<IReportingWorklistBroker>
        {
            public ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
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

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class ToBeVerified : WorklistBase<IReportingWorklistBroker>
        {
            public ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
                criteria.ReportingProcedureStep.Scheduling.Performer.Staff.EqualTo(currentUserStaff);
                criteria.ReportPart.Interpretor.EqualTo(currentUserStaff);
                criteria.ReportPart.Supervisor.IsNull();
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            public ReportingWorklistItemSearchCriteria[] GetResidentQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
                criteria.ReportPart.Interpretor.EqualTo(currentUserStaff);
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

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class Verified : WorklistBase<IReportingWorklistBroker>
        {
            public ReportingWorklistItemSearchCriteria[] GetResidentQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.CM });
                criteria.ReportPart.Interpretor.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            public ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
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

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class MyResidentToBeVerified : WorklistBase<IReportingWorklistBroker>
        {
            public ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
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

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class ToBeProtocolled : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetWorklist(typeof(ProtocolProcedureStep), ReportingToBeProtocolledWorklist.QueryConditions, null);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetWorklistCount(typeof(ProtocolProcedureStep), ReportingToBeProtocolledWorklist.QueryConditions, null);
            }
        }

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class ToBeApproved : WorklistBase<IReportingWorklistBroker>
        {
            public ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.EqualTo(ActivityStatus.IP);
                criteria.Protocol.Status.EqualTo(ProtocolStatus.AA);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetWorklist(typeof(ProtocolProcedureStep), GetQueryConditions(currentUserStaff), null);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetWorklistCount(typeof(ProtocolProcedureStep), GetQueryConditions(currentUserStaff), null);
            }
        }

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class CompletedProtocol : WorklistBase<IReportingWorklistBroker>
        {
            public ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.CM, ActivityStatus.DC });
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetWorklist(typeof(ProtocolProcedureStep), GetQueryConditions(currentUserStaff), null);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetWorklistCount(typeof(ProtocolProcedureStep), GetQueryConditions(currentUserStaff), null);
            }
        }

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class SuspendedProtocol : WorklistBase<IReportingWorklistBroker>
        {
            public ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.EqualTo(ActivityStatus.SU);
                criteria.Protocol.Status.EqualTo(ProtocolStatus.SU);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetWorklist(typeof(ProtocolProcedureStep), GetQueryConditions(currentUserStaff), null);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetWorklistCount(typeof(ProtocolProcedureStep), GetQueryConditions(currentUserStaff), null);
            }
        }
    }
}
