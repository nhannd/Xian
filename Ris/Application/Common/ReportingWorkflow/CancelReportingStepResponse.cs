#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class CancelReportingStepResponse : DataContractBase
    {
        public CancelReportingStepResponse(EntityRef cancelledStepRef, List<EntityRef> scheduledInterpretationStepRefs)
        {
            this.CancelledStepRef = cancelledStepRef;
            this.ScheduledInterpretationStepRefs = scheduledInterpretationStepRefs;
        }

        /// <summary>
        /// The step that was cancelled.
        /// </summary>
        [DataMember]
        public EntityRef CancelledStepRef;

        /// <summary>
        /// References to any new interpretation steps that were scheduled.
        /// </summary>
        [DataMember]
        public List<EntityRef> ScheduledInterpretationStepRefs;
    }
}