#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class CompleteInterpretationForTranscriptionRequest : CompleteInterpretationRequestBase
    {
        public CompleteInterpretationForTranscriptionRequest(EntityRef interpretationStepRef)
            : base(interpretationStepRef)
        {
        }

        public CompleteInterpretationForTranscriptionRequest(EntityRef interpretationStepRef
            , Dictionary<string, string> reportPartExtendedProperties
            , EntityRef supervisorRef)
            : base(interpretationStepRef, reportPartExtendedProperties, supervisorRef)
        {
        }
    }
}