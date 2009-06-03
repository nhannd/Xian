using System;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// The exception that is thrown when there is an error is encountered while trying to deserialize DICOM graphics.
	/// </summary>
	public sealed class DicomGraphicsDeserializationException : Exception
	{
		internal DicomGraphicsDeserializationException(string message) : base(message) {}
	}
}