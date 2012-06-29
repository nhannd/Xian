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
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Application.Common.Admin.PatientAdmin
{
    [DataContract]
    public class UpdatePatientProfileResponse : DataContractBase
    {
        public UpdatePatientProfileResponse(PatientProfileSummary patientProfile)
        {
            this.PatientProfile = patientProfile;
        }

        [DataMember]
        public PatientProfileSummary PatientProfile;
    }
}
