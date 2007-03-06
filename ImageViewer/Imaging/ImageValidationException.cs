using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// The exception that is thrown when a DICOM image fails validation.
	/// </summary>
	public class ImageValidationException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ImageValidationException"/>.
		/// </summary>
		public ImageValidationException() {}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageValidationException"/> with the
		/// specified message.
		/// </summary>
		/// <param name="message"></param>
		public ImageValidationException(string message) : base(message) {}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageValidationException"/> with the
		/// specified message and inner exception.
		/// </summary>
		/// <param name="message"></param>
		public ImageValidationException(string message, Exception inner) : base(message, inner) { }
	}
}
