using System;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	//TODO (CR May09): public with internal constructor?
	internal sealed class DicomGraphicsDeserializationException : Exception
	{
		public DicomGraphicsDeserializationException(string message) : base(message) {}
	}
}