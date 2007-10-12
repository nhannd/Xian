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
    /// Encapsulation of a listening port number.
    /// </summary>
    [SerializableAttribute]
    public struct ListeningPort
    {
        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="listeningPort">The listening port as a 32-bit integer.</param>
        public ListeningPort(int listeningPort)
        {
            // validate the input
            if (listeningPort < 1 || listeningPort > int.MaxValue)
                throw new System.ArgumentOutOfRangeException("listeningPort", SR.ExceptionDicomListeningPortOutOfRange);

            _listeningPort = listeningPort;
        }

        /// <summary>
        /// Gets the listening port as a 32-bit integer.
        /// </summary>
        /// <returns>The listening port as a 32-bit integer.</returns>
        public int ToInt()
        {
            return _listeningPort;
        }

        /// <summary>
        /// Implicit cast to return the listen port as an integer for ease of use.
        /// </summary>
        /// <param name="lp">The ListeningPort object to be casted.</param>
        /// <returns>An integer representing the listening port.</returns>
        public static implicit operator int(ListeningPort lp)
        {
            return lp._listeningPort;
        }

        private int _listeningPort;
    }
}
