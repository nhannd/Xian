using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

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