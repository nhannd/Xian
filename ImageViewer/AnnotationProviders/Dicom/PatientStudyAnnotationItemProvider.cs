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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class PatientStudyAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public PatientStudyAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.PatientStudy", new AnnotationResourceResolver(typeof(PatientStudyAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.PatientStudy.AdditionalPatientsHistory",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.AdditionalPatientsHistory; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.PatientStudy.Occupation",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.Occupation),
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.PatientStudy.PatientsAge",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.PatientsAge),
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<double>
					(
						"Dicom.PatientStudy.PatientsSize",
						resolver,
						FrameDataRetrieverFactory.GetDoubleRetriever(DicomTags.PatientsSize),
						delegate(double input)
						{
							if (double.IsNaN(input) || input == 0)
								return "";

							return String.Format("{0} {1}", input.ToString("F2"), SR.Label_metres);
						}
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<double>
					(
						"Dicom.PatientStudy.PatientsWeight",
						resolver,
						FrameDataRetrieverFactory.GetDoubleRetriever(DicomTags.PatientsWeight),
						delegate(double input)
						{
							if (double.IsNaN(input) || input == 0)
								return "";

							return String.Format("{0} {1}", input.ToString("F2"), SR.Label_kilograms);
						}
					)
				);
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}
	}
}
