#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	public interface ISeriesIdentifier : ISeriesData, IIdentifier
	{
		[DicomField(DicomTags.SeriesNumber)]
		new int? SeriesNumber { get; }
	}

	/// <summary>
	/// Query identifier for a series.
	/// </summary>
	[DataContract(Namespace = QueryNamespace.Value)]
	public class SeriesIdentifier : Identifier, ISeriesIdentifier
	{
		#region Private Fields

		private string _studyInstanceUid;
		private string _seriesInstanceUid;
		private string _modality;
		private string _seriesDescription;
		private int? _seriesNumber;
		private int? _numberOfSeriesRelatedInstances;

		#endregion

		#region Public Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public SeriesIdentifier()
		{
		}

		public SeriesIdentifier(ISeriesIdentifier other)
			: base(other)
		{
			CopyFrom(other);
			SeriesNumber = other.SeriesNumber;
		}

		public SeriesIdentifier(ISeriesData other, IIdentifier identifier)
			: base(identifier)
		{
			CopyFrom(other);
		}

		public SeriesIdentifier(ISeriesData other)
		{
			CopyFrom(other);
		}

		private void CopyFrom(ISeriesData other)
		{
			StudyInstanceUid = other.StudyInstanceUid;
			SeriesInstanceUid = other.SeriesInstanceUid;
			Modality = other.Modality;
			SeriesDescription = other.SeriesDescription;
			SeriesNumber = other.SeriesNumber;
			NumberOfSeriesRelatedInstances = other.NumberOfSeriesRelatedInstances;
		}

		/// <summary>
		/// Creates an instance of <see cref="SeriesIdentifier"/> from a <see cref="DicomAttributeCollection"/>.
		/// </summary>
		public SeriesIdentifier(DicomAttributeCollection attributes)
			: base(attributes)
		{
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the level of the query - SERIES.
		/// </summary>
		public override string QueryRetrieveLevel
		{
			get { return "SERIES"; }
		}

		/// <summary>
		/// Gets or sets the Study Instance Uid of the identified series.
		/// </summary>
		[DicomField(DicomTags.StudyInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		/// <summary>
		/// Gets or sets the Series Instance Uid of the identified series.
		/// </summary>
		[DicomField(DicomTags.SeriesInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public string SeriesInstanceUid
		{
			get { return _seriesInstanceUid; }
			set { _seriesInstanceUid = value; }
		}

		/// <summary>
		/// Gets or sets the modality of the identified series.
		/// </summary>
		[DicomField(DicomTags.Modality, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string Modality
		{
			get { return _modality; }
			set { _modality = value; }
		}

		/// <summary>
		/// Gets or sets the series description of the identified series.
		/// </summary>
		[DicomField(DicomTags.SeriesDescription, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public string SeriesDescription
		{
			get { return _seriesDescription; }
			set { _seriesDescription = value; }
		}

		/// <summary>
		/// Gets or sets the series number of the identified series.
		/// </summary>
		[DicomField(DicomTags.SeriesNumber, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public int? SeriesNumber
		{
			get { return _seriesNumber; }
			set { _seriesNumber = value; }
		}

		int ISeriesData.SeriesNumber
		{
			get { return _seriesNumber ?? 0; }
		}

		/// <summary>
		/// Gets or sets the number of composite object instances belonging to the identified series.
		/// </summary>
		[DicomField(DicomTags.NumberOfSeriesRelatedInstances, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = false)]
		public int? NumberOfSeriesRelatedInstances
		{
			get { return _numberOfSeriesRelatedInstances; }
			set { _numberOfSeriesRelatedInstances = value; }
		}

		#endregion

		public override string ToString()
		{
			return String.Format("{0} | {1} | {2}", this.SeriesNumber, this.SeriesDescription, this.SeriesInstanceUid);
		}
	}
}
