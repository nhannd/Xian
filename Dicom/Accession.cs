#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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


namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Encapsulation of the Accession Number DICOM tag.
    /// </summary>
    public struct Accession
    {

        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="accession">The Accession Number as a string.</param>
        public Accession(string accession)
        {
            // validate the input
            if (null == accession)
                throw new System.ArgumentNullException("accession", SR.ExceptionDicomAETitleNull);

            //if (0 == accession.Length)
            //    throw new System.ArgumentOutOfRangeException("accession", SR.ExceptionDicomAETitleZeroLength);

            _accession = accession;
        }

        /// <summary>
        /// Retrieves the Accession Number.
        /// </summary>
        /// <returns>Accession Number as a string.</returns>
        public override string ToString()
        {
            return _accession;
        }

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        /// <param name="acc">The Accession object to be casted.</param>
        /// <returns>A String representation of the Accession object.</returns>
        public static implicit operator String(Accession acc)
        {
            return acc.ToString();
        }

        private string _accession;
    }
}
