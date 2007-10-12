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
                return (IList)GetBroker(context).GetToBeReportedWorklist();
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetToBeReportedWorklistCount();
            }
        }

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class Draft : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetDraftWorklist(currentUserStaff);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetDraftWorklistCount(currentUserStaff);
            }
        }

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class InTranscription : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetInTranscriptionWorklist(currentUserStaff);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetInTranscriptionWorklistCount(currentUserStaff);
            }
        }

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class ToBeVerified : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetToBeVerifiedWorklist(currentUserStaff);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetToBeVerifiedWorklistCount(currentUserStaff);
            }
        }

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class ResidentToBeVerified : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetResidentToBeVerifiedWorklist(currentUserStaff);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetResidentToBeVerifiedWorklistCount(currentUserStaff);
            }
        }

        [ExtensionOf(typeof(ReportingWorklistExtensionPoint))]
        public class Verified : WorklistBase<IReportingWorklistBroker>
        {
            public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
            {
                return (IList)GetBroker(context).GetVerifiedWorklist(currentUserStaff);
            }

            public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
            {
                return GetBroker(context).GetVerifiedWorklistCount(currentUserStaff);
            }
        }

    }
}
