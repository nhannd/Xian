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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public interface IDicomImageSetDescriptor : IImageSetDescriptor
	{
		IStudyRootStudyIdentifier SourceStudy { get; }
	}

	public class DicomImageSetDescriptor : ImageSetDescriptor, IDicomImageSetDescriptor
	{
		private readonly IStudyRootStudyIdentifier _sourceStudy;

		private string _name;
		private string _patientInfo;
		private string _uid;

		public DicomImageSetDescriptor(IStudyRootStudyIdentifier sourceStudy)
		{
			Platform.CheckForNullReference(sourceStudy, "sourceStudy");
			_sourceStudy = sourceStudy;
		}

		public IStudyRootStudyIdentifier SourceStudy
		{
			get { return _sourceStudy; }
		}

		public override string Name
		{
			get
			{
				if (_name == null)
					_name = GetName() ?? "";
				return _name;
			}
			set { throw new InvalidOperationException("The Name property cannot be set publicly."); }
		}

		public override string PatientInfo
		{
			get
			{
				if (_patientInfo == null)
					_patientInfo = GetPatientInfo() ?? "";
				return _patientInfo;
			}
			set { throw new InvalidOperationException("The PatientInfo property cannot be set publicly."); }
		}

		public override string Uid
		{
			get
			{
				if (_uid == null)
					_uid = GetUid() ?? "";
				return _uid;
			}
			set { throw new InvalidOperationException("The Uid property cannot be set publicly."); }
		}

		protected virtual string GetName()
		{
			DateTime studyDate;
			DateParser.Parse(_sourceStudy.StudyDate, out studyDate);
			DateTime studyTime;
			TimeParser.Parse(_sourceStudy.StudyTime, out studyTime);

			string modalitiesInStudy = StringUtilities.Combine(_sourceStudy.ModalitiesInStudy, ", ");

			return String.Format("{0} {1} [{2}] {3}",
										  studyDate.ToString(Format.DateFormat),
										  studyTime.ToString(Format.TimeFormat),
										  modalitiesInStudy ?? "",
										  _sourceStudy.StudyDescription);
		}

		protected virtual string GetPatientInfo()
		{
			return String.Format("{0} � {1}", new PersonName(_sourceStudy.PatientsName).FormattedName, _sourceStudy.PatientId);
		}

		protected virtual string GetUid()
		{
			return _sourceStudy.StudyInstanceUid;
		}
	}

	public class BasicImageSetDescriptor : ImageSetDescriptor
	{
		private string _name;
		private string _patientInfo;
		private string _uid;

		public BasicImageSetDescriptor()
		{}

		public override string Name
		{
			get { return _name ?? ""; }
			set { _name = value; }
		}

		public override string PatientInfo
		{
			get { return _patientInfo ?? ""; }
			set { _patientInfo = value; }
		}

		public override string Uid
		{
			get { return _uid ?? ""; }
			set { _uid = value; }
		}
	}

	public interface IImageSetDescriptor
	{
		IImageSet ImageSet { get; }

		string Name { get; }
		string PatientInfo { get; }
		string Uid { get; }
	}

	public abstract class ImageSetDescriptor : IImageSetDescriptor
	{
		private ImageSet _imageSet;

		protected ImageSetDescriptor()
		{
		}

		#region IImageSetDescriptor Members

		IImageSet IImageSetDescriptor.ImageSet
		{
			get { return _imageSet; }	
		}

		public virtual ImageSet ImageSet
		{
			get { return _imageSet; }
			internal set { _imageSet = value; }
		}

		public abstract string Name { get; set; }

		public abstract string PatientInfo { get; set; }

		public abstract string Uid { get; set; }

		#endregion

		public override string ToString()
		{
			return StringUtilities.Combine(new string[] { PatientInfo, Name, Uid }, " | ", true);
		}
	}
}
