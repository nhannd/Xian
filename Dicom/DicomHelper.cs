using System;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;
using MySR = ClearCanvas.Dicom.SR;

namespace ClearCanvas.Dicom
{
	/// <summary>
	/// Encapsulates a set of static functions that helps with work on various aspects to
    /// do with the OFFIS DICOM Toolkit.
	/// </summary>
	public class DicomHelper
	{
        /// <summary>
        /// Check the OFCondition object that is returned from many of the OFFIS functions/methods.
        /// Used in the extraction of a DICOM tag. If the condition is that the tag does not exist,
        /// this will not be considered an exception, and execution will proceed normally.
        /// </summary>
        /// <exception cref="GeneralDicomException">GeneralDicomException</exception>
        /// <param name="status">The condition object that will be checked.</param>
        /// <param name="tag">The tag that was to be extracted.</param>
        /// <param name="tagExists">Output argument that will indicate whether or not
        /// the tag that was to be extracted exists in the dataset.</param>
		public static void CheckReturnValue(OFCondition status, DcmTagKey tag, out bool tagExists)
		{
			Platform.CheckForNullReference(status, "status");
			Platform.CheckForNullReference(tag, "tag");

			if (status.bad())
			{
				// Status code 1 - invalid tag
				// Usually happens when a tag is queried that exists, but has an empty value.
				// This is not a particularly exceptional situation, since we mostly only
				// care whether or not the tag has a value.  If the tag is 'invalid', regardless
				// of the reason, set tagExists to false and return.
				
				// Status code 2 = Tag not found
				// This is not an exceptional situation (and in fact can occur often), 
				// so don't throw an exception, but set tagExists to false.

				//NOTE: The rest of the return codes *ARE* exceptional situations.

				if (status.code() == 1 || status.code() == 2)
					tagExists = false;
				else
					throw new GeneralDicomException(String.Format(SR.ExceptionDICOMTag, tag.toString(), status.text()));
			}
			else
			{
				tagExists = true;
			}
		}

        /// <summary>
        /// Checks the condition returned when loading a DICOM file. If there was a problem
        /// in loading the file, an exception will be thrown.
        /// </summary>
        /// <exception cref="GeneralDicomExceptoin">GeneralDicomException</exception>
        /// <param name="status">The condition object to be checked.</param>
        /// <param name="filename">The filename that was to be loaded.</param>
		public static void CheckReturnValue(OFCondition status, string filename)
		{
			Platform.CheckForNullReference(status, "status");
			Platform.CheckForNullReference(filename, "filename");

			if (status.bad())
				throw new GeneralDicomException(String.Format(SR.ExceptionDICOMFile, filename, status.text()));
		}

        /// <summary>
        /// Compares one OFCondition condition object with another for semantic equality. This was created
        /// because the static OFCondition objects that the OFFIS toolkit creates when the library is 
        /// initialized cannot be used as-is in the C# world. For example, DUL_PEERREQUESTEDRELEASE is 
        /// a OFCondition object that can be compared to condition objects returned from certain network
        /// functions. The object, however, cannot be compared directly, since the addresses where they
        /// reside in the C++ world and C# world are different. This function compares their semantic
        /// meanings for equality.
        /// </summary>
        /// <param name="condition1">The first condition object.</param>
        /// <param name="condition2">The second condition object.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Takes in a string that contains a DICOM DT value
        /// and returns it in the format of the current user-configured
        /// culture. This function assumes that the DT value is formatted
        /// correctly, i.e. in the ANSI format of YYYYMMDD
        /// </summary>
        public static string ConvertFromDicomDA(string dicomDAValue)
        {
            if (dicomDAValue.Length < 8)
                return dicomDAValue;

            DateTime newDate = new DateTime(
                Convert.ToInt32(dicomDAValue.Substring(0, 4)),
                Convert.ToInt32(dicomDAValue.Substring(4, 2)),
                Convert.ToInt32(dicomDAValue.Substring(6, 2))
                );

            string[] formats = newDate.GetDateTimeFormats(Thread.CurrentThread.CurrentCulture);
            return formats[0];
        }

        private DicomHelper() { }
    }
}
