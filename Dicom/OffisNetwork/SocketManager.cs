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


namespace ClearCanvas.Dicom.OffisNetwork
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClearCanvas.Dicom.OffisWrapper;

    internal class SocketManager
    {
        /// <summary>
        /// Initialize the Winsock library in Windows. In 
        /// non-Windows platforms, this function does nothing via a compiler define.
        /// </summary>
		/// <remarks>
		/// Internally, WSAStartup is used, each call to which (beyond the first one, of course)
		/// simply increments a reference count, and therefore must be paired with 
		/// a call to <see cref="DeinitializeSockets"/>.
		/// </remarks>
        public static void InitializeSockets()
        {
            OffisDcm.InitializeSockets();
        }

        /// <summary>
        /// Deinitialize the Winsock library in Windows. In
        /// non-Windows platforms, this function does nothing.
        /// </summary>
		/// <remarks>
		/// Internally, WSACleanup is used, each call to which simply decrements a reference count.
		/// Consequently, <see cref="InitializeSockets"/> must be called first, and <see cref="DeinitializeSockets"/>
		/// must be called for each call to <see cref="InitializeSockets"/>.
		/// </remarks>
		public static void DeinitializeSockets()
        {
            OffisDcm.DeinitializeSockets();
        }       
    }
}
