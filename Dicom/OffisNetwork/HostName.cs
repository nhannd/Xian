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
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.OffisNetwork
{
    /// <summary>
    /// Encapsulation of a hostname.
    /// </summary>
    [SerializableAttribute]
    public struct HostName
    {
        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="hostName">The Hostname as a string.</param>
        public HostName(string hostName)
        {
            // validate the input
            if (null == hostName)
                throw new System.ArgumentNullException("hostName", SR.ExceptionDicomHostnameNull);

            if (0 == hostName.Length)
                throw new System.ArgumentOutOfRangeException("hostName", SR.ExceptionDicomHostnameZeroLength);

            // todo: if input is an ip address, ensure that the format is correct

            // todo: if input is an alphanumeric name, ensure that the format is correct

            // validation complete
            _hostName = hostName;
        }

        /// <summary>
        /// Gets the Hostname as a string.
        /// </summary>
        /// <returns>The hostname in String form.</returns>
        public override string ToString()
        {
            return _hostName;
        }

        /// <summary>
        /// Implicit cast to a String object, for ease of use.
        /// </summary>
        /// <param name="hn">The Hostname object to be casted.</param>
        /// <returns>A String representation of the Hostname object.</returns>
        public static implicit operator String(HostName hn)
        {
            return hn.ToString();
        }

        private string _hostName;
    }
}
