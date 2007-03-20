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
	public class CTImageAnnotationItemProvider : AnnotationItemProvider
	{
		private List<IAnnotationItem> _annotationItems;

		public CTImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.CTImage")
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
								"Dicom.CTImage.KVP",
								this,
								delegate(ImageSop imageSop)
								{
									double val = double.NaN;
									bool tagExists;
									imageSop.GetTag(Dcm.KVP, out val, out tagExists);
									string str = String.Format("{0} kV", val);
									return str;
								},
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.CTImage.XRayTubeCurrent",
								this,
								delegate(ImageSop imageSop)
								{
									int val;
									bool tagExists;
									imageSop.GetTag(Dcm.XRayTubeCurrent, out val, out tagExists);
									string str = String.Format("{0} mA", val);
									return str;
								},
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.CTImage.GantryDetectorTilt",
								this,
								delegate(ImageSop imageSop)
								{
									double val = double.NaN;
									bool tagExists;
									imageSop.GetTag(Dcm.GantryDetectorTilt, out val, out tagExists);
									string str = String.Format("{0}°", val);
									return str;
								},
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.CTImage.ExposureTime",
								this,
								delegate(ImageSop imageSop)
								{
									int val;
									bool tagExists;
									imageSop.GetTag(Dcm.ExposureTime, out val, out tagExists);
									string str = String.Format("{0} ms", val);
									return str;
								},
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.CTImage.ConvolutionKernel",
								this,
								delegate(ImageSop imageSop)
								{
									string val;
									bool tagExists;
									imageSop.GetTag(Dcm.ConvolutionKernel, out val, out tagExists);
									return val;
								},
								DicomBasicResultFormatter.RawStringFormat
							)
						);

				}

				return _annotationItems;
			}
		}
	}
}
