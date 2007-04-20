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
