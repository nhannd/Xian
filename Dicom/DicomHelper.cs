namespace ClearCanvas.Dicom
{
    using System;
    using ClearCanvas.Common;
    using ClearCanvas.Dicom.OffisWrapper;
    using ClearCanvas.Dicom;
    using MySR = ClearCanvas.Dicom.SR;
    using System.Text;

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

        public static bool CompareConditions(OFCondition condition1, OFCondition condition2)
        {
            return (condition1.code() == condition2.code() && condition1.module() == condition2.module());
        }

        /// <summary>
        /// Utility function that checks for the validity of a directory path
        /// that is passed in. It will also return the "normalized" path. Right now,
        /// that just means there will be a trailing backslash appended, which is the
        /// correct denotation for a directory.
        /// </summary>
        /// <param name="directory">Directory path to check and normalize.</param>
        /// <returns>Normalized directory path.</returns>
        public static String NormalizeDirectory(String directory)
        {
            if (null == directory)
                throw new System.ArgumentNullException("directory", MySR.ExceptionDicomSaveDirectoryNull);

            // make sure that the path passed in has a trailing backslash 
            StringBuilder normalizedDirectory = new StringBuilder();
            if (directory.EndsWith("\\"))
            {
                normalizedDirectory.Append(directory);
            }
            else
            {
                normalizedDirectory.AppendFormat("{0}\\", directory);
            }

            // contract check: existence of saveDirectory
            if (!System.IO.Directory.Exists(normalizedDirectory.ToString()))
            {
                StringBuilder message = new StringBuilder();
                message.AppendFormat(MySR.ExceptionDicomSaveDirectoryDoesNotExist, normalizedDirectory.ToString());

                throw new System.ArgumentException(message.ToString(), "directory");
            }

            return normalizedDirectory.ToString();
        }

        private DicomHelper() { }
	}
}
