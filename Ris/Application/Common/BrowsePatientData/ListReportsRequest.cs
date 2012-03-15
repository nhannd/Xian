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
    [DataContract]
    public class ListReportsRequest : DataContractBase
    {
        public ListReportsRequest()
        {
        }

        public ListReportsRequest(EntityRef patientRef)
        {
            this.PatientRef = patientRef;
        }

		public ListReportsRequest(EntityRef patientRef, EntityRef orderRef)
		{
			this.PatientRef = patientRef;
			this.OrderRef = orderRef;
		}


		/// <summary>
		/// Specifies patient whose reports should be returned. Ignored if <see cref="OrderRef"/> is valued.
		/// </summary>
        [DataMember]
        public EntityRef PatientRef;

		/// <summary>
		/// Specifies the order whose reports should be returned.  May be null.
		/// </summary>
		[DataMember]
		public EntityRef OrderRef;
	}
}
