using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
	[DataContract]
	public class DicomSeriesDetail : DataContractBase, ICloneable 
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

		#region ICloneable Members

		public object Clone()
		{
			DicomSeriesDetail clone = new DicomSeriesDetail();
			clone.ModalityPerformedProcedureStepRef = this.ModalityPerformedProcedureStepRef;
			clone.DicomSeriesRef = this.DicomSeriesRef;
			clone.StudyInstanceUID = this.StudyInstanceUID;
			clone.SeriesInstanceUID = this.SeriesInstanceUID;
			clone.SeriesNumber = this.SeriesNumber;
			clone.SeriesDescription = this.SeriesDescription;
			clone.NumberOfSeriesRelatedInstances = this.NumberOfSeriesRelatedInstances;
			return clone;
		}

		#endregion
	}
}
