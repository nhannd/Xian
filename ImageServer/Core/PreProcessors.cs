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
using ClearCanvas.ImageServer.Core.Edit;

namespace ClearCanvas.ImageServer.Core
{

    /// <summary>
    /// Represents the result of the DICOM file preprocessing.
    /// </summary>
    public class InstancePreProcessingResult
    {
        #region Private Members

        #endregion

        /// <summary>
        /// Indicates whether the file has been updated 
        /// as part of auto-reconciliation process.
        /// </summary>
        public bool AutoReconciled { get; set; }

        /// <summary>
        /// Indicates whether or not the file should be discarded.
        /// </summary>
        public bool DiscardImage { get; set; }

        public bool Modified { get; set; }
    }

    /// <summary>
    /// Represents changes applied to a DICOM
    /// </summary>
    public class UpdateItem
    {
        #region Private Members
        private readonly DicomTag _tag;
        private readonly string _originalValue;
        private readonly string _newValue;
        
        #endregion
        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="UpdateItem"/>
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="originalValue"></param>
        /// <param name="newValue"></param>
        public UpdateItem(uint tag, string originalValue, string newValue)
        {
            _tag = DicomTagDictionary.GetDicomTag(tag);
            _originalValue = originalValue;
            _newValue = newValue;
        }

        /// <summary>
        /// Creates an instance of <see cref="UpdateItem"/>
        /// </summary>
        /// <param name="command"></param>
        /// <param name="file"></param>
        public UpdateItem(IUpdateImageTagCommand command, DicomFile file)
        {
            _tag = command.UpdateEntry.TagPath.Tag;
            _originalValue = file.DataSet[Tag].ToString();
            _newValue = command.UpdateEntry.Value != null ? command.UpdateEntry.Value.ToString() : String.Empty;
        }
        
        #endregion
        #region Public Properties

        /// <summary>
        /// Gets the DICOM tag being updated.
        /// </summary>
        public DicomTag Tag
        {
            get { return _tag; }
        }

        /// <summary>
        /// Gets the original value of the DICOM tag being updated.
        /// </summary>
        public string OriginalValue
        {
            get { return _originalValue; }
        }

        /// <summary>
        /// Gets the new value of the DICOM tag being updated.
        /// </summary>
        public string NewValue
        {
            get { return _newValue; }
        } 
        #endregion
    }
}
