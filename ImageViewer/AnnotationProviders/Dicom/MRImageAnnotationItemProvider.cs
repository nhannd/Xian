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
	public class MRImageAnnotationItemProvider : AnnotationItemProvider
	{
		private List<IAnnotationItem> _annotationItems;

		public MRImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.MRImage")
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
								"Dicom.MRImage.EchoTime",
								this,
								delegate(ImageSop imageSop)
								{
									double val = double.NaN;
									bool tagExists;
									imageSop.GetTag(Dcm.EchoTime, out val, out tagExists);
									string str = String.Format("{0:F2} ms", val);
									return str;
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
