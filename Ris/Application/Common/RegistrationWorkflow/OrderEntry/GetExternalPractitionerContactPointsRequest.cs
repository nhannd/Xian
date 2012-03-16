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
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class GetExternalPractitionerContactPointsRequest : DataContractBase
    {
        public GetExternalPractitionerContactPointsRequest(EntityRef practitionerRef)
        {
            this.PractitionerRef = practitionerRef;
        }

        /// <summary>
        /// Reference to an external practitioner.
        /// </summary>
        [DataMember]
        public EntityRef PractitionerRef;

		/// <summary>
		/// Specifies whether to include de-activated items in the results.
		/// </summary>
		[DataMember]
		public bool IncludeDeactivated;
    }
}
