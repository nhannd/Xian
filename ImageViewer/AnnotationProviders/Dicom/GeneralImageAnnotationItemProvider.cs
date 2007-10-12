#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class GeneralImageAnnotationItemProvider : AnnotationItemProvider
	{
		private List<IAnnotationItem> _annotationItems;

		public GeneralImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralImage")
		{
		}

		protected override List<IAnnotationItem> AnnotationItems
		{
			get
			{
				if (_annotationItems == null)
				{
					_annotationItems = new List<IAnnotationItem>();

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.AcquisitionDate",
								this,
								delegate(ImageSop imageSop) { return imageSop.AcquisitionDate; },
								DicomBasicResultFormatter.DateFormat
							)
						);


					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.AcquisitionTime",
								this,
								delegate(ImageSop imageSop) { return imageSop.AcquisitionTime; },
								DicomBasicResultFormatter.TimeFormat
							)
						);


					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.AcquisitionDateTime",
								this,
								delegate(ImageSop imageSop) { return imageSop.AcquisitionDateTime; },
								DicomBasicResultFormatter.DateTimeFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.AcquisitionNumber",
								this,
								delegate(ImageSop imageSop) { return imageSop.AcquisitionNumber.ToString(); },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.ContentDate",
								this,
								new DicomTagAsStringRetriever(DicomTags.ContentDate).GetTagValue,
								DicomBasicResultFormatter.DateFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.ContentTime",
								this,
								new DicomTagAsStringRetriever(DicomTags.ContentTime).GetTagValue,
								DicomBasicResultFormatter.TimeFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.DerivationDescription",
								this,
								new DicomTagAsStringRetriever(DicomTags.DerivationDescription).GetTagValue,
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.ImageComments",
								this,
								delegate(ImageSop imageSop) { return imageSop.ImageComments; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.ImagesInAcquisition",
								this,
								delegate(ImageSop imageSop) { return imageSop.ImagesInAcquisition.ToString(); },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.ImageType",
								this,
								delegate(ImageSop imageSop) { return imageSop.ImageType; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.InstanceNumber",
								this,
								delegate(ImageSop imageSop)
								{
									string str = String.Format("{0}/{1}",
										imageSop.InstanceNumber,
										imageSop.ParentSeries.Sops.Count);
									return str;
								},
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.LossyImageCompression",
								this,
								delegate(ImageSop imageSop) { return imageSop.LossyImageCompression; },
								DicomBasicResultFormatter.BooleanFormatter
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<double[]>
							(
								"Dicom.GeneralImage.LossyImageCompressionRatio",
								this,
								delegate(ImageSop imageSop) { return imageSop.LossyImageCompressionRatio; },
								delegate(double[] values) 
								{
									return StringUtilities.Combine<double>(values, ",\n",
										delegate(double value) { return value.ToString("F2"); });
								}
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.QualityControlImage",
								this,
								new DicomTagAsStringRetriever(DicomTags.QualityControlImage).GetTagValue,
								DicomBasicResultFormatter.BooleanFormatter
							)
						);
				}

				return _annotationItems;
			}
		}
	}
}
