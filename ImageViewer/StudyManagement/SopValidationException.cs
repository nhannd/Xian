using System;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class SopValidationException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SopValidationException"/>.
		/// </summary>
		public SopValidationException() {}

		/// <summary>
		/// Initializes a new instance of <see cref="SopValidationException"/> with the
		/// specified message.
		/// </summary>
		/// <param name="message"></param>
		public SopValidationException(string message) : base(message) {}

		/// <summary>
		/// Initializes a new instance of <see cref="SopValidationException"/> with the
		/// specified message and inner exception.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public SopValidationException(string message, Exception inner) : base(message, inner) { }
	}
}
