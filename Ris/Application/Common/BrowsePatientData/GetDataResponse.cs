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

namespace ClearCanvas.Ris.Application.Common.BrowsePatientData
{
    [DataContract]
    public class GetDataResponse : DataContractBase
    {
        [DataMember]
        public ListPatientProfilesResponse ListPatientProfilesResponse;

        [DataMember]
        public GetPatientProfileDetailResponse GetPatientProfileDetailResponse;

		[DataMember]
		public ListVisitsResponse ListVisitsResponse;

		[DataMember]
		public GetVisitDetailResponse GetVisitDetailResponse;

		[DataMember]
        public ListOrdersResponse ListOrdersResponse;

        [DataMember]
        public GetOrderDetailResponse GetOrderDetailResponse;

        [DataMember]
        public ListReportsResponse ListReportsResponse;

        [DataMember]
        public GetReportDetailResponse GetReportDetailResponse;
    }
}
