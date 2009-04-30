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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Provides data for Closed events.
    /// </summary>
    public class ClosedEventArgs : EventArgs
    {
        private CloseReason _reason;

        /// <summary>
        /// Constructor.
        /// </summary>
        internal ClosedEventArgs(CloseReason reason)
        {
            _reason = reason;
        }

        /// <summary>
        /// Gets the reason that the object was closed.
        /// </summary>
        public CloseReason Reason
        {
            get { return _reason; }
        }
    }

    /// <summary>
    /// Provides data for ItemClosed events.
    /// </summary>
    /// <typeparam name="TItem">Type of the item that was closed.</typeparam>
    public class ClosedItemEventArgs<TItem> : ItemEventArgs<TItem>
    {
        private CloseReason _reason;

        /// <summary>
        /// Constructor.
        /// </summary>
		internal ClosedItemEventArgs(TItem item, CloseReason reason)
            :base(item)
        {
            _reason = reason;
        }

        /// <summary>
        /// Gets the reason that the item was closed.
        /// </summary>
        public CloseReason Reason
        {
            get { return _reason; }
        }
    }
}
