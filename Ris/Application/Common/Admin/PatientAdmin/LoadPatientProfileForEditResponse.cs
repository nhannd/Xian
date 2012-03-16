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

namespace ClearCanvas.Ris.Application.Common.Admin.PatientAdmin
{
    [DataContract]
    public class LoadPatientProfileForEditResponse : DataContractBase
    {
        public LoadPatientProfileForEditResponse(EntityRef patientRef, EntityRef patientProfileRef, PatientProfileDetail patientDetail)
        {
            this.PatientRef = patientRef;
            this.PatientProfileRef = patientProfileRef;
            this.PatientDetail = patientDetail;
        }

        [DataMember]
        public EntityRef PatientRef;

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public PatientProfileDetail PatientDetail;
    }
}
