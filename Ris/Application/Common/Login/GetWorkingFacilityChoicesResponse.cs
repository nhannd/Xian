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
using ClearCanvas.Common.Serialization;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Login
{
    [DataContract]
    public class GetWorkingFacilityChoicesResponse : DataContractBase
    {
        public GetWorkingFacilityChoicesResponse(List<FacilitySummary> workingFacilityChoices)
        {
            this.FacilityChoices = workingFacilityChoices;
        }

        /// <summary>
        /// List of facilities that the user may choose an working facility.
        /// </summary>
        [DataMember]
        public List<FacilitySummary> FacilityChoices;
    }
}
