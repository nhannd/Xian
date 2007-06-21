using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom.Offis
{
    internal static class OffisHelper
    {
        /// <summary>
        /// Checks if a string is empty.
        /// </summary>
        /// <param name="variable">The string to check.</param>
        /// <param name="variableName">The variable name of the string to checked.</param>
        /// <exception cref="ArgumentNullException"><paramref name="variable"/> or or <paramref name="variableName"/>
        /// is <b>null</b>.</exception>
        /// <exception cref="ArgumentException"><paramref name="variable"/> is zero length.</exception>
        public static void CheckForEmptyString(string variable, string variableName)
        {
            CheckForNullReference(variable, variableName);
            CheckForNullReference(variableName, "variableName");

            if (variable.Length == 0)
                throw new ArgumentException(String.Format(SR.ExceptionEmptyString, variableName));
        }

        /// <summary>
        /// Checks if an object reference is null.
        /// </summary>
        /// <param name="variable">The object reference to check.</param>
        /// <param name="variableName">The variable name of the object reference to check.</param>
        /// <remarks>Use for checking if an input argument is <b>null</b>.  To check if a member variable
        /// is <b>null</b> (i.e., to see if an object is in a valid state), use <see cref="CheckMemberIsSet"/> instead.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="variable"/> or <paramref name="variableName"/>
        /// is <b>null</b>.</exception>
        public static void CheckForNullReference(object variable, string variableName)
        {
            if (variableName == null)
                throw new ArgumentNullException("variableName");

            if (null == variable)
                throw new ArgumentNullException(variableName);
        }
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
        internal static void CheckReturnValue(OFCondition status, DcmTagKey tag, out bool tagExists)
        {
            CheckForNullReference(status, "status");
            CheckForNullReference(tag, "tag");

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
        internal static void CheckReturnValue(OFCondition status, string filename)
        {
            CheckForNullReference(status, "status");
            CheckForNullReference(filename, "filename");

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
        internal static bool CompareConditions(OFCondition condition1, OFCondition condition2)
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
        internal static String NormalizeDirectory(String directory)
        {
            if (null == directory)
                throw new System.ArgumentNullException("directory", SR.ExceptionDicomSaveDirectoryNull);

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
                message.AppendFormat(SR.ExceptionDicomSaveDirectoryDoesNotExist, normalizedDirectory.ToString());

                throw new System.ArgumentException(message.ToString(), "directory");
            }

            return normalizedDirectory.ToString();
        }

        internal static OFCondition FindAndGetRawStringFromItem(DcmItem dcmItem, DcmTagKey tagKey, out byte[] rawBytes)
        {
            int lengthRequiredOfArray = 0;
            OffisDcm.findAndGetRawStringFromItemGetLength(dcmItem, tagKey, ref lengthRequiredOfArray, false);

            rawBytes = new byte[lengthRequiredOfArray];
            OFCondition cond = OffisDcm.findAndGetRawStringFromItem(dcmItem, tagKey, rawBytes, ref lengthRequiredOfArray, false);
            return cond;
        }

        internal static E_TransferSyntax ConvertTransferSyntax(TransferSyntax syntax)
        {

            if (syntax.Uid.Equals(TransferSyntax.ImplicitVRLittleEndian))
                return E_TransferSyntax.EXS_LittleEndianImplicit;
            else if (syntax.Uid.Equals(TransferSyntax.ExplicitVRLittleEndian))
                return E_TransferSyntax.EXS_LittleEndianExplicit;
            else if (syntax.Uid.Equals(TransferSyntax.ExplicitVRBigEndian))
                return E_TransferSyntax.EXS_BigEndianExplicit;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGBaselineProcess1))
                return E_TransferSyntax.EXS_JPEGProcess1TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGExtendedProcess24))
                return E_TransferSyntax.EXS_JPEGProcess2_4TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGExtendedProcess35Retired))
                return E_TransferSyntax.EXS_JPEGProcess3_5TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGSpectralSelectionNonHierarchicalProcess68Retired))
                return E_TransferSyntax.EXS_JPEGProcess6_8TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGSpectralSelectionNonHierarchicalProcess79Retired))
                return E_TransferSyntax.EXS_JPEGProcess7_9TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGFullProgressionNonHierarchicalProcess1012Retired))
                return E_TransferSyntax.EXS_JPEGProcess10_12TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGFullProgressionNonHierarchicalProcess1113Retired))
                return E_TransferSyntax.EXS_JPEGProcess11_13TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGLosslessNonHierarchicalProcess14))
                return E_TransferSyntax.EXS_JPEGProcess14TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGLosslessNonHierarchicalProcess15Retired))
                return E_TransferSyntax.EXS_JPEGProcess15TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGExtendedHierarchicalProcess1618Retired))
                return E_TransferSyntax.EXS_JPEGProcess16_18TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGExtendedHierarchicalProcess1719Retired))
                return E_TransferSyntax.EXS_JPEGProcess17_19TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGSpectralSelectionHierarchicalProcess2022Retired))
                return E_TransferSyntax.EXS_JPEGProcess20_22TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGSpectralSelectionHierarchicalProcess2123Retired))
                return E_TransferSyntax.EXS_JPEGProcess21_23TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGFullProgressionHierarchicalProcess2426Retired))
                return E_TransferSyntax.EXS_JPEGProcess24_26TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGFullProgressionHierarchicalProcess2527Retired))
                return E_TransferSyntax.EXS_JPEGProcess25_27TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGLosslessHierarchicalProcess28Retired))
                return E_TransferSyntax.EXS_JPEGProcess28TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGLosslessHierarchicalProcess29Retired))
                return E_TransferSyntax.EXS_JPEGProcess29TransferSyntax;
            else if (syntax.Uid.Equals(TransferSyntax.RLELossless))
                return E_TransferSyntax.EXS_RLELossless;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGLSLosslessImageCompression))
                return E_TransferSyntax.EXS_JPEGLSLossless;
            else if (syntax.Uid.Equals(TransferSyntax.JPEGLSLossyNearLosslessImageCompression))
                return E_TransferSyntax.EXS_JPEGLSLossy;
            else if (syntax.Uid.Equals(TransferSyntax.DeflatedExplicitVRLittleEndian))
                return E_TransferSyntax.EXS_DeflatedLittleEndianExplicit;
            else if (syntax.Uid.Equals(TransferSyntax.JPEG2000ImageCompressionLosslessOnly))
                return E_TransferSyntax.EXS_JPEG2000LosslessOnly;
            else if (syntax.Uid.Equals(TransferSyntax.JPEG2000ImageCompression))
                return E_TransferSyntax.EXS_JPEG2000;
            else if (syntax.Uid.Equals(TransferSyntax.MPEG2MainProfileMainLevel))
                return E_TransferSyntax.EXS_MPEG2MainProfileAtMainLevel;
            else if (syntax.Uid.Equals(TransferSyntax.JPEG2000Part2MulticomponentImageCompressionLosslessOnly))
                return E_TransferSyntax.EXS_JPEG2000MulticomponentLosslessOnly;
            else if (syntax.Uid.Equals(TransferSyntax.JPEG2000Part2MulticomponentImageCompression))
                return E_TransferSyntax.EXS_JPEG2000Multicomponent;
         
            return E_TransferSyntax.EXS_Unknown;
        }
    }
}
