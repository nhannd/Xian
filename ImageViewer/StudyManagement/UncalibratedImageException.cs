using System;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Exception indicating that the parent image does not provide pixel spacing information.
	/// </summary>
	public class UncalibratedImageException : Exception
	{
		public UncalibratedImageException() : base("The image does not provide pixel spacing information, nor has it been otherwise calibrated.") {}
	}
}