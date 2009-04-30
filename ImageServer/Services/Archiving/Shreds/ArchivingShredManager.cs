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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.Archiving.Shreds
{
	/// <summary>
	/// Manager for handling the ImageServer archiving service.
	/// </summary>
	public class ArchivingShredManager: ThreadedService
	{
		#region PartitionArchiveService Class
		/// <summary>
		/// Class to represent archive service 
		/// </summary>
		protected class PartitionArchiveService
		{
			private readonly IImageServerArchivePlugin _archive;
			private PartitionArchive _partitionArchive;
			private ServerPartition _serverPartition;
		
			public PartitionArchiveService(IImageServerArchivePlugin archive, PartitionArchive partitionArchive, ServerPartition partition)
			{
				_archive = archive;
				_partitionArchive = partitionArchive;
				_serverPartition = partition;
			}

			public PartitionArchive PartitionArchive
			{
				get { return _partitionArchive; }
				set { _partitionArchive = value; }
			}

			public IImageServerArchivePlugin ArchivePlugin
			{
				get { return _archive; }
			}

			public ServerPartition ServerPartition
			{
				get { return _serverPartition; }
				set { _serverPartition = value; }
			}
		}
		#endregion

		#region Private Members
		private static ArchivingShredManager _instance;
		private readonly List<PartitionArchiveService> _archiveServiceList = new List<PartitionArchiveService>();
		private readonly object _syncLock = new object();
		#endregion

		#region Constructors
		/// <summary>
		/// **** For internal use only***
		/// </summary>
		private ArchivingShredManager(string name)
			: base(name)
		{ }
		#endregion

		#region Properties
		/// <summary>
		/// Singleton instance of the class.
		/// </summary>
		public static ArchivingShredManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new ArchivingShredManager("Archiving");

				return _instance;
			}
			set
			{
				_instance = value;
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Load the list of <see cref="PartitionArchive"/> entries that are enabled.
		/// </summary>
		/// <returns>The list of <see cref="PartitionArchive"/> instances from the persistant store</returns>
		private static IList<PartitionArchive> LoadEnabledPartitionArchives()
		{
			using (IReadContext readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
			{
				IPartitionArchiveEntityBroker broker = readContext.GetBroker<IPartitionArchiveEntityBroker>();

				PartitionArchiveSelectCriteria criteria = new PartitionArchiveSelectCriteria();

				criteria.Enabled.EqualTo(true);

				return broker.Find(criteria);
			}
		}

		/// <summary>
		/// Load the list of currently configured <see cref="ServerPartition"/> instances.
		/// </summary>
		/// <returns>The partition list.</returns>
		private static IList<ServerPartition> LoadPartitions()
		{
			//Get partitions
			IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();

			using (IReadContext read = store.OpenReadContext())
			{
				IServerPartitionEntityBroker broker = read.GetBroker<IServerPartitionEntityBroker>();
				ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();
				return broker.Find(criteria);
			}
		}

		/// <summary>
		/// Check the currently configured archives and plugins to see if any have been disabled.
		/// </summary>
		private void CheckConfiguredArchives()
		{
			IList<ServerPartition> partitionList = LoadPartitions();

			lock (_syncLock)
			{
				IList<PartitionArchiveService> partitionsToDelete = new List<PartitionArchiveService>();

				foreach (PartitionArchiveService archiveService in _archiveServiceList)
				{
					archiveService.PartitionArchive = PartitionArchive.Load(archiveService.PartitionArchive.GetKey());
					if (!archiveService.PartitionArchive.Enabled)
					{
						Platform.Log(LogLevel.Info, "PartitionArchive {0} has been disabled, stopping plugin.", archiveService.PartitionArchive.Description);
						archiveService.ArchivePlugin.Stop();
						partitionsToDelete.Add(archiveService);
					}
					else
					{
						bool bFound = false;
						foreach (ServerPartition serverPartition in partitionList)
						{
							if (serverPartition.GetKey().Equals(archiveService.ServerPartition.GetKey()) && serverPartition.Enabled)
							{
								bFound = true;
								break;
							}
						}

						if (!bFound)
						{
							Platform.Log(LogLevel.Info, "Partition was deleted or disabled, shutting down archive server {0}",
										 archiveService.ServerPartition.Description);
							archiveService.ArchivePlugin.Stop();
							partitionsToDelete.Add(archiveService);
						}
					}
				}

				// Remove the services from our internal list.
				foreach (PartitionArchiveService archivePlugin in partitionsToDelete)
					_archiveServiceList.Remove(archivePlugin);

				// Load the current extension list
				ImageServerArchiveExtensionPoint ep = new ImageServerArchiveExtensionPoint();
				ExtensionInfo[] extensionInfoList = ep.ListExtensions();


				// Scan the current list of enabled partition archives to see if any
				// new archives have been added
				foreach (PartitionArchive partitionArchive in LoadEnabledPartitionArchives())
				{
					ServerPartition newPartition = ServerPartition.Load(partitionArchive.ServerPartitionKey);

					if (!newPartition.Enabled)
						continue;

					bool bFound = false;
					foreach (PartitionArchiveService service in _archiveServiceList)
					{
						if (!partitionArchive.GetKey().Equals(service.PartitionArchive.GetKey()))
							continue;

						// Reset the context partition, incase its changed.
						service.PartitionArchive = partitionArchive;

						bFound = true;
						break;
					}

					if (!bFound)
					{
						// No match, scan the current extensions for a matching extension
						// to run the service
						foreach (ExtensionInfo extensionInfo in extensionInfoList)
						{
							IImageServerArchivePlugin archive =
								(IImageServerArchivePlugin)ep.CreateExtension(new ClassNameExtensionFilter(extensionInfo.FormalName));

							if (archive.ArchiveType.Equals(partitionArchive.ArchiveTypeEnum))
							{
								PartitionArchiveService service = new PartitionArchiveService(archive, partitionArchive, newPartition);
								Platform.Log(LogLevel.Info, "Detected PartitionArchive was added, starting archive {0}", partitionArchive.Description);
								service.ArchivePlugin.Start(partitionArchive);
								_archiveServiceList.Add(service);
								break;
							}
						}
					}
				}
			}
		}
		#endregion

		#region Protected Methods
		protected override void Initialize()
		{
			_archiveServiceList.Clear();


			// Force a read context to be opened.  When developing the retry mechanism 
			// for startup when the DB was down, there were problems when the type
			// initializer for enumerated values were failng first.  For some reason,
			// when the database went back online, they would still give exceptions.
			// changed to force the processor to open a dummy DB connect and cause an 
			// exception here, instead of getting to the enumerated value initializer.

			IList<PartitionArchive> partitionArchiveList = LoadEnabledPartitionArchives();


			ImageServerArchiveExtensionPoint ep = new ImageServerArchiveExtensionPoint();
			ExtensionInfo[] extensionInfoList = ep.ListExtensions();

			foreach (PartitionArchive partitionArchive in partitionArchiveList)
			{
				ServerPartition partition = ServerPartition.Load(partitionArchive.ServerPartitionKey);


				if (!partition.Enabled)
				{
					Platform.Log(LogLevel.Info, "Server Partition '{0}' is disabled, not starting PartitionArchive '{1}'", partition.Description,
					             partitionArchive.Description);
					continue;
				}

				if (!partitionArchive.Enabled)
				{
					Platform.Log(LogLevel.Info, "PartitionArchive '{0}' is disabled, not starting", partitionArchive.Description);
					continue;					
				}

				foreach (ExtensionInfo extensionInfo in extensionInfoList)
				{
					IImageServerArchivePlugin archive =
						(IImageServerArchivePlugin) ep.CreateExtension(new ClassNameExtensionFilter(extensionInfo.FormalName));

					if (archive.ArchiveType.Equals(partitionArchive.ArchiveTypeEnum))
					{
						
						_archiveServiceList.Add(new PartitionArchiveService(archive, partitionArchive, partition));
						break;
					}
				}
			}
		}

		protected override void Run()
		{
			foreach (PartitionArchiveService node in _archiveServiceList)
			{
				Platform.Log(LogLevel.Info, "Starting partition archive: {0}", node.PartitionArchive.Description);
				node.ArchivePlugin.Start(node.PartitionArchive);
			}

			while (!CheckStop(60000))
			{
				CheckConfiguredArchives();
			}
		}

		protected override void Stop()
		{
			lock (_syncLock)
			{
				foreach (PartitionArchiveService node in _archiveServiceList)
				{
					Platform.Log(LogLevel.Info, "Stopping partition archive: {0}", node.PartitionArchive.Description);
					node.ArchivePlugin.Stop();
				}
			}
		}
		#endregion
	}
}