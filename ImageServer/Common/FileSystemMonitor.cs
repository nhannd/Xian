using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;


namespace ClearCanvas.ImageServer.Common
{

    /// <summary>
    /// Class for monitoring the status of filesystems.
    /// </summary>
    public class FilesystemMonitor : IDisposable
    {
        #region Private Members
        private IList<ServerFilesystemInfo> _filesystemList = new List<ServerFilesystemInfo>();
        private IList<ServerPartition> _partitionList;
        private IPersistentStore _store;
        private Thread _theThread = null;
        private bool _stop = false;
        private object _lock = new object();
        #endregion

        #region Constructors
        public FilesystemMonitor()
        {
            _store = PersistentStoreRegistry.GetDefaultStore();
        }
        #endregion

        #region Public Properties
        public IList<ServerFilesystemInfo> Filesystems
        {
            get { return _filesystemList; }
        }
        public IList<ServerPartition> Partitions
        {
            get { return _partitionList; }
        }
        #endregion

        #region Public Methods

        public bool CheckFilesystemWriteable(ServerEntityKey filesystemKey)
        {
            lock (_lock)
            {
                foreach (ServerFilesystemInfo info in _filesystemList)
                {
                    if (info.Filesystem.GetKey().Equals(filesystemKey))
                    {
                        return info.Writeable;
                    }
                }
            }
            return false;
        }

        public bool CheckFilesystemReadable(ServerEntityKey filesystemKey)
        {
            lock (_lock)
            {
                foreach (ServerFilesystemInfo info in _filesystemList)
                {
                    if (info.Filesystem.GetKey().Equals(filesystemKey))
                    {
                        return info.Readable;
                    }
                }
            }
            return false;
        }
        public void Load()
        {
            
            IReadContext read = _store.OpenReadContext();

            IGetServerPartitions partitionsQuery = read.GetBroker<IGetServerPartitions>();

            _partitionList = partitionsQuery.Execute();

            IGetFilesystems filesystemQuery = read.GetBroker<IGetFilesystems>();

            IList<Filesystem> filesystemList = filesystemQuery.Execute();

            IGetFilesystemTiers filesystemTierQuery = read.GetBroker<IGetFilesystemTiers>();

            IList<FilesystemTier> tierList = filesystemTierQuery.Execute();

            read.Dispose();

            foreach (Filesystem filesystem in filesystemList)
            {
                foreach (FilesystemTier tier in tierList)
                {
                    if (tier.GetKey().Equals(filesystem.FilesystemTierKey))
                    {
                        ServerFilesystemInfo info = new ServerFilesystemInfo(filesystem, tier);
                        _filesystemList.Add(info);
                        break;
                    }
                }
            }

            StartThread();
        }
        #endregion

        #region Private Methods


        private void StartThread()
        {
            if (_theThread == null)
            {
                _theThread = new Thread(Run);
                _theThread.Name = "Filesystem Monitor";

                _theThread.Start();
            }
        }

        private void StopThread()
        {
            if (_theThread != null)
            {
                _stop = true;

                _theThread.Join();
                _theThread = null;
            }
        }

        private void Run()
        {
            DateTime nextFilesystemCheck = Platform.Time;

            while(_stop == false)
            {
                DateTime now = Platform.Time;

                if (now > nextFilesystemCheck)
                {
                    // Check very minute
                    nextFilesystemCheck = now.AddMinutes(2);

                    lock (_lock)
                    {
                        foreach (ServerFilesystemInfo info in _filesystemList)
                        {
                            info.LoadFreeSpace();
                        }
                    }
                }
                Thread.Sleep(5000);
            }            
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            StopThread();
        }

        #endregion
    }
}
