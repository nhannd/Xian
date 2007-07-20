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
	public class ImagePlaneAnnotationItemProvider : AnnotationItemProvider
	{
		private List<IAnnotationItem> _annotationItems;

		public ImagePlaneAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.ImagePlane")
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
								"Dicom.ImagePlane.SliceThickness",
								this,
								delegate(ImageSop imageSop)
								{
									return String.Format("{0:F1} mm", imageSop.SliceThickness);
								},
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
					(
						new DicomAnnotationItem<string>
							(
								"Dicom.ImagePlane.SliceLocation",
								this,
								delegate(ImageSop imageSop)
								{
									return String.Format("{0:F1} mm", imageSop.SliceLocation);
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
