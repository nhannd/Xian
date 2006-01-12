using System;
using ClearCanvas.Common;

namespace ClearCanvas.Common.Dicom
{
	/// <summary>
	/// Summary description for Dicom.
	/// </summary>
	public class DicomHelper
	{
		public static void CheckReturnValue(OFCondition status, DcmTagKey tag, out bool tagExists)
		{
			Platform.CheckForNullReference(status, "status");
			Platform.CheckForNullReference(tag, "tag");

			if (status.bad())
			{
				// Status code 2 = Tag not found
				// This is not an exceptional situation (and in fact can occur often), 
				// so don't throw an exception, but set tagExists to false.
				if (status.code() == 2)
					tagExists = false;
				else
					throw new DicomException(String.Format(SR.ExceptionDICOMTag, tag.toString(), status.text()));
			}
			else
			{
				tagExists = true;
			}
		}

		public static void CheckReturnValue(OFCondition status, string filename)
		{
			Platform.CheckForNullReference(status, "status");
			Platform.CheckForNullReference(filename, "filename");

			if (status.bad())
				throw new DicomException(String.Format(SR.ExceptionDICOMFile, filename, status.text()));
		}
	}
}
