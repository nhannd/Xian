#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.BrowsePatientData
{
    [Serializable]
    public enum PatientOrdersQueryDetailLevel
    {
        Order,
        Procedure
    }

    [DataContract]
    public class ListOrdersRequest : DataContractBase
    {
        public ListOrdersRequest(EntityRef patientRef, PatientOrdersQueryDetailLevel queryDetailLevel)
        {
            this.PatientRef = patientRef;
            this.QueryDetailLevel = queryDetailLevel;
        }

        public ListOrdersRequest()
        {
        }

        [DataMember]
        public EntityRef PatientRef;

        [DataMember]
        public PatientOrdersQueryDetailLevel QueryDetailLevel;
    }
}
