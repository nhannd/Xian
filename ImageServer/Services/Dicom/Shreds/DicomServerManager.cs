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
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Dicom.Shreds
{
	/// <summary>
	/// This class manages the DICOM SCP Shred for the ImageServer.
	/// </summary>
	public class DicomServerManager : ThreadedService
	{
		#region Private Members
		private readonly List<DicomScp<DicomScpContext>> _listenerList = new List<DicomScp<DicomScpContext>>();
		private readonly object _syncLock = new object();
		private static DicomServerManager _instance;
		IList<ServerPartition> _partitions;
		private EventHandler<ServerPartitionChangedEventArgs> _changedEvent;
		#endregion

		#region Constructor
		public DicomServerManager(string name) : base(name)
		{}
		#endregion

		#region Properties
		/// <summary>
		/// Singleton instance of the class.
		/// </summary>
		public static DicomServerManager Instance
		{
			get
			{
				if (_instance == null)
					_instance = new DicomServerManager("DICOM Service Manager");

				return _instance;
			}
			set
			{
				_instance = value;
			}
		}
		#endregion

		#region Private Methods

		private void StartListeners(ServerPartition part)
		{
			DicomScpContext parms =
				new DicomScpContext(part, new FilesystemSelector(FilesystemMonitor.Instance));

			if (DicomSettings.Default.ListenIPV4)
			{
				DicomScp<DicomScpContext> ipV4Scp = new DicomScp<DicomScpContext>(parms, AssociationVerifier.Verify, AssociationAuditLogger.InstancesTransferredAuditLogger);

				ipV4Scp.ListenPort = part.Port;
				ipV4Scp.AeTitle = part.AeTitle;

				if (ipV4Scp.Start(IPAddress.Any))
				{
					_listenerList.Add(ipV4Scp);
					ApplicationActivityAuditHelper helper = new ApplicationActivityAuditHelper(
											ServerPlatform.AuditSource, 
											EventIdentificationTypeEventOutcomeIndicator.Success, 
											ApplicationActivityType.ApplicationStarted, 
											new AuditProcessActiveParticipant(ipV4Scp.AeTitle));
					ServerPlatform.LogAuditMessage(helper);
				}
				else
				{
					ApplicationActivityAuditHelper helper = new ApplicationActivityAuditHelper(
											ServerPlatform.AuditSource,
											EventIdentificationTypeEventOutcomeIndicator.MajorFailureActionMadeUnavailable,
											ApplicationActivityType.ApplicationStarted,
											new AuditProcessActiveParticipant(ipV4Scp.AeTitle));
					ServerPlatform.LogAuditMessage(helper);
					Platform.Log(LogLevel.Error, "Unable to add IPv4 SCP handler for server partition {0}",
								 part.Description);
					Platform.Log(LogLevel.Error,
								 "Partition {0} will not accept IPv4 incoming DICOM associations.",
								 part.Description);
					ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, "DICOM Listener",
                                         AlertTypeCodes.UnableToStart, null, TimeSpan.Zero, "Unable to start IPv4 DICOM listener on {0} : {1}",
					                     ipV4Scp.AeTitle, ipV4Scp.ListenPort);
				}
			}

			if (DicomSettings.Default.ListenIPV6)
			{
				DicomScp<DicomScpContext> ipV6Scp = new DicomScp<DicomScpContext>(parms, AssociationVerifier.Verify, AssociationAuditLogger.InstancesTransferredAuditLogger);

				ipV6Scp.ListenPort = part.Port;
				ipV6Scp.AeTitle = part.AeTitle;

				if (ipV6Scp.Start(IPAddress.IPv6Any))
				{
					_listenerList.Add(ipV6Scp);
					ApplicationActivityAuditHelper helper = new ApplicationActivityAuditHelper(
											ServerPlatform.AuditSource,
											EventIdentificationTypeEventOutcomeIndicator.Success,
											ApplicationActivityType.ApplicationStarted,
											new AuditProcessActiveParticipant(ipV6Scp.AeTitle));
					ServerPlatform.LogAuditMessage(helper);
				}
				else
				{
					ApplicationActivityAuditHelper helper = new ApplicationActivityAuditHelper(
						ServerPlatform.AuditSource,
						EventIdentificationTypeEventOutcomeIndicator.MajorFailureActionMadeUnavailable,
						ApplicationActivityType.ApplicationStarted,
						new AuditProcessActiveParticipant(ipV6Scp.AeTitle));
					ServerPlatform.LogAuditMessage(helper);

					Platform.Log(LogLevel.Error, "Unable to add IPv6 SCP handler for server partition {0}",
								 part.Description);
					Platform.Log(LogLevel.Error,
								 "Partition {0} will not accept IPv6 incoming DICOM associations.",
								 part.Description);
					ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, "DICOM Listener",
                                         AlertTypeCodes.UnableToStart, null, TimeSpan.Zero, "Unable to start IPv6 DICOM listener on {0} : {1}",
										 ipV6Scp.AeTitle, ipV6Scp.ListenPort);
				}
			}
		}

		private void CheckPartitions()
		{
    	
			lock (_syncLock)
			{
				_partitions = new List<ServerPartition>(ServerPartitionMonitor.Instance);
				IList<DicomScp<DicomScpContext>> scpsToDelete = new List<DicomScp<DicomScpContext>>();

				foreach (DicomScp<DicomScpContext> scp in _listenerList)
				{
					bool bFound = false;
					foreach (ServerPartition part in _partitions)
					{
						if (part.Port == scp.ListenPort && part.AeTitle.Equals(scp.AeTitle) && part.Enabled)
						{
							bFound = true;
							break;
						}
					}

					if (!bFound)
					{
						Platform.Log(LogLevel.Info, "Partition was deleted, shutting down listener {0}:{1}", scp.AeTitle, scp.ListenPort);
						scp.Stop();
						scpsToDelete.Add(scp);
						ApplicationActivityAuditHelper helper = new ApplicationActivityAuditHelper(
												ServerPlatform.AuditSource,
												EventIdentificationTypeEventOutcomeIndicator.Success,
												ApplicationActivityType.ApplicationStopped,
												new AuditProcessActiveParticipant(scp.AeTitle));
						ServerPlatform.LogAuditMessage(helper);
					}
				}

				foreach (DicomScp<DicomScpContext> scp in scpsToDelete)
					_listenerList.Remove(scp);

				foreach (ServerPartition part in _partitions)
				{
					if (!part.Enabled)
						continue;

					bool bFound = false;
					foreach (DicomScp<DicomScpContext> scp in _listenerList)
					{
						if (part.Port != scp.ListenPort || !part.AeTitle.Equals(scp.AeTitle))
							continue;

						// Reset the context partition, incase its changed.
						scp.Context.Partition = part;

						bFound = true;
						break;
					}

					if (!bFound)
					{
						Platform.Log(LogLevel.Info, "Detected partition was added, starting listener {0}:{1}", part.AeTitle, part.Port);
						StartListeners(part);
					}
				}
			}
		}
		#endregion

		#region Public Methods
		protected override void Initialize()
		{
			if (_partitions == null)
			{
				// Force a read context to be opened.  When developing the retry mechanism 
				// for startup when the DB was down, there were problems when the type
				// initializer for enumerated values were failing first.  For some reason,
				// when the database went back online, they would still give exceptions.
				// changed to force the processor to open a dummy DB connect and cause an 
				// exception here, instead of getting to the enumerated value initializer.
				using (IReadContext readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
				{
				}

				_changedEvent = delegate
				                	{
				                		CheckPartitions();
				                	};
				ServerPartitionMonitor.Instance.Changed += _changedEvent;

				_partitions = new List<ServerPartition>(ServerPartitionMonitor.Instance);
			}
		}

		/// <summary>
		/// Method called when starting the DICOM SCP.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The method starts a <see cref="DicomScp{DicomScpParameters}"/> instance for each server partition configured in
		/// the database.  It assumes that the combination of the configured AE Title and Port for the 
		/// partition is unique.  
		/// </para>
		/// </remarks>
		protected override void Run()
		{
			foreach (ServerPartition part in _partitions)
			{
				if (part.Enabled)
				{
					StartListeners(part);
				}
			}
		}

		/// <summary>
		/// Method called when stopping the DICOM SCP.
		/// </summary>
		protected override void Stop()
		{
			lock (_syncLock)
			{
				foreach (DicomScp<DicomScpContext> scp in _listenerList)
				{
					scp.Stop();
					ApplicationActivityAuditHelper helper = new ApplicationActivityAuditHelper(
								ServerPlatform.AuditSource,
								EventIdentificationTypeEventOutcomeIndicator.Success,
								ApplicationActivityType.ApplicationStopped,
								new AuditProcessActiveParticipant(scp.AeTitle));
					ServerPlatform.LogAuditMessage(helper);
	
				}
				ServerPartitionMonitor.Instance.Changed -= _changedEvent;
			}
		}
		#endregion
	}
}