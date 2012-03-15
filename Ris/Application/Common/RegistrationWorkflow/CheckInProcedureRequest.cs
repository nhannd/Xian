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
using System;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class CheckInProcedureRequest : DataContractBase
    {
        public CheckInProcedureRequest(List<EntityRef> procedures, DateTime? checkInTime)
        {
            this.Procedures = procedures;
        	this.CheckInTime = checkInTime;
        }

        [DataMember]
        public List<EntityRef> Procedures;

		/// <summary>
		/// Specifies the time that the patient checked in.  If null, the server will assign the current time.
		/// </summary>
		[DataMember]
    	public DateTime? CheckInTime;
    }
}
