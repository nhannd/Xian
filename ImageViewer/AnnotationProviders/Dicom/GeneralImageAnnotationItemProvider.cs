using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

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
								new DicomTagAsStringRetriever(Dcm.ContentDate).GetTagValue,
								DicomBasicResultFormatter.DateFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.ContentTime",
								this,
								new DicomTagAsStringRetriever(Dcm.ContentTime).GetTagValue,
								DicomBasicResultFormatter.TimeFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.DerivationDescription",
								this,
								new DicomTagAsStringRetriever(Dcm.DerivationDescription).GetTagValue,
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
								new DoubleFormatter().FormatList
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralImage.QualityControlImage",
								this,
								new DicomTagAsStringRetriever(Dcm.QualityControlImage).GetTagValue,
								DicomBasicResultFormatter.BooleanFormatter
							)
						);
				}

				return _annotationItems;
			}
		}
	}
}
