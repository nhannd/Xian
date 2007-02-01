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
		public GeneralImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralImage")
		{
		}

		protected override List<IAnnotationItem> AnnotationItems
		{
			get
			{
				List<IAnnotationItem> annotationItems = new List<IAnnotationItem>();

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralImage.AcquisitionDate",
							this,
							delegate(ImageSop imageSop) { return imageSop.AcquisitionDate; },
							DicomBasicResultFormatter.DateFormat
						)
					);


				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralImage.AcquisitionTime",
							this,
							delegate(ImageSop imageSop) { return imageSop.AcquisitionTime; },
							DicomBasicResultFormatter.TimeFormat
						)
					);


				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralImage.AcquisitionDateTime",
							this,
							delegate(ImageSop imageSop) { return imageSop.AcquisitionDateTime; },
							DicomBasicResultFormatter.DateTimeFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralImage.AcquisitionNumber",
							this,
							delegate(ImageSop imageSop) { return imageSop.AcquisitionNumber.ToString(); },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralImage.ContentDate",
							this,
							new DicomTagAsStringRetriever(Dcm.ContentDate).GetTagValue,
							DicomBasicResultFormatter.DateFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralImage.ContentTime",
							this,
							new DicomTagAsStringRetriever(Dcm.ContentTime).GetTagValue,
							DicomBasicResultFormatter.TimeFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralImage.DerivationDescription",
							this,
							new DicomTagAsStringRetriever(Dcm.DerivationDescription).GetTagValue,
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralImage.ImageComments",
							this,
							delegate(ImageSop imageSop) { return imageSop.ImageComments; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralImage.ImagesInAcquisition",
							this,
							delegate(ImageSop imageSop) { return imageSop.ImagesInAcquisition.ToString(); },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralImage.ImageType",
							this,
							delegate(ImageSop imageSop) { return imageSop.ImageType; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralImage.InstanceNumber",
							this,
							delegate(ImageSop imageSop) { return imageSop.InstanceNumber.ToString(); },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralImage.LossyImageCompression",
							this,
							delegate(ImageSop imageSop) { return imageSop.LossyImageCompression; },
							DicomBasicResultFormatter.BooleanFormatter
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<double[]>
						(
							"Dicom.GeneralImage.LossyImageCompressionRatio",
							this,
							delegate(ImageSop imageSop) { return imageSop.LossyImageCompressionRatio; },
							new DoubleFormatter().FormatList
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralImage.QualityControlImage",
							this,
							new DicomTagAsStringRetriever(Dcm.QualityControlImage).GetTagValue,
							DicomBasicResultFormatter.BooleanFormatter
						)
					);

				return annotationItems;
			}
		}
	}
}
