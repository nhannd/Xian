#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Class for monitoring the status of filesystems.
    /// </summary>
    /// <remarks>
    /// The class creates a background thread that monitors the current usage of the filesystems.  
    /// The class will also update itself and retrieve updated filesystem information from
    /// the database periodically.
    /// </remarks>
    public class FilesystemMonitor : IDisposable
    {
        #region Private Members
        private readonly Dictionary<ServerEntityKey, ServerFilesystemInfo> _filesystemList = new Dictionary<ServerEntityKey,ServerFilesystemInfo>();
        private readonly IPersistentStore _store;
        private Thread _theThread = null;
        private bool _stop = false;
        private readonly object _lock = new object();
        #endregion

        #region Constructors
        public FilesystemMonitor()
        {
            _store = PersistentStoreRegistry.GetDefaultStore();
        }
        #endregion

        #region Public Methods

        public IEnumerable<ServerFilesystemInfo> GetFilesystems()
        {
            lock (_lock)
            {
                return _filesystemList.Values;
            }
        }
        public ServerFilesystemInfo GetFilesystemInfo(ServerEntityKey filesystemKey)
        {
            lock(_lock)
            {
                if (_filesystemList.ContainsKey(filesystemKey))
                {
                    return _filesystemList[filesystemKey];
                }                
            }
            return null;
        }
        public bool CheckFilesystemAboveLowWatermark(ServerEntityKey filesystemKey)
        {
            lock (_lock)
            {
                if (_filesystemList.ContainsKey(filesystemKey))
                {
                    return _filesystemList[filesystemKey].AboveLowWatermark;
                }
            }
            return false;
        }
        public bool CheckFilesystemAboveHighWatermark(ServerEntityKey filesystemKey)
        {
            lock (_lock)
            {
                if (_filesystemList.ContainsKey(filesystemKey))
                {
                    return _filesystemList[filesystemKey].AboveHighWatermark;
                }
            }
            return false;
        }
        public float CheckFilesystemBytesToRemove(ServerEntityKey filesystemKey)
        {
            lock (_lock)
            {
                if (_filesystemList.ContainsKey(filesystemKey))
                {
                    return _filesystemList[filesystemKey].BytesToRemove;
                }
            }
            return 0.0f;
        }

        public Decimal CheckFilesystemPercentFull(ServerEntityKey filesystemKey)
        {
            lock (_lock)
            {
                if (_filesystemList.ContainsKey(filesystemKey))
                {
                    return (Decimal)_filesystemList[filesystemKey].UsedSpacePercentage;
                }
            }
            return 0.00M;
        }

        public bool CheckFilesystemWriteable(ServerEntityKey filesystemKey)
        {
            lock (_lock)
            {
                if (_filesystemList.ContainsKey(filesystemKey))
                {
                    return _filesystemList[filesystemKey].Writeable;
                }
            }
            return false;
        }

        public bool CheckFilesystemReadable(ServerEntityKey filesystemKey)
        {
            lock (_lock)
            {
                if (_filesystemList.ContainsKey(filesystemKey))
                {
                    return _filesystemList[filesystemKey].Readable;
                }
            }
            return false;
        }

        public void Load()
        {
            lock (_lock)
            {
                using (IReadContext read = _store.OpenReadContext())
                {
                    IFilesystemEntityBroker filesystemSelect = read.GetBroker<IFilesystemEntityBroker>();
                    FilesystemSelectCriteria criteria = new FilesystemSelectCriteria();
                    IList<Filesystem> filesystemList = filesystemSelect.Find(criteria);

                    foreach (Filesystem filesystem in filesystemList)
                    {
                        ServerFilesystemInfo info = new ServerFilesystemInfo(filesystem);
                        _filesystemList.Add(filesystem.GetKey(), info);

                        info.LoadFreeSpace();
                    }
                }

                StartThread();
            }
        }
        public void ReLoad()
        {
            lock (_lock)
            {
                using (IReadContext read = _store.OpenReadContext())
                {
                    IFilesystemEntityBroker filesystemSelect = read.GetBroker<IFilesystemEntityBroker>();
                    FilesystemSelectCriteria criteria = new FilesystemSelectCriteria();
                    IList<Filesystem> filesystemList = filesystemSelect.Find(criteria);

                    foreach (Filesystem filesystem in filesystemList)
                    {
                        if (_filesystemList.ContainsKey(filesystem.GetKey()))
                            _filesystemList[filesystem.GetKey()].Filesystem = filesystem;
                        else
                        {
                            ServerFilesystemInfo info = new ServerFilesystemInfo(filesystem);
                            _filesystemList.Add(filesystem.GetKey(), info);

                            info.LoadFreeSpace();
                        }
                    }
                }
            }
        }

        public List<ServerFilesystemInfo> FindNextTierFilesystems(ServerFilesystemInfo filesystem)
        {
            FilesystemTierEnum currTier = filesystem.Filesystem.FilesystemTierEnum;
            FilesystemTierEnum newTier = currTier;

            List<FilesystemTierEnum> listTiers = FilesystemTierEnum.GetAll();
            FilesystemTierEnum[] tiers = new FilesystemTierEnum[listTiers.Count];
            listTiers.CopyTo(tiers);
            Array.Sort(tiers, delegate(FilesystemTierEnum tier1, FilesystemTierEnum tier2)
                           {
                               return tier1.Enum.CompareTo(tier2.Enum);
                           });

            foreach (FilesystemTierEnum tier in tiers)
            {
                if (tier.Enum > newTier.Enum)
                {
                    newTier = tier;
                    break;
                }
            }

            List<ServerFilesystemInfo> list = new List<ServerFilesystemInfo>();

            if (newTier != currTier)
            {
                foreach (ServerFilesystemInfo fs in GetFilesystems())
                {
                    if (fs.Filesystem.FilesystemTierEnum == newTier)
                    {
                        list.Add(fs);
                    }
                }
            }

            return list;
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
            DateTime nextDbCheck = nextFilesystemCheck.AddMinutes(2);

            while(_stop == false)
            {
                DateTime now = Platform.Time;

                if (now > nextDbCheck)
                {
                    nextDbCheck = now.AddMinutes(2);
                    ReLoad();
                }
                else if (now > nextFilesystemCheck)
                {
                    // Check very minute
                    nextFilesystemCheck = now.AddSeconds(30);

                    lock (_lock)
                    {
                        foreach (ServerFilesystemInfo info in _filesystemList.Values)
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
