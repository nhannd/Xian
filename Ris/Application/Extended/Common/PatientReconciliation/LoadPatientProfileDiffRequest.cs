#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.PatientReconciliation
{
    [DataContract]
    public class LoadPatientProfileDiffRequest : DataContractBase
    {
        public LoadPatientProfileDiffRequest(EntityRef leftProfile, EntityRef rightProfile)
        {
            this.LeftProfileRef = leftProfile;
            this.RightProfileRef = rightProfile;
        }

        [DataMember]
        public EntityRef LeftProfileRef;

        [DataMember]
        public EntityRef RightProfileRef;
    }
}
