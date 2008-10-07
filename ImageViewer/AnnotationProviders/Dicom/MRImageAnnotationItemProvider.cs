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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class MRImageAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public MRImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.MRImage", new AnnotationResourceResolver(typeof(MRImageAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.EchoTime",
						resolver,
						delegate(Frame frame)
						{
							double value;
							bool tagExists = frame.ParentImageSop[DicomTags.EchoTime].TryGetFloat64(0, out value);
							if (tagExists)
								return String.Format(SR.Formatms, value);

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.MagneticFieldStrength",
						resolver,
						delegate(Frame frame)
						{
							double value;
							bool tagExists = frame.ParentImageSop[DicomTags.MagneticFieldStrength].TryGetFloat64(0, out value);
							if (tagExists)
								return String.Format(SR.FormatTeslas, value);

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.AcquisitionMatrix",
						resolver,
						delegate(Frame frame)
						{
							DicomAttribute acqAttrib = frame.ParentImageSop[DicomTags.AcquisitionMatrix];
							if (!acqAttrib.IsEmpty && acqAttrib.Count > 3)
							{
								ushort frequencyRows, frequencyColumns, phaseRows, phaseColumns;

								frequencyRows = acqAttrib.GetUInt16(0, 0);
								frequencyColumns = acqAttrib.GetUInt16(1, 0);
								phaseRows = acqAttrib.GetUInt16(2, 0);
								phaseColumns = acqAttrib.GetUInt16(3, 0);

								return String.Format("{0} x {1}", frequencyColumns, phaseRows);
							}

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.ReceiveCoilName",
						resolver,
						delegate(Frame frame)
						{
							string value;
							value = frame.ParentImageSop[DicomTags.ReceiveCoilName].GetString(0, null);
							return value;
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.RepetitionTime",
						resolver,
						delegate(Frame frame)
						{
							double value;
							bool tagExists = frame.ParentImageSop[DicomTags.RepetitionTime].TryGetFloat64(0, out value);
							if (tagExists)
								return String.Format(SR.Formatms, value);

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.EchoTrainLength",
						resolver,
						delegate(Frame frame)
						{
							int value;
							bool tagExists = frame.ParentImageSop[DicomTags.EchoTrainLength].TryGetInt32(0, out value);
							if (tagExists)
								return String.Format("{0}", value);

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}
	}
}