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

using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Anonymization;
using Path=ClearCanvas.Desktop.Path;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Export
{
	[ExtensionPoint]
	public sealed class ExportComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (ExportComponentViewExtensionPoint))]
	public class ExportComponent : ApplicationComponent
	{
		private readonly List<DicomFile> _files = new List<DicomFile>();
		private string _patientId = "";
		private string _patientsName = "PATIENT^ANONYMOUS";
		private DateTime? _patientsDateOfBirth = DateTime.Today;
		private string _studyId = "";
		private string _studyDescription = "";
		private string _accessionNumber = "00000001";
		private DateTime? _studyDateTime = DateTime.Today;

		private string _outputPath = "";

		public ExportComponent() {}

		public IList<DicomFile> Files
		{
			get { return _files; }
		}

		public string PatientId
		{
			get { return _patientId; }
			set
			{
				if (_patientId != value)
				{
					_patientId = value;
					base.NotifyPropertyChanged("PatientId");
				}
			}
		}

		public string PatientsName
		{
			get { return _patientsName; }
			set
			{
				if (_patientsName != value)
				{
					_patientsName = value;
					base.NotifyPropertyChanged("PatientsName");
				}
			}
		}

		public DateTime? PatientsDateOfBirth
		{
			get { return _patientsDateOfBirth; }
			set
			{
				if (_patientsDateOfBirth != value)
				{
					_patientsDateOfBirth = value;
					base.NotifyPropertyChanged("PatientsDateOfBirth");
				}
			}
		}

		public string StudyId {
			get { return _studyId; }
			set {
				if (_studyId != value) {
					_studyId = value;
					base.NotifyPropertyChanged("StudyId");
				}
			}
		}

		public string StudyDescription
		{
			get { return _studyDescription; }
			set
			{
				if (_studyDescription != value)
				{
					_studyDescription = value;
					base.NotifyPropertyChanged("StudyDescription");
				}
			}
		}

		public string AccessionNumber
		{
			get { return _accessionNumber; }
			set
			{
				if (_accessionNumber != value)
				{
					_accessionNumber = value;
					base.NotifyPropertyChanged("AccessionNumber");
				}
			}
		}

		public DateTime? StudyDateTime
		{
			get { return _studyDateTime; }
			set
			{
				if (_studyDateTime != value)
				{
					_studyDateTime = value;
					base.NotifyPropertyChanged("StudyDateTime");
				}
			}
		}

		public string OutputPath
		{
			get { return _outputPath; }
			set
			{
				if(_outputPath!=value)
				{
					_outputPath = value;
					base.NotifyPropertyChanged("OutputPath");
				}
			}
		}

		public bool ShowOutputPathDialog()
		{
			SelectFolderDialogCreationArgs dialogArgs = new SelectFolderDialogCreationArgs(_outputPath);
			dialogArgs.AllowCreateNewFolder = true;
			dialogArgs.Path = this.OutputPath;
			dialogArgs.Prompt = SR.MessageSelectOutputLocation;
			FileDialogResult result = base.Host.DesktopWindow.ShowSelectFolderDialogBox(dialogArgs);
			if(result.Action == DialogBoxAction.Ok)
			{
				this.OutputPath = result.FileName;
				return true;
			}
			return false;
		}

		public void Anonymize()
		{
			if (string.IsNullOrEmpty(_outputPath) || !Directory.Exists(_outputPath))
			{
				if (!ShowOutputPathDialog())
					return;
			}

			StudyData studyData = new StudyData();
			studyData.AccessionNumber = _accessionNumber;
			studyData.PatientId = _patientId;
			studyData.PatientsBirthDate = _patientsDateOfBirth;
			studyData.PatientsNameRaw = _patientsName;
			studyData.StudyDate = _studyDateTime;
			studyData.StudyDescription = _studyDescription;
			studyData.StudyId = _studyId;

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyData;

			foreach (DicomFile file in _files)
			{
				anonymizer.Anonymize(file);
				file.Filename = System.IO.Path.Combine(_outputPath, string.Format("{0}.{1}", file.MediaStorageSopInstanceUid, "dcm"));
				file.Save();
			}

			base.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Abort()
		{
			base.Exit(ApplicationComponentExitCode.None);
		}
	}
}