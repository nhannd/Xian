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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// A simple implementation of ITransactionMonitor.  In reality, ITransactionMonitor should be implemented
    /// by a remote service running on the server, but for the time being there is a simple stub that
    /// exists as part of the client process.  The stub will transmit change set notifications within the client
    /// process only - hence multiple clients will not be aware of each others' changes.
    /// </summary>
    public class TransactionNotifier : ITransactionNotifier
    {
        private Dictionary<Type, EventHandler<EntityChangeEventArgs>> _eventMap;

        internal TransactionNotifier()
        {
            _eventMap = new Dictionary<Type, EventHandler<EntityChangeEventArgs>>();
        }


        #region ITransactionNotifier Members

        public void Subscribe(Type entityType, EventHandler<EntityChangeEventArgs> eventHandler)
        {
            if (_eventMap.ContainsKey(entityType))
            {
                _eventMap[entityType] += eventHandler;
            }
            else
            {
                _eventMap.Add(entityType, eventHandler);
            }
        }

        public void Unsubscribe(Type entityType, EventHandler<EntityChangeEventArgs> eventHandler)
        {
            if (_eventMap.ContainsKey(entityType))
            {
                _eventMap[entityType] -= eventHandler;
            }
        }

        public void Queue(ICollection<EntityChange> changeSet)
        {
            // in a real implementation, the changeSet would be forwarded to some server
            // but for now we just post the changes as local events within this process
            foreach (EntityChange change in changeSet)
            {
                Notify(change);
            }
        }

        #endregion

        private void Notify(EntityChange change)
        {
            foreach (Type eventKeyClass in _eventMap.Keys)
            {
/*                // check if the eventKeyClass is the entity class, or a superclass of it
                if (eventKeyClass.IsAssignableFrom(change.EntityRef))
                {
                    // create an entity ref based on the eventKeyClass, regardless of the actual entity class
                    //EntityRefBase entityRef = EntityRefFactory.CreateReference(eventKeyClass, change.EntityOID, change.Version);
                    //EventsHelper.Fire(_eventMap[change.EntityClass], null, new EntityChangeEventArgs(entityRef, change.ChangeType));
                }
 * */
            }
        }
    }
}
