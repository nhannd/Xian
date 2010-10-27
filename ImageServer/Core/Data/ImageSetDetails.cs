#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Xml.Serialization;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Core.Data
{
	/// <summary>
	/// Represents the serializable detailed information of an image set.
	/// </summary>
	[XmlRoot("Details")]
	public class ImageSetDetails
	{
		#region Constructors

		public ImageSetDetails()
		{
		    StudyInfo = new StudyInformation();
		}

		public ImageSetDetails(IDicomAttributeProvider attributeProvider)
		{
			StudyInfo = new StudyInformation(attributeProvider);
		}

		#endregion

		#region Public Properties

		public int SopInstanceCount { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="StudyInformation"/> of the image set.
		/// </summary>
		public StudyInformation StudyInfo { get; set; }

		#endregion

		#region Public Methods
		/// <summary>
		/// Inserts a <see cref="DicomMessageBase"/> into the set.
		/// </summary>
		/// <param name="message"></param>
		public void InsertFile(DicomMessageBase message)
		{
			StudyInfo.Add(message);
		}
		#endregion
	}
}