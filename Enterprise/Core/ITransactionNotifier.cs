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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Specifies the inteface to the transaction monitor.  This is very preliminary and will likely
    /// change.
    /// </summary>
    public interface ITransactionNotifier
    {
        /// <summary>
        /// Allows a client to subscribe for change notifications for a given type of entity.
        /// It is extremely important that clients explicitly unsubscribe in order that resources
        /// are properly released.
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="eventHandler"></param>
        void Subscribe(Type entityType, EventHandler<EntityChangeEventArgs> eventHandler);

        /// <summary>
        /// Allows a client to unsubscribe from change notifications for a given type of entity.
        /// It is extremely important that clients explicitly unsubscribe in order that resources
        /// are properly released.
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="eventHandler"></param>
        void Unsubscribe(Type entityType, EventHandler<EntityChangeEventArgs> eventHandler);

        /// <summary>
        /// Allows a client to queue a set of changes for notification to other clients.
        /// </summary>
        /// <param name="changeSet"></param>
        void Queue(ICollection<EntityChange> changeSet);
    }
}
