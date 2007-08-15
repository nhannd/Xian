using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Common
{
    public class FilesystemMonitor
    {
        private IList<ServerPartition> _partitionList;
        private IList<Filesystem> _filesystemList;
        private IPersistentStore _store;
        public FilesystemMonitor()
        {
            _store = PersistentStoreRegistry.GetDefaultStore();
        }

        public void Load()
        {
            IReadContext read = _store.OpenReadContext();

            IGetServerPartitions partitionsQuery = read.GetBroker<IGetServerPartitions>();

            _partitionList = partitionsQuery.Execute();

            IGetFilesystems filesystemQuery = read.GetBroker<IGetFilesystems>();

            _filesystemList = filesystemQuery.Execute();

            read.Dispose();
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
