using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
	[DataContract]
	public class DicomSeriesDetail : DataContractBase
	{
		[DataMember]
		public EntityRef ModalityPerformedProcedureStepRef;

		[DataMember]
		public EntityRef DicomSeriesRef;

		[DataMember]
		public string StudyInstanceUID;

		[DataMember]
		public string SeriesInstanceUID;

		[DataMember]
		public string SeriesDescription;

		[DataMember]
		public string SeriesNumber;

		[DataMember]
		public int NumberOfSeriesRelatedInstances;
	}
}
