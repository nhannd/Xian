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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
    public class Worklists
    {
        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingDraftWorklist")]
        public class Draft : ReportingWorklist
        {
            protected override ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
                criteria.ReportingProcedureStep.Scheduling.Performer.Staff.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            protected override Type ProcedureStepType
            {
                get { return typeof(InterpretationStep); }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingInTranscriptionWorklist")]
        public class InTranscription : ReportingWorklist
        {
            protected override ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
                criteria.ReportingProcedureStep.Scheduling.Performer.Staff.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            protected override Type ProcedureStepType
            {
                get { return typeof(TranscriptionStep); }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingToBeVerifiedWorklist")]
        public class ToBeVerified : ReportingWorklist
        {
            protected override ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
                if (currentUserStaff.Type == StaffType.PRAR)
                {
                    criteria.ReportPart.Interpreter.EqualTo(currentUserStaff);
                }
                else
                {
                    criteria.ReportingProcedureStep.Scheduling.Performer.Staff.EqualTo(currentUserStaff);
                    criteria.ReportPart.Interpreter.EqualTo(currentUserStaff);
                    criteria.ReportPart.Supervisor.IsNull();
                }
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            protected override Type ProcedureStepType
            {
                get { return typeof(VerificationStep); }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingVerifiedWorklist")]
        public class Verified : ReportingWorklist
        {
            protected override Type ProcedureStepType
            {
                get { return typeof(PublicationStep); }
            }

            protected override ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.CM });
                if (currentUserStaff.Type == StaffType.PRAR)
                {
                    criteria.ReportPart.Interpreter.EqualTo(currentUserStaff);
                }
                else
                {
                    criteria.ReportPart.Verifier.EqualTo(currentUserStaff);
                }
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                IList results = base.GetWorklist(currentUserStaff, context);

                // Addendum appears as a separate item - should only be one item
                // It was decided to filter the result collection instead of rewriting the query
                // Filter out duplicate WorklistItems that have the same requested procedure and keep the newest item
                Dictionary<EntityRef, WorklistItem> filter = new Dictionary<EntityRef, WorklistItem>();
                foreach (WorklistItem item in results)
                {
                    if (!filter.ContainsKey(item.RequestedProcedureRef))
                    {
                        filter.Add(item.RequestedProcedureRef, item);
                    }
                    else
                    {
                        WorklistItem existingItem = filter[item.RequestedProcedureRef];
                        if (item.CreationTime > existingItem.CreationTime)
                            filter[item.RequestedProcedureRef] = item;
                    }
                }

                return new List<WorklistItem>(filter.Values);
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingReviewResidentReport")]
        public class ReviewResidentReport : ReportingWorklist
        {
            protected override ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
                criteria.ReportPart.Supervisor.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            protected override Type ProcedureStepType
            {
                get { return typeof(VerificationStep); }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingDraftProtocolWorklist")]
        public class DraftProtocol : ReportingWorklist
        {
            protected override ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.EqualTo(ActivityStatus.IP);
                criteria.ReportingProcedureStep.Performer.Staff.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            protected override Type ProcedureStepType
            {
                get { return typeof(ProtocolAssignmentStep); }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingCompletedProtocolWorklist")]
        public class CompletedProtocol : ReportingWorklist
        {
            protected override ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.EqualTo(ActivityStatus.CM);
                criteria.ReportingProcedureStep.Performer.Staff.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            protected override Type ProcedureStepType
            {
                get { return typeof(ProtocolAssignmentStep); }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingSuspendedProtocolWorklist")]
        public class SuspendedProtocol : ReportingWorklist
        {
            protected override ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.EqualTo(ActivityStatus.DC);
                criteria.Protocol.Status.EqualTo(ProtocolStatus.SU);
                criteria.ReportingProcedureStep.Performer.Staff.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            protected override Type ProcedureStepType
            {
                get { return typeof(ProtocolAssignmentStep); }
            }
        }

        [ExtensionOf(typeof(WorklistExtensionPoint), Name = "ReportingRejectedProtocolWorklist")]
        public class RejectedProtocol : ReportingWorklist
        {
            protected override ReportingWorklistItemSearchCriteria[] GetQueryConditions(Staff currentUserStaff)
            {
                ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
                criteria.ReportingProcedureStep.State.EqualTo(ActivityStatus.DC);
                criteria.Protocol.Status.EqualTo(ProtocolStatus.RJ);
                criteria.ReportingProcedureStep.Performer.Staff.EqualTo(currentUserStaff);
                return new ReportingWorklistItemSearchCriteria[] { criteria };
            }

            protected override Type ProcedureStepType
            {
                get { return typeof(ProtocolAssignmentStep); }
            }
        }
    }
}
