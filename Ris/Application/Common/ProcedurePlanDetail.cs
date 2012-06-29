#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ProcedurePlanDetail : DataContractBase
    {
        [DataMember] 
        public EntityRef OrderRef;

        [DataMember]
        public DiagnosticServiceSummary DiagnosticServiceSummary;

        [DataMember]
        public List<ProcedureDetail> Procedures;
    }
}