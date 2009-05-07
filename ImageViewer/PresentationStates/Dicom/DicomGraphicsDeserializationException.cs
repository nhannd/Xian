using System;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	internal sealed class DicomGraphicsDeserializationException : Exception
	{
		public DicomGraphicsDeserializationException(string message) : base(message) {}
	}
}