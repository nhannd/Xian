using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using Timer=System.Threading.Timer;

namespace ClearCanvas.ImageServer.Common
{

    public class ServerPartitionChangedEventArgs:EventArgs
    {
    }

    public class ServerPartitionMonitor :  IEnumerable<ServerPartition>
    {
        private object _partitionsLock = new Object();
        private Dictionary<string, ServerPartition> _partitions = new Dictionary<string,ServerPartition>();
        private EventHandler<ServerPartitionChangedEventArgs> _changedListener;
        private Timer _timer;
        private static ServerPartitionMonitor _instance = new ServerPartitionMonitor();

        static public ServerPartitionMonitor Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// ***** internal use only ****
        /// </summary>
        private ServerPartitionMonitor()
        {
            LoadPartitions();

            _timer = new Timer(SynchDB, null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
        }

        
        public ServerPartition GetPartition(string serverAE)
        {
            if (String.IsNullOrEmpty(serverAE))
                return null;

            lock(_partitionsLock)
            {
                if (_partitions.ContainsKey(serverAE))
                    return _partitions[serverAE];
                else
                    return null;
            }
            
        }

        public event EventHandler<ServerPartitionChangedEventArgs>  Changed
        {
            add { _changedListener += value; }
            remove { _changedListener -= value; }
        }

        

        public void LoadPartitions()
        {
            bool changed = false;
            lock(_partitionsLock)
            {
                Dictionary<string, ServerPartition> templist = new Dictionary<string, ServerPartition>();
                IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
                using (IReadContext ctx = store.OpenReadContext())
                {
                    IServerPartitionEntityBroker broker = ctx.GetBroker<IServerPartitionEntityBroker>();
                    ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
                    IList<ServerPartition> list = broker.Find(criteria);
                    foreach(ServerPartition partition in list)
                    {
                        if (IsChanged(partition))
                        {
                            changed = true;
                        }

                        templist.Add(partition.AeTitle, partition);
                    }
                }

                _partitions = templist;
            }

            if (changed && _changedListener!=null)
            {
                EventsHelper.Fire(_changedListener, this, new ServerPartitionChangedEventArgs());
            }
        }

        private void SynchDB(object state)
        {
            LoadPartitions();
        }

        private bool IsChanged(ServerPartition p2)
        {
            if (_partitions.ContainsKey(p2.AeTitle))
            {
                ServerPartition p1 = _partitions[p2.AeTitle];
                if (p1.AcceptAnyDevice != p2.AcceptAnyDevice)
                {
                    return true;
                }
                if (p1.AutoInsertDevice != p2.AutoInsertDevice)
                {
                    return true;
                }

                if (p1.DefaultRemotePort != p2.DefaultRemotePort)
                {
                    return true;
                }

                if (p1.Description != p2.Description)
                {
                    return true;
                }

                //p1.DuplicateSopPolicyEnum != p2.DuplicateSopPolicyEnum ||
                if (p1.Enabled != p2.Enabled)
                {
                    return true;
                }

                if (p1.PartitionFolder != p2.PartitionFolder)
                {
                    return true;
                }

                if (p1.Port != p2.Port)
                {
                    return true;
                }

                else
                    return false;
            }
            else
            {
                return true;
            }
        }

        #region IEnumerable<ServerPartition> Members

        public IEnumerator<ServerPartition> GetEnumerator()
        {
            return _partitions.Values.GetEnumerator();
        }

        #endregion


        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _partitions.Values.GetEnumerator();
        }

        #endregion
    }
}
