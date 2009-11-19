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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Export
{
	[ExtensionPoint]
	public sealed class ExportComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof (ExportComponentViewExtensionPoint))]
	public class ExportComponent : ApplicationComponent
	{
		private string _patientId = "12345678";
		private string _patientsName = "Patient^Anonymous";
		private DateTime? _patientsDateOfBirth = Platform.Time;
		private string _studyId = "";
		private string _studyDescription = "";
		private string _accessionNumber = "00000001";
		private DateTime? _studyDate = Platform.Time;

		private string _outputPath = "";

		internal ExportComponent()
		{
		}

		[ValidationMethodFor("OutputPath")]
		private ValidationResult ValidateOutputPath()
		{
			if (String.IsNullOrEmpty(OutputPath))
				return new ValidationResult(false, SR.MessageOutputPathMustBeSpecified);

			if (!Directory.Exists(OutputPath))
				return new ValidationResult(false, SR.MessageDirectoryDoesNotExist);

			return new ValidationResult(true, "");
		}

		//[ValidateLength(1, Message = "MessagePatientIdCannotBeEmpty")]
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

		//[ValidateLength(1, Message = "MessagePatientNameCannotBeEmpty")]
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

		public string StudyId
		{
			get { return _studyId; }
			set
			{
				if (_studyId != value)
				{
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

		public DateTime? StudyDate
		{
			get { return _studyDate; }
			set
			{
				if (_studyDate != value)
				{
					_studyDate = value;
					base.NotifyPropertyChanged("StudyDate");
				}
			}
		}

		public string OutputPath
		{
			get { return _outputPath; }
			set
			{
				if (_outputPath != value)
				{
					_outputPath = value;
					base.NotifyPropertyChanged("OutputPath");
				}
			}
		}

		public void ShowOutputPathDialog()
		{
			SelectFolderDialogCreationArgs dialogArgs = new SelectFolderDialogCreationArgs(_outputPath);
			dialogArgs.AllowCreateNewFolder = true;
			dialogArgs.Path = this.OutputPath;
			dialogArgs.Prompt = SR.MessageSelectOutputLocation;
			FileDialogResult result = base.Host.DesktopWindow.ShowSelectFolderDialogBox(dialogArgs);
			if (result.Action == DialogBoxAction.Ok)
				OutputPath = result.FileName;

			if (!this.HasValidationErrors)
				base.ShowValidation(true);
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				base.ShowValidation(true);
			}
			else
			{
				this.ExitCode = ApplicationComponentExitCode.Accepted;
				this.Host.Exit();
			}
		}
        
		public void Cancel()
		{
			base.Exit(ApplicationComponentExitCode.None);
		}
	}
}