using System;

namespace ClearCanvas.Common.DICOM
{
	/// <summary>
	/// Summary description for Dicom.
	/// </summary>
	public class Dicom
	{
		public static void CheckReturnValue(OFCondition status, DcmTagKey tag, out bool tagExists)
		{
			if (status == null)
				throw new NullReferenceException(SR.ExceptionNullReference("status"));

			if (tag == null)
				throw new NullReferenceException(SR.ExceptionNullReference("tag"));

			if (status.bad())
			{
				// Status code 2 = Tag not found
				// This is not an exceptional situation (and in fact can occur often), 
				// so don't throw an exception, but set tagExists to false.
				if (status.code() == 2)
					tagExists = false;
				else
					throw new DicomException(SR.ExceptionDICOMTag(tag.toString(), status.text()));
			}
			else
			{
				tagExists = true;
			}
		}

		public static void CheckReturnValue(OFCondition status, string filename)
		{
			if (status == null)
				throw new NullReferenceException(SR.ExceptionNullReference("status"));

			if (filename == null)
				throw new NullReferenceException(SR.ExceptionNullReference("filename"));

			if (status.bad())
				throw new DicomException(SR.ExceptionDICOMFile(filename, status.text()));
		}
	}
}
