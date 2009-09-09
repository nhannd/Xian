using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.Iod
{
	public interface ISeriesData
	{
		/// <summary>
		/// Gets the Study Instance Uid of the identified series.
		/// </summary>
		[DicomField(DicomTags.StudyInstanceUid)]
		string StudyInstanceUid { get; }

		/// <summary>
		/// Gets the Series Instance Uid of the identified series.
		/// </summary>
		[DicomField(DicomTags.SeriesInstanceUid)]
		string SeriesInstanceUid { get; }

		/// <summary>
		/// Gets the modality of the identified series.
		/// </summary>
		[DicomField(DicomTags.Modality)]
		string Modality { get; }

		/// <summary>
		/// Gets the series description of the identified series.
		/// </summary>
		[DicomField(DicomTags.SeriesDescription)]
		string SeriesDescription { get; }

		/// <summary>
		/// Gets the series number of the identified series.
		/// </summary>
		[DicomField(DicomTags.SeriesNumber)]
		int SeriesNumber { get; }

		/// <summary>
		/// Gets the number of composite object instances belonging to the identified series.
		/// </summary>
		[DicomField(DicomTags.NumberOfSeriesRelatedInstances)]
		int? NumberOfSeriesRelatedInstances { get; }
	}
}