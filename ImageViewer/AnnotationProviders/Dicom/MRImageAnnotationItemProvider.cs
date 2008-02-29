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
	public class MRImageAnnotationItemProvider : AnnotationItemProvider
	{
		private List<IAnnotationItem> _annotationItems;

		public MRImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.MRImage", new AnnotationResourceResolver(typeof(MRImageAnnotationItemProvider).Assembly))
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
							"Dicom.MRImage.EchoTime",
							resolver,
							delegate(Frame frame)
							{
								double val = double.NaN;
								bool tagExists;
								frame.ParentImageSop.GetTag(DicomTags.EchoTime, out val, out tagExists);
								string str = String.Format(SR.Formatms, val);
								return str;
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
								double val = double.NaN;
								bool tagExists;
								frame.ParentImageSop.GetTag(DicomTags.MagneticFieldStrength, out val, out tagExists);

								double strengthInTeslas = val / 10000;
								string str = String.Format(SR.FormatTeslas, strengthInTeslas);
								return str;
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
								ushort frequencyRows, frequencyColumns, phaseRows, phaseColumns;
								bool tagExists;
								frame.ParentImageSop.GetTag(DicomTags.AcquisitionMatrix, out frequencyRows, 0, out tagExists);
								frame.ParentImageSop.GetTag(DicomTags.AcquisitionMatrix, out frequencyColumns, 1, out tagExists);
								frame.ParentImageSop.GetTag(DicomTags.AcquisitionMatrix, out phaseRows, 2, out tagExists);
								frame.ParentImageSop.GetTag(DicomTags.AcquisitionMatrix, out phaseColumns, 3, out tagExists);

								// Just include freq for now
								string str = String.Format("{0} x {1}", frequencyColumns, phaseRows);
								return str;
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
								string val;
								bool tagExists;
								frame.ParentImageSop.GetTag(DicomTags.ReceiveCoilName, out val, out tagExists);
								return val;
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
								double val = double.NaN;
								bool tagExists;
								frame.ParentImageSop.GetTag(DicomTags.RepetitionTime, out val, out tagExists);
								string str = String.Format(SR.Formatms, val);
								return str;
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
								int val;
								bool tagExists;
								frame.ParentImageSop.GetTag(DicomTags.EchoTrainLength, out val, out tagExists);
								string str = String.Format("{0}", val);
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