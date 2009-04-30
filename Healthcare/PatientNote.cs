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
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// PatientNote
    /// </summary>
    public partial class PatientNote
	{
        /// <summary>
        /// Constructor for creating a new patient note.
        /// </summary>
        /// <param name="author"></param>
        /// <param name="category"></param>
        /// <param name="comment"></param>
        public PatientNote(Staff author, PatientNoteCategory category, string comment)
        {
            _author = author;
            _category = category;
            _comment = comment;

            // valid from now indefinitely
            _creationTime = Platform.Time;
            _validRange = new DateTimeRange(_creationTime, null);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PatientNote"/> is currently valid.
        /// </summary>
        public bool IsCurrent
        {
            get { return this.ValidRange == null || this.ValidRange.Includes(Platform.Time); }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PatientNote"/> has expired.
        /// </summary>
        public bool IsExpired
        {
            get { return _validRange != null && _validRange.Until < Platform.Time; }
        }

        private void CustomInitialize()
        {
        }
    }
}