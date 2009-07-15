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

using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	/// <summary>
	/// HSM Based archive plugin.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="HsmArchive"/> class is a plugin to implement an 
	/// Hierarchical Storage Management archive.  Among the HSM style interfaces 
	/// to automated tape libraries are the Sun/StorageTek QFS/SAMFS and Legato DiskXtender.  
	/// </para>
	/// <para>
	/// The <see cref="HsmArchive"/> class takes as input an accessable directory where
	/// the filesystem for the HSM has been mounted.  When storing studies to the HSM
	/// filesystem, a hierarchical folder structure is created.  At the root, a folder
	/// typically based on Study Date is created.  Next, a folder named after Study Instance
	/// UID of the study being archived is created.  Finally, the ZIP file for the study
	/// is placed in this folder.  The zip file has a timestamp as the filename.
	/// </para>
	/// <para>
	/// The zip file created is not in a compressed format.  It assumes the images themselves
	/// are compressed, or the HSM filesystem / underlying tape drives are doing the compression.
	/// </para>
	/// <para>
	/// When a restore of a study occurs, the HsmArchive will do an initial read of the zip 
	/// file.  If the read fails, it will reschedule the read after a configurable time delay,
	/// allowing the HSM system to read the zip file off disk and restore it.
	/// </para>
	/// <para>
	/// The HsmArchive class is basically a shell for the archive.  A configurable number of threads
	/// are created to handle the actual archiving and restoring of data.
	/// </para>
	/// </remarks>
	[ExtensionOf(typeof(ImageServerArchiveExtensionPoint))]
	public class HsmArchive : ImageServerArchiveBase
	{
		private HsmArchiveService _archiveService;
		private HsmRestoreService _restoreService;
		
		private string _hsmPath;

		public string HsmPath
		{
			get { return _hsmPath; }
		}

		/// <summary>
		/// The <see cref="PartitionArchive"/> associated with the HsmArchive.
		/// </summary>
		public PartitionArchive PartitionArchive
		{
			get { return _partitionArchive; }
		}

		/// <summary>
		/// The Archive Type.
		/// </summary>
		public override ArchiveTypeEnum ArchiveType
		{
			get { return ArchiveTypeEnum.HsmArchive; }
		}

		public override RestoreQueue GetRestoreCandidate()
		{
			RestoreQueue queueItem;

			using (IUpdateContext updateContext = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				QueryRestoreQueueParameters parms = new QueryRestoreQueueParameters();

				parms.PartitionArchiveKey = _partitionArchive.GetKey();
				parms.ProcessorId = ServerPlatform.ProcessorId;
				parms.RestoreQueueStatusEnum = RestoreQueueStatusEnum.Restoring;
				IQueryRestoreQueue broker = updateContext.GetBroker<IQueryRestoreQueue>();

				// Stored procedure only returns 1 result.
				queueItem = broker.FindOne(parms);

	

				if (queueItem != null)
					updateContext.Commit();
			}
			if (queueItem == null)
			{
				using (IUpdateContext updateContext = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
					RestoreQueueSelectCriteria criteria = new RestoreQueueSelectCriteria();
					criteria.RestoreQueueStatusEnum.EqualTo(RestoreQueueStatusEnum.Restoring);
					IRestoreQueueEntityBroker restoreQueueBroker = updateContext.GetBroker<IRestoreQueueEntityBroker>();

					if (restoreQueueBroker.Count(criteria) > HsmSettings.Default.MaxSimultaneousRestores)
						return null;

					QueryRestoreQueueParameters parms = new QueryRestoreQueueParameters();

					parms.PartitionArchiveKey = _partitionArchive.GetKey();
					parms.ProcessorId = ServerPlatform.ProcessorId;
					parms.RestoreQueueStatusEnum = RestoreQueueStatusEnum.Pending;
					IQueryRestoreQueue broker = updateContext.GetBroker<IQueryRestoreQueue>();

					parms.RestoreQueueStatusEnum = RestoreQueueStatusEnum.Pending;
					queueItem = broker.FindOne(parms);

					if (queueItem != null)
						updateContext.Commit();
				}
			}
			return queueItem;
		}

		/// <summary>
		/// Start the archive.
		/// </summary>
		/// <param name="archive">The <see cref="PartitionArchive"/> to start.</param>
		public override void Start(PartitionArchive archive)
		{
			_partitionArchive = archive;

			LoadServerPartition();
		
			_hsmPath = string.Empty;

			//Hsm Archive specific Xml data.
			XmlElement element = archive.ConfigurationXml.DocumentElement;
			foreach (XmlElement node in element.ChildNodes)
				if (node.Name.Equals("RootDir"))
					_hsmPath = node.InnerText;
			
			// Start the restore service
			_restoreService = new HsmRestoreService("HSM Restore", this);
			_restoreService.StartService();

			// If not "readonly", start the archive service.
			if (!_partitionArchive.ReadOnly)
			{
				_archiveService = new HsmArchiveService("HSM Archive", this);	
				_archiveService.StartService();
			}			
		}

		/// <summary>
		/// Stop the archive.
		/// </summary>
		public override void Stop()
		{
			if (_restoreService != null)
			{
				_restoreService.StopService();
				_restoreService = null;
			}

			if (_archiveService != null)
			{
				_archiveService.StopService();
				_archiveService = null;
			}
		}
	}
}
