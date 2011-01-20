#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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