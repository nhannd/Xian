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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
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
		private List<FilesystemTierEnum> _tiers = new List<FilesystemTierEnum>();
		private readonly Dictionary<ServerEntityKey, ServerFilesystemInfo> _filesystemList = new Dictionary<ServerEntityKey, ServerFilesystemInfo>();
		// tier mapping table, sorted by tier order
		private Dictionary<FilesystemTierEnum, List<ServerFilesystemInfo>> _tierMap = new Dictionary<FilesystemTierEnum, List<ServerFilesystemInfo>>();
		private readonly IPersistentStore _store;
		private Timer _dbTimer;
		private Timer _fsTimer;
		private EventHandler<FilesystemChangedEventArgs> _changedListener;
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
		/// Return a collection of the currently configured filesystems.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ServerFilesystemInfo> GetFilesystems()
		{
			lock (_lock)
			{
				return _filesystemList.Values;
			}
		}

		/// <summary>
		/// Returns a collection of the currently configured filesystems associated with a tier.
		/// </summary>
		/// <param name="tier"></param>
		/// <returns></returns>
		public IEnumerable<ServerFilesystemInfo> GetFilesystems(FilesystemTierEnum tier)
		{
			lock (_lock)
			{
				return _tierMap[tier];
			}
		}

		/// <summary>
		/// Map of filesystems per tier.
		/// </summary>
		/// <returns></returns>
		public Dictionary<FilesystemTierEnum, List<ServerFilesystemInfo>> GetTierMap()
		{
			lock (_lock)
			{
				return _tierMap;
			}
		}

		/// <summary>
		/// Get Information regarding a specific filesystem.
		/// </summary>
		/// <param name="filesystemKey">The primary key of a filesystem to get info for.</param>
		/// <returns>A <see cref="ServerFilesystemInfo"/> structure for the filesystem, or null if the filesystem ahs not been found.</returns>
		public ServerFilesystemInfo GetFilesystemInfo(ServerEntityKey filesystemKey)
		{
			lock (_lock)
			{
				if (_filesystemList.ContainsKey(filesystemKey))
				{
					return _filesystemList[filesystemKey];
				}
			}
			return null;
		}

		/// <summary>
		/// Check if a filesystem is above the configured low watermark.
		/// </summary>
		/// <param name="filesystemKey"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Check if a filesystem is above the configured high watermark.
		/// </summary>
		/// <param name="filesystemKey"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Calculate the number of bytes to remove from a filesystem to get it to the low watermark.
		/// </summary>
		/// <param name="filesystemKey"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Get the Percent Full for a filesystem.
		/// </summary>
		/// <param name="filesystemKey"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Check if a filesystem is writeable.
		/// </summary>
		/// <param name="filesystemKey"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Check if a filesystem is readable.
		/// </summary>
		/// <param name="filesystemKey"></param>
		/// <returns></returns>
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

		public Dictionary<FilesystemTierEnum, List<ServerFilesystemInfo>> FindLowerTierFilesystems(ServerFilesystemInfo filesystem)
		{
			lock (_lock)
			{

				Dictionary<FilesystemTierEnum, List<ServerFilesystemInfo>> map =
					new Dictionary<FilesystemTierEnum, List<ServerFilesystemInfo>>();

				foreach (FilesystemTierEnum tier in _tiers)
				{
					if (tier.Enum > filesystem.Filesystem.FilesystemTierEnum.Enum)
						map.Add(tier, _tierMap[tier]);
				}

				return map;
			}
		}

		/// <summary>
		/// Gets the first filesystem in lower tier for storage purpose.
		/// </summary>
		/// <param name="filesystem">The current filesystem</param>
		/// <returns></returns>
		public ServerFilesystemInfo GetLowerTierFilesystemForStorage(ServerFilesystemInfo filesystem)
		{
			Dictionary<FilesystemTierEnum, List<ServerFilesystemInfo>> lowerTier = FindLowerTierFilesystems(filesystem);
			if (lowerTier == null || lowerTier.Count == 0)
				return null;

			foreach (List<ServerFilesystemInfo> listFS in lowerTier.Values)
			{
				// sort by free space size (descending)
				listFS.Sort(delegate(ServerFilesystemInfo fs1, ServerFilesystemInfo fs2)
							   {
								   if (fs1.Filesystem.FilesystemTierEnum.Enum.Equals(fs2.Filesystem.FilesystemTierEnum.Enum))
								   {
									   if (fs1.HighwaterMarkMargin < fs2.HighwaterMarkMargin)
										   return 1;
									   else if (fs1.HighwaterMarkMargin > fs2.HighwaterMarkMargin)
										   return -1;
									   else
										   return 0;

								   }
								   else
								   {
									   return fs1.Filesystem.FilesystemTierEnum.Enum.CompareTo(fs2.Filesystem.FilesystemTierEnum.Enum);
								   }
							   });

				// first one that's writable
				ServerFilesystemInfo newFilesystem = listFS.Find(delegate(ServerFilesystemInfo fs)
											  {
												  return fs.Writeable;
											  });

				if (newFilesystem != null)
				{
					return newFilesystem;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the first filesystem in lower tier for storage purpose.
		/// </summary>
		/// <param name="context">A database read context to use for search</param>
		/// <param name="location">The output storage location</param>
		/// <param name="partitionKey">The primark key of the ServerPartition table.</param>
		/// <param name="studyInstanceUid">The Study Instance UID of th estudy</param>
		/// <returns></returns>
		public bool GetStudyStorageLocation(IReadContext context, ServerEntityKey partitionKey, string studyInstanceUid, out StudyStorageLocation location)
		{
			IQueryStudyStorageLocation procedure = context.GetBroker<IQueryStudyStorageLocation>();
			StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();
			parms.ServerPartitionKey = partitionKey;
			parms.StudyInstanceUid = studyInstanceUid;
			IList<StudyStorageLocation> locationList = procedure.Find(parms);

			foreach (StudyStorageLocation studyLocation in locationList)
			{
				if (CheckFilesystemReadable(studyLocation.FilesystemKey))
				{
					location = studyLocation;
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
		/// <returns>true if a location was found, false otherwise.</returns>
		public bool GetStudyStorageLocation(ServerEntityKey partitionKey, string studyInstanceUid, out StudyStorageLocation location)
		{
			using (IReadContext read = _store.OpenReadContext())
			{
				return GetStudyStorageLocation(read, partitionKey, studyInstanceUid, out location);
			}
		}

		public bool GetStudyStorageLocation(IReadContext context, ServerEntityKey studyStorageKey, out StudyStorageLocation location)
		{
			IQueryStudyStorageLocation procedure = context.GetBroker<IQueryStudyStorageLocation>();
			StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();
			parms.StudyStorageKey = studyStorageKey;
			IList<StudyStorageLocation> locationList = procedure.Find(parms);

			foreach (StudyStorageLocation studyLocation in locationList)
			{
				if (CheckFilesystemReadable(studyLocation.FilesystemKey))
				{
					location = studyLocation;
					return true;
				}
			}

			//Platform.Log(LogLevel.Error, "Unable to find readable StudyStorageLocation for study.");
			location = null;
			return false;
		}

		/// <summary>
		/// Retrieves the storage location from the database for the specified study storage key.  Checks if the filesystem is online.
		/// </summary>
		/// <param name="studyStorageKey">The study storage key to get a location for.</param>
		/// <param name="location">The returned storage location.</param>
		/// <returns>true if a location was found, false otherwise.</returns>
		public bool GetStudyStorageLocation(ServerEntityKey studyStorageKey, out StudyStorageLocation location)
		{
			using (IReadContext read = _store.OpenReadContext())
			{
				return GetStudyStorageLocation(read, studyStorageKey, out location);
			}
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
			LoadFilesystems();
		}

		/// <summary>
		/// Load filesystem information from the database.
		/// </summary>
		private void LoadFilesystems()
		{
			bool changed = false;

			lock (_lock)
			{
				_tiers = FilesystemTierEnum.GetAll();

				// sorted by enum values
				_tiers.Sort(delegate(FilesystemTierEnum tier1, FilesystemTierEnum tier2)
						   {
							   return tier1.Enum.CompareTo(tier2.Enum);
						   });

				_tierMap = new Dictionary<FilesystemTierEnum, List<ServerFilesystemInfo>>();
				foreach (FilesystemTierEnum tier in _tiers)
				{
					_tierMap.Add(tier, new List<ServerFilesystemInfo>());
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
							_tierMap[filesystem.FilesystemTierEnum].Add(_filesystemList[filesystem.Key]);
						}
						else
						{
							ServerFilesystemInfo info = new ServerFilesystemInfo(filesystem);
							_filesystemList.Add(filesystem.Key, info);
							_tierMap[filesystem.FilesystemTierEnum].Add(info);
							info.LoadFreeSpace();
							changed = true;
						}
					}
				}

				
			}

			if (changed && _changedListener != null)
				EventsHelper.Fire(_changedListener, this, new FilesystemChangedEventArgs(this));
		}

		/// <summary>
		/// Timer callback for checking filesystem status.
		/// </summary>
		/// <param name="state"></param>
		private void CheckFilesystems(object state)
		{
			lock (_lock)
			{
				foreach (ServerFilesystemInfo info in _filesystemList.Values)
				{
					info.LoadFreeSpace();
				}
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
