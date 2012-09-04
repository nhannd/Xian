#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Serialization;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.BrowsePatientData
{
    [DataContract]
    public class GetDataRequest : DataContractBase
    {
        [DataMember]
        public ListPatientProfilesRequest ListPatientProfilesRequest;

        [DataMember]
        public GetPatientProfileDetailRequest GetPatientProfileDetailRequest;

		[DataMember]
		public ListVisitsRequest ListVisitsRequest;

		[DataMember]
		public GetVisitDetailRequest GetVisitDetailRequest;

		[DataMember]
        public ListOrdersRequest ListOrdersRequest;

        [DataMember]
        public GetOrderDetailRequest GetOrderDetailRequest;

        [DataMember]
        public ListReportsRequest ListReportsRequest;

        [DataMember]
        public GetReportDetailRequest GetReportDetailRequest;
    }
}
