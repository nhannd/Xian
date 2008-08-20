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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Utilities.DicomEditor
{
	/// <summary>
	/// Extension point for views onto <see cref="AnonymizeStudyComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class AnonymizeStudyComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// AnonymizeStudyComponent class.
	/// </summary>
	[AssociateView(typeof(AnonymizeStudyComponentViewExtensionPoint))]
	public class AnonymizeStudyComponent : ApplicationComponent
	{
		private readonly StudyItem _studyItem;

		private string _patientsName;
		private string _patientId;
		private string _accessionNumber;
		private string _studyDescription;
		private DateTime? _dateOfBirth;
		private DateTime? _studyDate;
		private bool _preserveSeriesData;

		internal AnonymizeStudyComponent(StudyItem studyItem)
		{
			_studyItem = studyItem;
		}

		[ValidateNotNull(Message = "MessagePatientsNameCannotBeEmpty")]
		public string PatientsName
		{
			get { return _patientsName; }
			set
			{
				if (_patientsName == value)
					return;
				
				_patientsName = value;
				NotifyPropertyChanged("PatientsName");
			}
		}

		[ValidateNotNull(Message = "MessagePatientIdCannotBeEmpty")]
		public string PatientId
		{
			get { return _patientId; }
			set
			{
				if (_patientId == value)
					return;

				_patientId = value;
				NotifyPropertyChanged("PatientId");
			}
		}

		//[ValidateNotNull(Message = "MessageAccessionNumberCannotBeEmpty")]
		public string AccessionNumber
		{
			get { return _accessionNumber; }
			set
			{
				if (_accessionNumber == value)
					return;

				_accessionNumber = value;
				NotifyPropertyChanged("AccessionNumber");
			}
		}

		public string StudyDescription
		{
			get { return _studyDescription; }
			set
			{
				if (_studyDescription == value)
					return;

				_studyDescription = value;
				NotifyPropertyChanged("StudyDescription");
			}
		}

		public bool PreserveSeriesData
		{
			get { return _preserveSeriesData; }
			set
			{
				if (_preserveSeriesData == value)
					return;

				_preserveSeriesData = value;
				NotifyPropertyChanged("PreserveSeriesDescriptions");
			}
		}

		public DateTime? StudyDate
		{
			get { return _studyDate; }
			set
			{
				if (_studyDate == value)
					return;

				_studyDate = value;
				NotifyPropertyChanged("StudyDate");
			}
		}
	
		public DateTime? DateOfBirth
		{
			get { return _dateOfBirth; }
			set
			{
				if (_dateOfBirth == value)
					return;

				_dateOfBirth = value;
				NotifyPropertyChanged("DateOfBirth");
			}
		}

		public override void Start()
		{
			_patientsName = "Patient^Anonymous";
			_patientId = "PatientId";
			_patientId = "12345";
			_studyDate = Platform.Time;
			_accessionNumber = "A12345";
			_studyDescription = _studyItem.StudyDescription;
			_preserveSeriesData = true;

			_dateOfBirth = DateParser.Parse(_studyItem.PatientsBirthDate);
			if (_dateOfBirth != null)
			{
				_dateOfBirth = new DateTime(_dateOfBirth.Value.Year, 1, 1, 0, 0, 0);
				_dateOfBirth = _dateOfBirth.Value.AddDays(TimeSpan.FromDays(new Random().Next(0, 364)).Days);
			}

			base.Start();
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
			this.ExitCode = ApplicationComponentExitCode.None;
			this.Host.Exit();
		}
	}
}
