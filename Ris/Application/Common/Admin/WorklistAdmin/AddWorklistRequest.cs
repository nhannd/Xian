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

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class AddWorklistRequest : DataContractBase
    {
        public AddWorklistRequest(WorklistAdminDetail detail, bool isUserWorklist)
        {
            Detail = detail;
        	IsUserWorklist = isUserWorklist;
        }


		/// <summary>
		/// Specifies whether the request should create a user-defined worklist, as opposed to an administratively-owned worklist.
		/// </summary>
		[DataMember]
    	public bool IsUserWorklist;

		/// <summary>
		/// Specifies the details of the worklist to create.
		/// </summary>
        [DataMember]
        public WorklistAdminDetail Detail;
    }
}