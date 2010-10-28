#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.Utilities.Anonymization
{
	/// <summary>
	/// A class containing commonly anonymized dicom series attributes.
	/// </summary>
	[Cloneable(true)]
	public class SeriesData
	{
		private string _seriesInstanceUid = "";
		private string _seriesDescription = "";
		private string _seriesNumber = "";
		private string _protocolName = "";

		/// <summary>
		/// Constructor.
		/// </summary>
		public SeriesData()
		{
		}

		/// <summary>
		/// Gets or sets the series description.
		/// </summary>
		[DicomField(DicomTags.SeriesDescription)] 
		public string SeriesDescription
		{
			get { return _seriesDescription; }
			set { _seriesDescription = value ?? ""; }
		}

		/// <summary>
		/// Gets or sets the series number.
		/// </summary>
		[DicomField(DicomTags.SeriesNumber)] 
		public string SeriesNumber
		{
			get { return _seriesNumber; }
			set { _seriesNumber = value ?? ""; }
		}

		/// <summary>
		/// Gets or sets the protocol name.
		/// </summary>
		[DicomField(DicomTags.ProtocolName)]
		public string ProtocolName
		{
			get { return _protocolName; }
			set { _protocolName = value ?? ""; }
		}

		internal string SeriesInstanceUid
		{
			get { return _seriesInstanceUid; }
			set { _seriesInstanceUid = value ?? ""; }
		}

		internal void LoadFrom(DicomFile file)
		{
			file.DataSet.LoadDicomFields(this);
			this.SeriesInstanceUid = file.DataSet[DicomTags.SeriesInstanceUid];
		}

		internal void SaveTo(DicomFile file)
		{
			file.DataSet.SaveDicomFields(this);
			file.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(this.SeriesInstanceUid);
		}
		
		/// <summary>
		/// Creates a deep clone of this instance.
		/// </summary>
		public SeriesData Clone()
		{
			return CloneBuilder.Clone(this) as SeriesData;
		}
	}
}