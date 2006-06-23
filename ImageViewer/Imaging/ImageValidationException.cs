using System;

namespace ClearCanvas.Workstation.Model.Imaging
{
	/// <summary>
	/// Summary description for Exceptions.
	/// </summary>
	public class ImageValidationException : ApplicationException
	{
		public ImageValidationException() {}
		public ImageValidationException(string message) : base(message) {}
		public ImageValidationException(string message, Exception inner) : base(message, inner) {}
	}
}
