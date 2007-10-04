using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;
using System.Diagnostics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Annotations.Dicom
{
	internal sealed class DicomFilteredAnnotationLayoutProvider : StoredAnnotatationLayoutProvider
	{
		private IImageSopProvider _image;

		public DicomFilteredAnnotationLayoutProvider(IImageSopProvider image)
		{
			Platform.CheckForNullReference(image, "image");
			_image = image;
		}

		protected override string StoredLayoutId
		{
			get 
			{
				string layoutId = DicomFilteredAnnotationLayoutStore.Instance.GetMatchingStoredLayoutId(_image);
				//Trace.WriteLine(String.Format("Layout Id: {0}", layoutId));
				return layoutId;
			}
		}
	}
}
