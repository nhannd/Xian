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
        private IList<ServerPartition> _partitionList;
        private IList<Filesystem> _filesystemList;
        private IPersistentStore _store;
        public FileSystemMonitor(IPersistentStore persistentStore)
        {
            _store = persistentStore;
        }

        public void Load()
        {
            IReadContext dbRead = _store.OpenReadContext();

            IGetServerPartitions partitionsQuery = dbRead.GetBroker<IGetServerPartitions>();

            _partitionList = partitionsQuery.Execute();

            IGetFilesystems filesystemQuery = dbRead.GetBroker<IGetFilesystems>();

            _filesystemList = filesystemQuery.Execute();
        }

        public IList<ServerPartition> Partitions
        {
            get { return _partitionList; }
        }
        public IList<Filesystem> Filesystems
        {
            get { return _filesystemList; }
        }

    }
}
