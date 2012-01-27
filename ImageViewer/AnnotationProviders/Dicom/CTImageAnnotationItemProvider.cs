#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
	public class CTImageAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public CTImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.CTImage", new AnnotationResourceResolver(typeof(CTImageAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.CTImage.KVP",
						resolver,
						delegate(Frame frame)
						{
							double value;
							bool tagExists = frame.ParentImageSop[DicomTags.Kvp].TryGetFloat64(0, out value);
							if (tagExists)
								return String.Format(SR.FormatKilovolts, value);

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.CTImage.XRayTubeCurrent",
						resolver,
						f => DXImageAnnotationItemProvider.GetXRayTubeCurrentInMa(f, SR.FormatMilliamps),
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.CTImage.GantryDetectorTilt",
						resolver,
						delegate(Frame frame)
						{
							double value;
							bool tagExists = frame.ParentImageSop[DicomTags.GantryDetectorTilt].TryGetFloat64(0, out value);
							if (tagExists)
								return String.Format(SR.FormatDegrees, value);

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.CTImage.Exposure",
						resolver,
						f => DXImageAnnotationItemProvider.GetExposureInMas(f, SR.FormatMilliampSeconds),
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.CTImage.ExposureTime",
						resolver,
						f => DXImageAnnotationItemProvider.GetExposureTimeInMs(f, SR.FormatMilliseconds),
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.CTImage.ConvolutionKernel",
						resolver,
						delegate(Frame frame)
						{
							string value;
							value = frame.ParentImageSop[DicomTags.ConvolutionKernel].GetString(0, null);
							return value;
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.CTImage.TableSpeed",
						resolver,
						GetTableSpeed,
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.CTImage.TablePosition",
						resolver,
						GetTablePosition,
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.CTImage.Composite.TablePositionSpeed",
						resolver,
						delegate(Frame frame)
						{
							var position = GetTablePosition(frame);
							var speed = GetTableSpeed(frame);
							if (string.IsNullOrEmpty(position))
								return speed;
							if (string.IsNullOrEmpty(speed))
								return position;
							return string.Format(SR.FormatTablePositionSpeed, position, speed);
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}

		private static string GetTableSpeed(Frame frame)
		{
			float value;
			var tagExists = frame.ParentImageSop[DicomTags.TableSpeed].TryGetFloat32(0, out value);
			return tagExists ? string.Format(SR.FormatMillimetersPerSecond, value.ToString("F2")) : string.Empty;
		}

		private static string GetTablePosition(Frame frame)
		{
			var attribute = frame.ParentImageSop[DicomTags.CtPositionSequence];
			var items = attribute.Values as DicomSequenceItem[];
			if (!attribute.IsNull && !attribute.IsEmpty && items != null && items.Length > 0) {
				double value;
				var tagExists = items[0][DicomTags.TablePosition].TryGetFloat64(0, out value);
				return tagExists ? string.Format(SR.FormatMillimeters, value.ToString("F2")) : string.Empty;
			}
			return string.Empty;
		}
	}
}
