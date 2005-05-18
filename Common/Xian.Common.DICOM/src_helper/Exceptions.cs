using System;

namespace Xian.Common.DICOM
{
	/// <summary>
	/// Summary description for DICOMException.
	/// </summary>
	public class DicomException : ApplicationException
	{
		public DicomException() {}
		public DicomException(string message) : base(message) {}
		public DicomException(string message, Exception inner) : base(message, inner) {}
	}
}
