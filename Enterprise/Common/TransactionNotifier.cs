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
using System.ServiceModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common
{
    internal class TransactionNotifier
    {
        // service contract
        [ServiceContract(CallbackContract = typeof(ITransactionNotifier))]
        interface ITransactionNotifier
        {
            [OperationContract(IsOneWay = true)]
            void Notify(EntityChangeSet changeSet);
        }

        class TransactionNotifierService : ITransactionNotifier
        {
            private TransactionNotifier _owner;

            public TransactionNotifierService(TransactionNotifier owner)
            {
                _owner = owner;
            }

            #region ITransactionNotifier Members

            public void Notify(EntityChangeSet changeSet)
            {
                _owner.OnNotify(changeSet);
            }

            #endregion
        }

        interface ITransactionNotifierChannel : ITransactionNotifier, IClientChannel
        {
        }



        private ITransactionNotifierChannel _channel;
        private event EventHandler<EntityChangeEventArgs> _notified;


        public TransactionNotifier()
        {

        }

        public void StartUp()
        {
            // Construct InstanceContext to handle messages on callback interface. 
            // An instance of ChatApp is created and passed to the InstanceContext.
            InstanceContext instanceContext = new InstanceContext(new TransactionNotifierService(this));

            // Create the participant with the given endpoint configuration
            // Each participant opens a duplex channel to the mesh
            // participant is an instance of the chat application that has opened a channel to the mesh
            DuplexChannelFactory<ITransactionNotifierChannel> factory = 
                new DuplexChannelFactory<ITransactionNotifierChannel>(instanceContext, "ChatEndpoint");

            _channel = factory.CreateChannel();

            // Retrieve the PeerNode associated with the participant and register for online/offline events
            // PeerNode represents a node in the mesh. Mesh is the named collection of connected nodes.
            IOnlineStatus ostat = _channel.GetProperty<IOnlineStatus>();
            //ostat.Online += new EventHandler(OnOnline);
            //ostat.Offline += new EventHandler(OnOffline);

            _channel.Open();
        }

        public void ShutDown()
        {
            _channel.Close();
        }

        public void Notify(EntityChangeSet changeSet)
        {
            _channel.Notify(changeSet);
        }

        public event EventHandler<EntityChangeEventArgs> Notified
        {
            add { _notified += value; }
            remove { _notified -= value; }
        }

        private void OnNotify(EntityChangeSet changeSet)
        {
            EventsHelper.Fire(_notified, this, new EntityChangeEventArgs(changeSet));
        }

    }
}
