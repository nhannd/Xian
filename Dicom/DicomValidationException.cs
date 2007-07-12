using System;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// The exception that is thrown when DICOM values fail validation.
	/// </summary>
	public class DicomValidationException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DicomValidationException"/>.
		/// </summary>
		public DicomValidationException() {}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomValidationException"/> with the
		/// specified message.
		/// </summary>
		/// <param name="message"></param>
		public DicomValidationException(string message) : base(message) {}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomValidationException"/> with the
		/// specified message and inner exception.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public DicomValidationException(string message, Exception inner) : base(message, inner) { }
	}
}
