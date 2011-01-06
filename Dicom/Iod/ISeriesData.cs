#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;

namespace ClearCanvas.Dicom.Iod
{
	public interface ISeriesData
	{
		/// <summary>
		/// Gets the Study Instance UID of the identified series.
		/// </summary>
		[DicomField(DicomTags.StudyInstanceUid)]
		string StudyInstanceUid { get; }

		/// <summary>
		/// Gets the Series Instance UID of the identified series.
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