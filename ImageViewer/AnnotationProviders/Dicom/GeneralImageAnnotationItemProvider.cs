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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class GeneralImageAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public GeneralImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralImage", new AnnotationResourceResolver(typeof(GeneralImageAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralImage.AcquisitionDate",
						resolver,
						delegate(Frame frame) { return frame.AcquisitionDate; },
						DicomDataFormatHelper.DateFormat
					)
				);


			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralImage.AcquisitionTime",
						resolver,
						delegate(Frame frame) { return frame.AcquisitionTime; },
						delegate (string input)
							{
								if (String.IsNullOrEmpty(input))
									return String.Empty;

								DateTime time;
								if (!TimeParser.Parse(input, out time))
									return input;

								return time.ToString("HH:mm:ss.FFFFFF");
							}
					)
				);


			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralImage.AcquisitionDateTime",
						resolver,
						delegate(Frame frame) { return frame.AcquisitionDateTime; },
						delegate(string input)
							{
								if (String.IsNullOrEmpty(input))
									return String.Empty;

								DateTime dateTime;
								if (!DateTimeParser.Parse(input, out dateTime))
									return input;

								return String.Format("{0} {1}", 
									dateTime.Date.ToString(Format.DateFormat),
									dateTime.ToString("HH:mm:ss.FFFFFF"));
							}
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralImage.AcquisitionNumber",
						resolver,
						delegate(Frame frame) { return frame.AcquisitionNumber.ToString(); },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
						"Dicom.GeneralImage.ContentDate",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.ContentDate),
						DicomDataFormatHelper.DateFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralImage.ContentTime",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.ContentTime),
						DicomDataFormatHelper.TimeFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralImage.DerivationDescription",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.DerivationDescription),
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralImage.ImageComments",
						resolver,
						delegate(Frame frame) { return frame.ImageComments; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralImage.ImagesInAcquisition",
						resolver,
						delegate(Frame frame) { return frame.ImagesInAcquisition.ToString(); },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralImage.ImageType",
						resolver,
						delegate(Frame frame) { return frame.ImageType; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add(new InstanceNumberAnnotationItem());

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralImage.LossyImageCompression",
						resolver,
						delegate(Frame frame)
							{
								bool lossy = false;
								if (!String.IsNullOrEmpty(frame.LossyImageCompression))
								{
									int lossyValue;
									if (Int32.TryParse(frame.LossyImageCompression, out lossyValue) && lossyValue != 0)
										lossy = true;
								}
								else if (frame.LossyImageCompressionRatio != null && frame.LossyImageCompressionRatio.Length > 0)
								{
									lossy = true;
								}

								return lossy ? SR.ValueLossy : "";
							},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<double[]>
					(
						"Dicom.GeneralImage.LossyImageCompressionRatio",
						resolver,
						delegate(Frame frame) { return frame.LossyImageCompressionRatio; },
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
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.QualityControlImage),
						DicomDataFormatHelper.BooleanFormatter
					)
				);

			_annotationItems.Add
				(
					new LateralityViewPositionAnnotationItem
					(
						"Dicom.GeneralImage.ViewPosition",
						false, true
					)
				);

			_annotationItems.Add
				(
					new LateralityViewPositionAnnotationItem
					(
						"Dicom.GeneralImage.ImageLaterality",
						true, false
					)
				);
			
			_annotationItems.Add
				(
					new LateralityViewPositionAnnotationItem
					(
						"Dicom.GeneralImage.Composite.LateralityViewPosition",
						true, true
					)
				);
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}
	}
}
