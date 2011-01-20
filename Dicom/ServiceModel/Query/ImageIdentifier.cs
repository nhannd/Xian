#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
	public interface IImageIdentifier : ISopInstanceData, IIdentifier
	{
		[DicomField(DicomTags.InstanceNumber)]
		new int? InstanceNumber { get; }
	}

	/// <summary>
	/// Query identifier for a composite object instance.
	/// </summary>
	[DataContract(Namespace = QueryNamespace.Value)]
	public class ImageIdentifier : Identifier, IImageIdentifier
	{
		#region Private Fields

		private string _studyInstanceUid;
		private string _seriesInstanceUid;
		private string _sopInstanceUid;
		private string _sopClassUid;
		private int? _instanceNumber;

		#endregion

		#region Public Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ImageIdentifier()
		{
		}

		public ImageIdentifier(IImageIdentifier other)
			: base(other)
		{
			CopyFrom(other);
			InstanceNumber = other.InstanceNumber;
		}

		public ImageIdentifier(ISopInstanceData other, IIdentifier identifier)
			: base(identifier)
		{
			CopyFrom(other);
		}

		public ImageIdentifier(ISopInstanceData other)
		{
			CopyFrom(other);
		}

		/// <summary>
		/// Creates an instance of <see cref="ImageIdentifier"/> from a <see cref="DicomAttributeCollection"/>.
		/// </summary>
		public ImageIdentifier(DicomAttributeCollection attributes)
			: base(attributes)
		{
		}

		#endregion

		private void CopyFrom(ISopInstanceData other)
		{
 			StudyInstanceUid = other.StudyInstanceUid;
			SeriesInstanceUid = other.SeriesInstanceUid;
			SopInstanceUid = other.SopInstanceUid;
			SopClassUid = other.SopClassUid;
			InstanceNumber = other.InstanceNumber;
		}

		#region Public Properties

		/// <summary>
		/// Gets the level of the query - IMAGE.
		/// </summary>
		public override string QueryRetrieveLevel
		{
			get { return "IMAGE"; }
		}

		/// <summary>
		/// Gets or sets the Study Instance Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.StudyInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public string StudyInstanceUid
		{
			get { return _studyInstanceUid; }
			set { _studyInstanceUid = value; }
		}

		/// <summary>
		/// Gets or sets the Series Instance Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.SeriesInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public string SeriesInstanceUid
		{
			get { return _seriesInstanceUid; }
			set { _seriesInstanceUid = value; }
		}

		/// <summary>
		/// Gets or sets the Sop Instance Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.SopInstanceUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public string SopInstanceUid
		{
			get { return _sopInstanceUid; }
			set { _sopInstanceUid = value; }
		}

		/// <summary>
		/// Gets or sets the Sop Class Uid of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.SopClassUid, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public string SopClassUid
		{
			get { return _sopClassUid; }
			set { _sopClassUid = value; }
		}

		/// <summary>
		/// Gets or sets the Instance Number of the identified sop instance.
		/// </summary>
		[DicomField(DicomTags.InstanceNumber, CreateEmptyElement = true, SetNullValueIfEmpty = true)]
		[DataMember(IsRequired = true)]
		public int? InstanceNumber
		{
			get { return _instanceNumber; }
			set { _instanceNumber = value; }
		}

		int ISopInstanceData.InstanceNumber
		{
		    get { return _instanceNumber ?? 0; }
		}

		#endregion

		public override string ToString()
		{
			return String.Format("{0} | {1}", this.InstanceNumber, this.SopInstanceUid);
		}
	}
}
