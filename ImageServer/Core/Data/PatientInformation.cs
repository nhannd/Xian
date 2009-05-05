#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
		#region Private Fields
		private string _name;
		private string _patientId;
		private string _issuerOfPatientId;
		private string _birthdate;
		private string _age;
		private string _sex;
		#endregion

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

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string PatientId
		{
			get { return _patientId; }
			set { _patientId = value; }
		}


		public string PatientsBirthdate
		{
			get { return _birthdate; }
			set { _birthdate = value; }
		}

		public string Age
		{
			get { return _age; }
			set { _age = value; }
		}

		public string Sex
		{
			get { return _sex; }
			set { _sex = value; }
		}

		public string IssuerOfPatientId
		{
			get { return _issuerOfPatientId; }
			set { _issuerOfPatientId = value; }
		}

		#endregion
	}
}