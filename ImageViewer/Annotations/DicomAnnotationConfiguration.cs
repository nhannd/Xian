using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations
{
	public class DicomAnnotationConfiguration : AnnotationConfiguration
	{
		private string _modality;
		private static readonly string _identifierPrefix = "DICOM.";

		public DicomAnnotationConfiguration(string modality, AnnotationLayout annotationLayout)
			: base(_identifierPrefix + modality, modality, annotationLayout)
		{
			_modality = modality;
		}

		protected override bool MeetsCriteria(PresentationImage presentationImage)
		{
			DicomPresentationImage dicomImage = (DicomPresentationImage)presentationImage;
			if (dicomImage == null)
				return false;

			try
			{
				if (dicomImage.ImageSop.Modality == _modality)
					return true;
			}
			catch (Exception e)
			{
				Platform.Log(e);
			}

			try
			{
				string modality = String.Empty;
				bool tagExists = false;
				dicomImage.ImageSop.GetTag(Dcm.Modality, out modality, out tagExists);
				if (modality == _modality)
					return true;
			}
			catch (Exception e)
			{
				Platform.Log(e);
			}

			return false;
		}
	}
}
