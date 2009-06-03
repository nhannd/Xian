using System;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	public sealed class DicomGraphicsDeserializationException : Exception
	{
		internal DicomGraphicsDeserializationException(string message) : base(message) {}
	}
}