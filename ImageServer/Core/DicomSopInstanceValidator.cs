#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Core
{
    /// <summary>
    /// Represent a class to validate a DICOM message. Used by <see cref="SopInstanceImporter"/>.
    /// Note: This class does not perform DICOM conformance validation.
    /// </summary>
    internal class DicomSopInstanceValidator
    {
        /// <summary>
        /// Validates the contents in the <see cref="DicomMessageBase"/> object.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="DicomDataException"/> is thrown if the DICOM object fails the validation.
        public void Validate(DicomMessageBase message)
        {
            String studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);
            String seriesInstanceUid = message.DataSet[DicomTags.SeriesInstanceUid].GetString(0, string.Empty);
            String sopInstanceUid = message.DataSet[DicomTags.SopInstanceUid].GetString(0, string.Empty);

            if (String.IsNullOrEmpty(studyInstanceUid))
            {
                throw new DicomDataException("Study Instance UID is missing or empty");
            }

            if (studyInstanceUid.Length > 64 || seriesInstanceUid.Length > 64 || sopInstanceUid.Length > 64)
            {
                if (studyInstanceUid.Length > 64)
                    throw new DicomDataException(string.Format("Study Instance UID is > 64 bytes in the SOP Instance : {0}", studyInstanceUid));

                else if (seriesInstanceUid.Length > 64)
                    throw new DicomDataException(string.Format("Series Instance UID is > 64 bytes in the SOP Instance : {0}", seriesInstanceUid));
                else
                    throw new DicomDataException(string.Format("SOP Instance UID is > 64 bytes in the SOP Instance : {0}", sopInstanceUid));
            }
        }
    }
}