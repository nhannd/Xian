using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Common
{
    public class FileSystemMonitor
    {
        private IList<ServerPartition> _list;
        private IPersistentStore _store;
        public FileSystemMonitor(IPersistentStore persistentStore)
        {
            _store = persistentStore;
        }

        public void Load()
        {
            IReadContext dbRead = _store.OpenReadContext();

            IGetServerPartitions partitionsQuery = dbRead.GetBroker<IGetServerPartitions>();

            _list = partitionsQuery.Execute();
        }
    }
}
