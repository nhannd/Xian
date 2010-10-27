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
	/// Represents the serializable patient information
	/// </summary>
	[XmlRoot("Patient")]
	public class PatientInformation
	{
		#region Constructors
		public PatientInformation()
		{
		}

		public PatientInformation(IDicomAttributeProvider attributeProvider)
		{
			if (attributeProvider[DicomTags.PatientsName] != null)
				Name = attributeProvider[DicomTags.PatientsName].ToString();

			if (attributeProvider[DicomTags.PatientId] != null)
				PatientId = attributeProvider[DicomTags.PatientId].ToString();

			if (attributeProvider[DicomTags.PatientsAge] != null)
				Age = attributeProvider[DicomTags.PatientsAge].ToString();

			if (attributeProvider[DicomTags.PatientsBirthDate] != null)
				PatientsBirthdate = attributeProvider[DicomTags.PatientsBirthDate].ToString();

			if (attributeProvider[DicomTags.PatientsSex] != null)
				Sex = attributeProvider[DicomTags.PatientsSex].ToString();

			if (attributeProvider[DicomTags.IssuerOfPatientId] != null)
				IssuerOfPatientId = attributeProvider[DicomTags.IssuerOfPatientId].ToString();
		}
		#endregion

		#region Public Properties

		public string Name { get; set; }

		public string PatientId { get; set; }

		public string PatientsBirthdate { get; set; }

		public string Age { get; set; }

		public string Sex { get; set; }

		public string IssuerOfPatientId { get; set; }

		#endregion
	}
}