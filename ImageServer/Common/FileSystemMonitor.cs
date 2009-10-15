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
using System.Collections.ObjectModel;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using Timer=System.Threading.Timer;

namespace ClearCanvas.ImageServer.Common
{
	/// <summary>
	/// Event args for partition monitor
	/// </summary>
	public class FilesystemChangedEventArgs : EventArgs
	{
		private readonly FilesystemMonitor _monitor;
		public FilesystemChangedEventArgs(FilesystemMonitor theMonitor)
		{
			_monitor = theMonitor;
		}

		public FilesystemMonitor Monitor
		{
			get { return _monitor; }
		}
	}

    /// <summary>
    /// Represents a lookup table mapping a <see cref="FilesystemTierEnum"/> to a list of <see cref="ServerFilesystemInfo"/> on that tier.
    /// </summary>
    class TierInfo : Dictionary<FilesystemTierEnum, List<ServerFilesystemInfo>>
    {
    }

    /// <summary>
    /// Represents a collection of <See cref="ServerFilesystemInfo"/>
    /// </summary>
    class FilesystemInfoCollection : Collection<ServerFilesystemInfo>
    {
        /// <summary>
        /// Create a new <see cref="FilesystemInfoCollection"/> with copies of the items in another list
        /// </summary>
        /// <param name="anotherList"></param>
        public FilesystemInfoCollection(IEnumerable<ServerFilesystemInfo> anotherList)
        {
            if (anotherList!=null)
            {
                foreach (ServerFilesystemInfo fs in anotherList)
                {
                    Add(new ServerFilesystemInfo(fs));
                }
            }
            
        }
    }

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
		#region Private Static Members
		private static FilesystemMonitor _singleton;
		private static readonly object _lock = new object();
		#endregion

		#region Private Members
		private readonly Dictionary<ServerEntityKey, ServerFilesystemInfo> _filesystemList = new Dictionary<ServerEntityKey, ServerFilesystemInfo>();
		private TierInfo _tierInfo = new TierInfo();
		private readonly IPersistentStore _store;
		private Timer _dbTimer;
		private Timer _fsTimer;
		private EventHandler<FilesystemChangedEventArgs> _changedListener;
		private StorageLocationCache _storageLocationCache = new StorageLocationCache();
		#endregion

		#region Private Constructors
		private FilesystemMonitor()
		{
			_store = PersistentStoreRegistry.GetDefaultStore();
		}
		#endregion

		#region Events
		/// <summary>
		/// Event handler for changes in filesystems.
		/// </summary>
		public event EventHandler<FilesystemChangedEventArgs> Changed
		{
			add { _changedListener += value; }
			remove { _changedListener -= value; }
		}
		#endregion

		#region Public Static Members
		/// <summary>
		/// Singleton FilesystemMonitor created the first time its referenced.
		/// </summary>
		public static FilesystemMonitor Instance
		{
			get
			{
				lock (_lock)
				{
					if (_singleton == null)
					{
						_singleton = new FilesystemMonitor();
						_singleton.Initialize();
					}
					return _singleton;
				}
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Return a snapshot the current filesystems.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ServerFilesystemInfo> GetFilesystems()
		{
			lock (_lock)
			{
                // Return a list with copies of the current <see cref="ServerFilesystemInfo"/>
			    return new FilesystemInfoCollection(_filesystemList.Values);
			}
		}

        /// <summary>
        /// Return a snapshot the current filesystems.
        /// </summary>
        /// <returns></returns>
        public IList<ServerFilesystemInfo> GetFilesystems(Predicate<ServerFilesystemInfo> filter)
        {
            lock (_lock)
            {
                FilesystemInfoCollection copy = new FilesystemInfoCollection(_filesystemList.Values);
                CollectionUtils.Remove(copy, filter);
                return copy;
            }
        }


		/// <summary>
		/// Get the snapshot of the specific filesystem.
		/// </summary>
		/// <param name="filesystemKey">The primary key of a filesystem to get info for.</param>
		/// <returns>A <see cref="ServerFilesystemInfo"/> structure for the filesystem, or null if the filesystem ahs not been found.</returns>
		public ServerFilesystemInfo GetFilesystemInfo(ServerEntityKey filesystemKey)
		{
			lock (_lock)
			{
				ServerFilesystemInfo info;
				if (!_filesystemList.TryGetValue(filesystemKey, out info))
				{
					LoadFilesystems();
					_filesystemList.TryGetValue(filesystemKey, out info);
				}
				
                // TODO: Should we throw an exception instead?
				if (info==null)
					return null;

				return new ServerFilesystemInfo(info); // return a copy 
			}
		}

		/// <summary>
		/// Calculate the number of bytes to remove from a filesystem to get it to the low watermark.
		/// </summary>
		/// <param name="filesystemKey"></param>
		/// <returns></returns>
		public float CheckFilesystemBytesToRemove(ServerEntityKey filesystemKey)
		{
			lock (_lock)
			{
				ServerFilesystemInfo info;
				if (!_filesystemList.TryGetValue(filesystemKey, out info))
				{
					LoadFilesystems();
					_filesystemList.TryGetValue(filesystemKey, out info);
				}
				if (info != null)
					return info.BytesToRemove;
			}
			return 0.0f;
		}

		/// <summary>
		/// Check if a filesystem is writeable.
		/// </summary>
		/// <param name="filesystemKey"></param>
		/// <returns></returns>
		public bool CheckFilesystemWriteable(ServerEntityKey filesystemKey)
		{
			lock (_lock)
			{
				ServerFilesystemInfo info;
				if (!_filesystemList.TryGetValue(filesystemKey, out info))
				{
					LoadFilesystems();
					_filesystemList.TryGetValue(filesystemKey, out info);
				}
				if (info != null)
					return info.Writeable;
			}
			return false;
		}

		/// <summary>
		/// Check if a filesystem is readable.
		/// </summary>
		/// <param name="filesystemKey"></param>
		/// <returns></returns>
		public bool CheckFilesystemReadable(ServerEntityKey filesystemKey)
		{
			lock (_lock)
			{
				ServerFilesystemInfo info;
				if (!_filesystemList.TryGetValue(filesystemKey, out info))
				{
					LoadFilesystems();
					_filesystemList.TryGetValue(filesystemKey, out info);
				}
				if (info != null)
					return info.Readable;
			}
			return false;
		}

		/// <summary>
		/// Check if a filesystem is online.
		/// </summary>
		/// <param name="filesystemKey">The filesystem primary Key</param>
		/// <returns></returns>
		public bool CheckFilesystemOnline(ServerEntityKey filesystemKey)
		{
			lock (_lock)
			{
				ServerFilesystemInfo info;
				if (!_filesystemList.TryGetValue(filesystemKey, out info))
				{
					LoadFilesystems();
					_filesystemList.TryGetValue(filesystemKey, out info);
				}
				if (info != null)
					return info.Online;
			}
			return false;
		}

		private List<FilesystemTierEnum> FindLowerTierFilesystems(ServerFilesystemInfo filesystem)
		{
		    List<FilesystemTierEnum> lowerTiers = new List<FilesystemTierEnum>();

			foreach (FilesystemTierEnum tier in _tierInfo.Keys)
			{
				if (tier.Enum > filesystem.Filesystem.FilesystemTierEnum.Enum)
                    lowerTiers.Add(tier);
			}

            return lowerTiers;
		}


		/// <summary>
		/// Gets the first filesystem in lower tier for storage purpose.
		/// </summary>
		/// <param name="filesystem">The current filesystem</param>
		/// <returns></returns>
		public ServerFilesystemInfo GetLowerTierFilesystemForStorage(ServerFilesystemInfo filesystem)
		{
            lock(_lock)
            {
                List<FilesystemTierEnum> lowerTiers = FindLowerTierFilesystems(filesystem);
                if (lowerTiers == null || lowerTiers.Count == 0)
                    return null;

                List<ServerFilesystemInfo> list = new List<ServerFilesystemInfo>();
                foreach (FilesystemTierEnum tier in lowerTiers)
                {
                    list.AddRange(_tierInfo[tier]);
                }
                CollectionUtils.Remove(list, delegate(ServerFilesystemInfo fs) { return !fs.Writeable; });
                list = CollectionUtils.Sort(list, FilesystemSorter.SortByFreeSpace);
                ServerFilesystemInfo lowtierFilesystem= CollectionUtils.FirstElement(list);
				if (lowtierFilesystem == null)
					return null;
                return new ServerFilesystemInfo(lowtierFilesystem);//return a copy
            }
			
		}

		/// <summary>
		/// Gets the first filesystem in lower tier for storage purpose.
		/// </summary>
		/// <param name="context">A database read context to use for search</param>
		/// <param name="location">The output storage location</param>
		/// <param name="partitionKey">The primark key of the ServerPartition table.</param>
		/// <param name="studyInstanceUid">The Study Instance UID of the study</param>
		/// <param name="cacheValue">Specify if the value will be cached for future retrieval.</param>
		/// <returns></returns>
		public bool GetOnlineStudyStorageLocation(IPersistenceContext context, ServerEntityKey partitionKey, string studyInstanceUid, bool cacheValue, out StudyStorageLocation location)
		{
			location = _storageLocationCache.GetCachedStudy(partitionKey, studyInstanceUid);
			if (location != null)
			{
				if (CheckFilesystemOnline(location.FilesystemKey))
					return true;

				location = null;
				return false;
			}

			IQueryStudyStorageLocation procedure = context.GetBroker<IQueryStudyStorageLocation>();
			StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();
			parms.ServerPartitionKey = partitionKey;
			parms.StudyInstanceUid = studyInstanceUid;
			IList<StudyStorageLocation> locationList = procedure.Find(parms);

			foreach (StudyStorageLocation studyLocation in locationList)
			{
				if (CheckFilesystemOnline(studyLocation.FilesystemKey))
				{
					location = studyLocation;
					if (cacheValue)
						_storageLocationCache.AddCachedStudy(location);
					return true;
				}
			}

			//Platform.Log(LogLevel.Error, "Unable to find readable StudyStorageLocation for study.");
			location = null;
			return false;
		}

		/// <summary>
		/// Retrieves the storage location from the database for the specified study.  Checks if the filesystem is online.
		/// </summary>
		/// <param name="studyInstanceUid">The Study to check for.</param>
		/// <param name="location">The returned storage location.</param>
		/// <param name="partitionKey">The key for the server partition.</param>
		/// <param name="cacheValue">Specify if the value will be cached for future retrieval.</param>
		/// <returns>true if a location was found, false otherwise.</returns>
		public bool GetOnlineStudyStorageLocation(ServerEntityKey partitionKey, string studyInstanceUid, bool cacheValue, out StudyStorageLocation location)
		{
			using (IReadContext read = _store.OpenReadContext())
			{
				return GetOnlineStudyStorageLocation(read, partitionKey, studyInstanceUid, cacheValue, out location);
			}
		}

		/// <summary>
		/// Retrieves the storage location from the database for the specified study storage key.  Checks if the filesystem is online.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="studyStorageKey"></param>
		/// <param name="location"></param>
		/// <returns></returns>
        public bool GetOnlineStudyStorageLocation(IPersistenceContext context, ServerEntityKey studyStorageKey, out StudyStorageLocation location)
		{
			IQueryStudyStorageLocation procedure = context.GetBroker<IQueryStudyStorageLocation>();
			StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();
			parms.StudyStorageKey = studyStorageKey;
			IList<StudyStorageLocation> locationList = procedure.Find(parms);

			foreach (StudyStorageLocation studyLocation in locationList)
			{
				if (CheckFilesystemOnline(studyLocation.FilesystemKey))
				{
					location = studyLocation;
					return true;
				}
			}

            // TODO: throw new FilesystemIsNotWritableException();
			location = null;
			return false;
		}

		/// <summary>
		/// Retrieves the storage location from the database for the specified study storage key.  Checks if the filesystem is online.
		/// </summary>
		/// <param name="studyStorageKey">The study storage key to get a location for.</param>
		/// <param name="location">The returned storage location.</param>
		/// <returns>true if a location was found, false otherwise.</returns>
		public bool GetOnlineStudyStorageLocation(ServerEntityKey studyStorageKey, out StudyStorageLocation location)
		{
			using (IReadContext read = _store.OpenReadContext())
			{
				return GetOnlineStudyStorageLocation(read, studyStorageKey, out location);
			}
		}

        /// <summary>
        /// Returns a value indicating whether the filesystem with the specified key
        /// is writable.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsWritable(ServerEntityKey key)
        {
            ServerFilesystemInfo fs = GetFilesystemInfo(key);
            return fs.Writeable;
        }
		#endregion

		#region Private Methods
		/// <summary>
		/// Method for intializing.
		/// </summary>
		private void Initialize()
		{
			lock (_lock)
			{
				LoadFilesystems();
                StringBuilder log = new StringBuilder();
			    log.AppendLine("Filesystem Status:");
                foreach(ServerFilesystemInfo fs in _filesystemList.Values)
                {
                    log.AppendLine(String.Format("\t{0} : {1}", fs.Filesystem.Description, fs.StatusString));
                }
                Platform.Log(LogLevel.Info, log.ToString());

				_fsTimer = new Timer(CheckFilesystems, this, TimeSpan.FromSeconds(Settings.Default.FilesystemCheckDelaySeconds), TimeSpan.FromSeconds(Settings.Default.FilesystemCheckDelaySeconds));

				_dbTimer = new Timer(ReLoadFilesystems, this, TimeSpan.FromSeconds(Settings.Default.DbChangeDelaySeconds), TimeSpan.FromSeconds(Settings.Default.DbChangeDelaySeconds));
			}
		}

		/// <summary>
		/// Timer delegate for reloading filesystem information from the database.
		/// </summary>
		/// <param name="state"></param>
		private void ReLoadFilesystems(object state)
		{
			try
			{
				LoadFilesystems();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected exception when monitoring filesystem.");
			}
		}

		/// <summary>
		/// Load filesystem information from the database.
		/// </summary>
		private void LoadFilesystems()
		{
			bool changed = false;

			lock (_lock)
			{
                try
                {
                    List<FilesystemTierEnum> tiers = FilesystemTierEnum.GetAll();

                    // sorted by enum values
                    tiers.Sort(delegate(FilesystemTierEnum tier1, FilesystemTierEnum tier2)
                               {
                                   return tier1.Enum.CompareTo(tier2.Enum);
                               });

                    _tierInfo = new TierInfo();

                    foreach (FilesystemTierEnum tier in tiers)
                    {
                        _tierInfo.Add(tier, new List<ServerFilesystemInfo>());
                    }

                    using (IReadContext read = _store.OpenReadContext())
                    {
                        IFilesystemEntityBroker filesystemSelect = read.GetBroker<IFilesystemEntityBroker>();
                        FilesystemSelectCriteria criteria = new FilesystemSelectCriteria();
                        IList<Filesystem> filesystemList = filesystemSelect.Find(criteria);

                        foreach (Filesystem filesystem in filesystemList)
                        {
                            if (_filesystemList.ContainsKey(filesystem.Key))
                            {
                                if ((filesystem.HighWatermark != _filesystemList[filesystem.Key].Filesystem.HighWatermark)
                                    || (filesystem.LowWatermark != _filesystemList[filesystem.Key].Filesystem.LowWatermark))
                                    Platform.Log(LogLevel.Info, "Watermarks have changed for filesystem {0}, Low: {1}, High: {2}",
                                                 filesystem.Description, filesystem.LowWatermark, filesystem.HighWatermark);
                                _filesystemList[filesystem.Key].Filesystem = filesystem;
                                _tierInfo[filesystem.FilesystemTierEnum].Add(_filesystemList[filesystem.Key]);
                            }
                            else
                            {
                                ServerFilesystemInfo info = new ServerFilesystemInfo(filesystem);
                                _filesystemList.Add(filesystem.Key, info);
                                _tierInfo[filesystem.FilesystemTierEnum].Add(info);
                                info.LoadFreeSpace();
                                changed = true;
                            }
                        }
                    }
                    if (changed && _changedListener != null)
                        EventsHelper.Fire(_changedListener, this, new FilesystemChangedEventArgs(this));
                }
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex,
                                 "Exception has occurred while updating the filesystem list from the datbase. Retry later");
                }
			}

			
		}

		/// <summary>
		/// Timer callback for checking filesystem status.
		/// </summary>
		/// <param name="state"></param>
		private void CheckFilesystems(object state)
		{
			// Load the filesystem objects into a dedicated list, in case the fileysstem list changes
			// while we're doing this.  
			IList<ServerFilesystemInfo> tempList;

			lock (_lock)
			{
				tempList = new List<ServerFilesystemInfo>(_filesystemList.Count);

				foreach (ServerFilesystemInfo info in _filesystemList.Values)
				{
					tempList.Add(info);
				}
			}

			foreach (ServerFilesystemInfo info in tempList)
			{
				info.LoadFreeSpace();
			}
		}

		#endregion

		#region IDisposable Implementation
		public void Dispose()
		{
			if (_fsTimer != null)
			{
				_fsTimer.Dispose();
				_fsTimer = null;
			}
			if (_dbTimer != null)
			{
				_dbTimer.Dispose();
				_dbTimer = null;
			}
		}
		#endregion
	}
}
