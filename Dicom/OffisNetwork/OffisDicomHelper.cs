#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom.OffisWrapper;
using MySR = ClearCanvas.Dicom.SR;

namespace ClearCanvas.Dicom.OffisNetwork
{
	/// <summary>
	/// Encapsulates a set of static functions that helps with work on various aspects to
    /// do with the OFFIS DICOM Toolkit.
	/// </summary>
	public static class OffisDicomHelper
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
					throw new DicomException(String.Format(SR.ExceptionDICOMTag, tag.toString(), status.text()));
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
				throw new DicomException(String.Format(SR.ExceptionDICOMFile, filename, status.text()));
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

		public static OFCondition FindAndGetOFString(DcmItem item, DcmTagKey tag, out string value, out bool tagExists)
		{
			OFCondition status = TryFindAndGetOFString(item, tag, out value);
			OffisDicomHelper.CheckReturnValue(status, tag, out tagExists);
			return status;
		}

		public static OFCondition FindAndGetOFString(DcmItem item, DcmTagKey tag, uint position, out string value, out bool tagExists)
		{
			OFCondition status = TryFindAndGetOFString(item, tag, position, out value);
			OffisDicomHelper.CheckReturnValue(status, tag, out tagExists);
			return status;
		}

		public static OFCondition FindAndGetOFStringArray(DcmItem item, DcmTagKey tag, out string valueArray, out bool tagExists)
		{
			OFCondition status = TryFindAndGetOFStringArray(item, tag, out valueArray);
			OffisDicomHelper.CheckReturnValue(status, tag, out tagExists);
			return status;
		}

		public static OFCondition TryFindAndGetOFString(DcmItem item, DcmTagKey tag, out string value)
		{
			value = "";
			int lengthRequired = 0;

			//This returns the length of the entire tag (not just at position 0), but that's ok.  At least it's guaranteed to be big enough.
			OFCondition status = OffisDcm.findAndGetRawStringFromItemGetLength(item, tag, ref lengthRequired, false);
			if (lengthRequired > 0)
			{
				StringBuilder buffer = new StringBuilder(lengthRequired);
				status = item.findAndGetOFString(tag, buffer);
				value = buffer.ToString();
			}

			return status;
		}

		public static OFCondition TryFindAndGetOFString(DcmItem item, DcmTagKey tag, uint position, out string value)
		{
			value = "";
			int lengthRequired = 0;

			//This returns the length of the entire tag (not just at 'position'), but that's ok.  At least it's guaranteed to be big enough.
			OFCondition status = OffisDcm.findAndGetRawStringFromItemGetLength(item, tag, ref lengthRequired, false);
			if (lengthRequired > 0)
			{
				StringBuilder buffer = new StringBuilder(lengthRequired);
				status = item.findAndGetOFString(tag, buffer, position);
				value = buffer.ToString();
			}

			return status;
		}

		public static OFCondition TryFindAndGetOFStringArray(DcmItem item, DcmTagKey tag, out string value)
		{
			value = "";
			int lengthRequired = 0;

			OFCondition status = OffisDcm.findAndGetRawStringFromItemGetLength(item, tag, ref lengthRequired, false);
			if (lengthRequired > 0)
			{
				StringBuilder buffer = new StringBuilder(lengthRequired);
				status = item.findAndGetOFStringArray(tag, buffer);
				value = buffer.ToString();
			}

			return status;
		}
		
		public static OFCondition TryFindAndGetRawStringFromItem(DcmItem dcmItem, DcmTagKey tagKey, out byte[] rawBytes)
        {
            int lengthRequiredOfArray = 0;
			OFCondition status = OffisDcm.findAndGetRawStringFromItemGetLength(dcmItem, tagKey, ref lengthRequiredOfArray, false);

			rawBytes = new byte[lengthRequiredOfArray];
			if (lengthRequiredOfArray > 0)
				status = OffisDcm.findAndGetRawStringFromItem(dcmItem, tagKey, rawBytes, ref lengthRequiredOfArray, false);

			return status;
        }

        public static OFCondition TryGetRawStringFromElement(DcmElement element, out byte[] rawBytes)
        {
            int lengthRequiredOfArray = 0;
            OFCondition status = OffisDcm.getRawStringFromElementGetLength(element, ref lengthRequiredOfArray);

            rawBytes = new byte[lengthRequiredOfArray];
            if (lengthRequiredOfArray > 0)                
                status = OffisDcm.getRawStringFromElement(element, rawBytes, ref lengthRequiredOfArray);

            return status;
        }

        public static OFCondition PutAndInsertRawStringIntoItem(DcmItem dcmItem, DcmTagKey tagKey, byte[] rawBytes)
        {
            return OffisDcm.putAndInsertRawStringIntoItem(dcmItem, tagKey, rawBytes);
        }

	}
}
