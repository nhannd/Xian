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

namespace ClearCanvas.Ris.Application.Common.TranscriptionWorkflow
{
	[DataContract]
	public class LoadTranscriptionForEditResponse : DataContractBase
	{
		public LoadTranscriptionForEditResponse(ReportDetail report, int reportPartIndex, OrderDetail order)
		{
			Report = report;
			ReportPartIndex = reportPartIndex;
			Order = order;
		}

		/// <summary>
		/// Gets the report detail.
		/// </summary>
		[DataMember]
		public ReportDetail Report;

		/// <summary>
		/// Gets the index of the active report part.
		/// </summary>
		[DataMember]
		public int ReportPartIndex;

		/// <summary>
		/// Gets the order detail.
		/// </summary>
		[DataMember]
		public OrderDetail Order;
	}
}