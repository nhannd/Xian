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
using System.Text;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class GeneralSeriesAnnotationItemProvider : AnnotationItemProvider
	{
		private List<IAnnotationItem> _annotationItems;

		public GeneralSeriesAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralSeries", new AnnotationResourceResolver(typeof(GeneralSeriesAnnotationItemProvider).Assembly))
		{
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			if (_annotationItems == null)
			{
				_annotationItems = new List<IAnnotationItem>();

				AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

				_annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.BodyPartExamined",
							resolver,
							delegate(ImageSop imageSop) { return imageSop.BodyPartExamined; },
							DicomDataFormatHelper.RawStringFormat
						)
					);

				_annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.Laterality",
							resolver,
							delegate(ImageSop imageSop) { return imageSop.Laterality; },
							DicomDataFormatHelper.RawStringFormat
						)
					);

				_annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.Modality",
							resolver,
							delegate(ImageSop imageSop) { return imageSop.Modality; },
							DicomDataFormatHelper.RawStringFormat
						)
					);

				_annotationItems.Add
					(
						new DicomAnnotationItem<PersonName[]>
						(
							"Dicom.GeneralSeries.OperatorsName",
							resolver,
							delegate(ImageSop imageSop) { return imageSop.OperatorsName; },
							DicomDataFormatHelper.PersonNameListFormatter
						)
					);

				_annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.PerformedProcedureStepDescription",
							resolver,
							SopDataRetrieverFactory.GetStringRetriever(DicomTags.PerformedProcedureStepDescription),
							DicomDataFormatHelper.RawStringFormat
						)
					);

				_annotationItems.Add
					(
						new DicomAnnotationItem<PersonName[]>
						(
							"Dicom.GeneralSeries.PerformingPhysiciansName",
							resolver,
							delegate(ImageSop imageSop) { return imageSop.PerformingPhysiciansName; },
							DicomDataFormatHelper.PersonNameListFormatter
						)
					);

				_annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.ProtocolName",
							resolver,
							SopDataRetrieverFactory.GetStringRetriever(DicomTags.ProtocolName),
							DicomDataFormatHelper.RawStringFormat
						)
					);

				_annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.SeriesDate",
							resolver,
							delegate(ImageSop imageSop) { return imageSop.SeriesDate; },
							DicomDataFormatHelper.DateFormat
						)
					);

				_annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.SeriesTime",
							resolver,
							delegate(ImageSop imageSop) { return imageSop.SeriesTime; },
							DicomDataFormatHelper.TimeFormat
						)
					);

				_annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.SeriesDescription",
							resolver,
							delegate(ImageSop imageSop) { return imageSop.SeriesDescription; },
							DicomDataFormatHelper.RawStringFormat
						)
					);

				_annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.SeriesNumber",
							resolver,
							delegate(ImageSop imageSop)
							{
								string str = String.Format("{0}/{1}",
									imageSop.SeriesNumber,
									imageSop.ParentSeries.ParentStudy.Series.Count);
								return str;
							},
							DicomDataFormatHelper.RawStringFormat
						)
					);
			}

			return _annotationItems;
		}
	}
}
