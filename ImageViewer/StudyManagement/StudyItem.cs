#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System;
using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	//TODO: get rid of this or incorporate into Dicom somehow.
	//NOTE: Leave this until we do ServerTree refactoring.

	/// <summary>
	/// Represents a remote dicom server.
	/// </summary>
	public class ApplicationEntity
	{
		private readonly string _host;
		private readonly string _aeTitle;
		private readonly int _port;
		private readonly int _headerServicePort;
		private readonly int _wadoServicePort;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="host"></param>
		/// <param name="aeTitle"></param>
		/// <param name="port"></param>
		/// <param name="headerServicePort"></param>
		/// <param name="wadoServicePort"></param>
		public ApplicationEntity(
			string host, 
			string aeTitle, 
			int port,
			int headerServicePort,
			int wadoServicePort)
		{
			_host = host;
			_aeTitle = aeTitle;
			_port = port;
			_headerServicePort = headerServicePort;
			_wadoServicePort = wadoServicePort;
		}

		/// <summary>
		/// The host name or IP address.
		/// </summary>
		public string Host
		{
			get { return _host; }
		}

		/// <summary>
		/// The AE Title.
		/// </summary>
		public string AETitle
		{
			get { return _aeTitle; }
		}

		/// <summary>
		/// The DICOM listening port.
		/// </summary>
		public int Port
		{
			get { return _port; }
		}

		/// <summary>
		/// Header service port for image streaming.
		/// </summary>
		public int HeaderServicePort
		{
			get { return _headerServicePort; }
		}

		/// <summary>
		/// WADO service port.
		/// </summary>
		public int WadoServicePort
		{
			get { return _wadoServicePort; }
		}
	}

	/// <summary>
	/// A study item.
	/// </summary>
	public class StudyItem
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StudyItem"/>.
		/// </summary>
		public StudyItem()
		{
		}

		/// <summary>
		/// Gets or sets the patient's birthdate.
		/// </summary>
		public string PatientsBirthDate
        {
            get { return _patientsBirthDate; }
            set { _patientsBirthDate = value; }
        }

		/// <summary>
		/// Gets or sets the patient's accession number.
		/// </summary>
		public string AccessionNumber
        {
            get { return _accessionNumber; }
            set { _accessionNumber = value; }
        }

		/// <summary>
		/// Gets or sets the study description.
		/// </summary>
		public string StudyDescription
        {
            get { return _studyDescription; }
            set { _studyDescription = value; }
        }

		/// <summary>
		/// Gets or sets the study date.
		/// </summary>
		public string StudyDate
        {
            get { return _studyDate; }
            set { _studyDate = value; }
        }

		/// <summary>
		/// Gets or sets the patient ID.
		/// </summary>
		public string PatientId
		{
			get { return _patientID; }
			set { _patientID = value; }
		}

		/// <summary>
		/// Gets or sets the patient's name.
		/// </summary>
		public PersonName PatientsName
        {
            get { return _patientsName; }
            set { _patientsName = value; }
        }

		/// <summary>
		/// Gets or sets the modalities in the study.
		/// </summary>
		public string ModalitiesInStudy
        {
            get { return _modalitiesInStudy; }
            set { _modalitiesInStudy = value; }
        }

		/// <summary>
		/// Gets or sets the Study Instance UID.
		/// </summary>
		public string StudyInstanceUID
		{
			get { return _studyInstanceUID; }
			set { _studyInstanceUID = value; }
		}

		/// <summary>
		/// Gets or sets the number of study related instances.
		/// </summary>
		public uint NumberOfStudyRelatedInstances
        {
            get { return _numberOfStudyRelatedInstances; }
            set { _numberOfStudyRelatedInstances = value; }
        }

		/// <summary>
		/// Gets or sets the specific character set.
		/// </summary>
		public string SpecificCharacterSet
        {
            get { return _specificCharacterSet; }
            set { _specificCharacterSet = value; }
        }

		/// <summary>
		/// Gets or sets the study loader name.
		/// </summary>
		public string StudyLoaderName
		{
			get { return _studyLoaderName; }
			set { _studyLoaderName = value; }
		}

		/// <summary>
		/// Gets or sets the server.
		/// </summary>
		public ApplicationEntity Server
		{
			get { return _server; }
			set { _server = value; }
		}

		/// <summary>
		/// Returns the patient's name, patient ID and study date associated
		/// with the <see cref="StudyItem"/> in string form.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			DateTime studyDate;
			DateParser.Parse(this.StudyDate, out studyDate);

			return String.Format("{0}; {1}; {2}",
				this.PatientsName,
				this.PatientId,
				studyDate.ToString(Format.DateFormat));
		}

        #region Private Members
        private string _patientID;
        private string _patientsBirthDate;
        private string _accessionNumber;
        private string _studyDescription;
        private string _studyDate;
        private string _studyInstanceUID;
        private string _studyLoaderName;
        private string _modalitiesInStudy;
        private uint _numberOfStudyRelatedInstances;
        private string _specificCharacterSet;
		private ApplicationEntity _server;
        private PersonName _patientsName;
        #endregion
	}

	/// <summary>
	/// A list of <see cref="StudyItem"/> objects.
	/// </summary>
	public class StudyItemList : List<StudyItem>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StudyItemList"/>.
		/// </summary>
		public StudyItemList()
		{
		}
	}

	/// <summary>
	/// A map of query parameters.
	/// </summary>
	public class QueryParameters : Dictionary<string,string>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="QueryParameters"/>.
		/// </summary>
		public QueryParameters()
		{
		}
	}
}
