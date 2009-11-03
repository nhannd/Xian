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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    ///<summary>
    /// Plugin for handling DICOM Retrieve Requests implementing the <see cref="IDicomScp{TContext}"/> interface.
    ///</summary>
    [ExtensionOf(typeof(DicomScpExtensionPoint<DicomScpContext>))]
    public class MoveScpExtension : BaseScp, IDicomScp<DicomScpContext>
    {
        #region Private members
        private readonly List<SupportedSop> _list = new List<SupportedSop>();
        private ImageServerStorageScu _theScu;
        #endregion

        #region Contructors
        /// <summary>
        /// Public default constructor.  Implements the Find and Move services for 
        /// Patient Root and Study Root queries.
        /// </summary>
        public MoveScpExtension()
        {
            SupportedSop sop = new SupportedSop();
            sop.SopClass = SopClass.PatientRootQueryRetrieveInformationModelMove;
            sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
            _list.Add(sop);

            sop = new SupportedSop();
            sop.SopClass = SopClass.StudyRootQueryRetrieveInformationModelMove;
            sop.SyntaxList.Add(TransferSyntax.ExplicitVrLittleEndian);
            sop.SyntaxList.Add(TransferSyntax.ImplicitVrLittleEndian);
            _list.Add(sop);
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Create a list of SOP Instances to move based on a Patient level C-MOVE-RQ.
        /// </summary>
        /// <param name="read"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool GetSopListForPatient(IPersistenceContext read, DicomMessageBase msg)
        {

            string patientId = msg.DataSet[DicomTags.PatientId].GetString(0, "");

            IStudyEntityBroker select = read.GetBroker<IStudyEntityBroker>();

            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.PatientId.EqualTo(patientId);
			criteria.ServerPartitionKey.EqualTo(Partition.Key);

            IList<Study> studyList = select.Find(criteria);


            foreach (Study study in studyList)
            {
                StudyStorageLocation location;
                
                if (false == FilesystemMonitor.Instance.GetOnlineStudyStorageLocation(Partition.Key, study.StudyInstanceUid, true, out location))
                    return false;

                StudyXml theStream = LoadStudyXml(location);

                _theScu.LoadStudyFromStudyXml(location.GetStudyPath(), theStream);
            }

            return true;
        }

        /// <summary>
        /// Create a list of DICOM SOP Instances to move based on a Study level C-MOVE-RQ.
        /// </summary>
        /// <param name="msg"></param>
       /// <returns></returns>
        private bool GetSopListForStudy(DicomMessageBase msg)
        {
        	bool bOfflineFound = false;

            string[] studyList = (string[]) msg.DataSet[DicomTags.StudyInstanceUid].Values;

            // Now get the storage location
            foreach (string studyInstanceUid in studyList)
            {
                StudyStorageLocation location;

				if (false == FilesystemMonitor.Instance.GetOnlineStudyStorageLocation(Partition.Key, studyInstanceUid, true, out location))
				{
					StudyStorage studyStorage;
					if (!GetStudyStatus(studyInstanceUid, out studyStorage))
					{
						return false;
					}
					else if (studyStorage.StudyStatusEnum == StudyStatusEnum.Nearline)
					{
						if (null == ServerHelper.InsertRestoreRequest(studyStorage))
						{
							Platform.Log(LogLevel.Error, "Unable to insert RestoreQueue entry for study {0}", studyStorage.StudyInstanceUid);
							return false;
						}
						else
						{
							bOfflineFound = true;
							Platform.Log(LogLevel.Info, "Inserted Restore request for nearline study {0}", studyStorage.StudyInstanceUid);
							continue;
						}
					}
					else
						return false;
				}

				StudyXml theStream = LoadStudyXml(location);

                _theScu.LoadStudyFromStudyXml(location.GetStudyPath(), theStream);
            }

			if (bOfflineFound) return false;

            return true;
        }

        /// <summary>
        /// Create a list of DICOM SOP Instances to move based on a Series level C-MOVE-RQ
        /// </summary>
        /// <param name="persistenceContext"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool GetSopListForSeries(IPersistenceContext persistenceContext, DicomMessageBase msg)
        {

            string studyInstanceUid = msg.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
            string[] seriesList = (string[])msg.DataSet[DicomTags.SeriesInstanceUid].Values;

            // Now get the storage location
            StudyStorageLocation location;

			if (false == FilesystemMonitor.Instance.GetOnlineStudyStorageLocation(Partition.Key, studyInstanceUid, true, out location))
            {
				StudyStorage studyStorage;
				if (!GetStudyStatus(studyInstanceUid, out studyStorage))
				{
					return false;
				}
				else if (studyStorage.StudyStatusEnum == StudyStatusEnum.Nearline)
				{
                    if (null == ServerHelper.InsertRestoreRequest(studyStorage))
					{
						Platform.Log(LogLevel.Error, "Unable to insert RestoreQueue entry for study {0}", studyStorage.StudyInstanceUid);
						return false;
					}
					else
					{
						Platform.Log(LogLevel.Info, "Inserted Restore request for nearline study {0}", studyStorage.StudyInstanceUid);
						return false;
					}
				}
				else
					return false;
            }

			IStudyEntityBroker select = persistenceContext.GetBroker<IStudyEntityBroker>();

			StudySelectCriteria criteria = new StudySelectCriteria();
			criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
			criteria.ServerPartitionKey.EqualTo(Partition.Key);

			Study study = select.FindOne(criteria);
        	StudyXml studyStream = LoadStudyXml(location);

            foreach (string seriesInstanceUid in seriesList)
            {
				_theScu.LoadSeriesFromSeriesXml(studyStream, Path.Combine(location.GetStudyPath(), seriesInstanceUid), studyStream[seriesInstanceUid], study.PatientsName, study.PatientId);
            }

            return true;
        }

        /// <summary>
        /// Create a list of DICOM SOP Instances to move based on an Image level C-MOVE-RQ.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool GetSopListForSop(DicomMessageBase msg)
        {
            string studyInstanceUid = msg.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
            string seriesInstanceUid = msg.DataSet[DicomTags.SeriesInstanceUid].GetString(0, "");
            string[] sopInstanceUidArray = (string[])msg.DataSet[DicomTags.SopInstanceUid].Values;

            // Now get the storage location
            StudyStorageLocation location;

			if (false == FilesystemMonitor.Instance.GetOnlineStudyStorageLocation(Partition.Key, studyInstanceUid, true, out location))
			{
				StudyStorage studyStorage;
				if (!GetStudyStatus(studyInstanceUid, out studyStorage))
				{
					return false;
				}
				else if (studyStorage.StudyStatusEnum == StudyStatusEnum.Nearline)
				{
                    if (null == ServerHelper.InsertRestoreRequest(studyStorage))
					{
						Platform.Log(LogLevel.Error, "Unable to insert RestoreQueue entry for study {0}", studyStorage.StudyInstanceUid);
						return false;
					}
					else
					{
						Platform.Log(LogLevel.Info, "Inserted Restore request for nearline study {0}", studyStorage.StudyInstanceUid);
						return false;
					}
				}
				else
					return false;
			}

        	// There can be multiple SOP Instance UIDs in the move request
            foreach (string sopInstanceUid in sopInstanceUidArray)
            {
                string path = Path.Combine(location.GetStudyPath(), seriesInstanceUid);
                path = Path.Combine(path, sopInstanceUid + ".dcm");
                _theScu.AddStorageInstance(new StorageInstance(path));
            }

            return true;
        }

        /// <summary>
        /// Load <see cref="Device"/> information for a Move destination.
        /// </summary>
        /// <param name="read"></param>
        /// <param name="partition"></param>
        /// <param name="remoteAe"></param>
        /// <returns></returns>
        private static Device LoadRemoteHost(IPersistenceContext read, ServerPartition partition, string remoteAe)
        {
            IDeviceEntityBroker select = read.GetBroker<IDeviceEntityBroker>();

            // Setup the select parameters.
            DeviceSelectCriteria selectParms = new DeviceSelectCriteria();
            selectParms.AeTitle.EqualTo(remoteAe);
            selectParms.ServerPartitionKey.EqualTo(partition.GetKey());

            return select.FindOne(selectParms);
        }
        #endregion

        #region IDicomScp Members
        /// <summary>
        /// Main routine for processing C-MOVE-RQ messages.  Called by the <see cref="DicomScp{DicomScpParameters}"/> component.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="association"></param>
        /// <param name="presentationID"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
            bool finalResponseSent = false;

            try
            {
                // Check for a Cancel message, and cancel the SCU.
                if (message.CommandField == DicomCommandField.CCancelRequest)
                {
                    if (_theScu != null)
                    {
                        _theScu.Cancel();
                    }
                    return true;
                }

                // Get the level of the Move.
                String level = message.DataSet[DicomTags.QueryRetrieveLevel].GetString(0, "");

                // Trim the remote AE, see extra spaces at the end before which has caused problems
                string remoteAe = message.MoveDestination.Trim();

                // Open a DB Connection
                using (IReadContext read = _store.OpenReadContext())
                {
                    // Load remote device information fromt he database.
                    Device device = LoadRemoteHost(read, Partition, remoteAe);
                    if (device == null)
                    {
                        Platform.Log(LogLevel.Error,
                                     "Unknown move destination \"{0}\", failing C-MOVE-RQ from {1} to {2}",
                                     remoteAe, association.CallingAE, association.CalledAE);

                        server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
                                                 DicomStatuses.QueryRetrieveMoveDestinationUnknown);
                        finalResponseSent = true;
                        return true;
                    }

                    // If the remote node is a DHCP node, use its IP address from the connection information, else
                    // use what is configured.  Always use the configured port.
                    if (device.Dhcp)
                        device.IpAddress = association.RemoteEndPoint.Address.ToString();

                    // Now setup the StorageSCU component
                    _theScu = new ImageServerStorageScu(Partition, device,
                                             association.CallingAE, message.MessageId);


                    // Now create the list of SOPs to send
                    bool bOnline;

                    if (level.Equals("PATIENT"))
                    {
                        bOnline = GetSopListForPatient(read, message);
                    }
                    else if (level.Equals("STUDY"))
                    {
                        bOnline = GetSopListForStudy(message);
                    }
                    else if (level.Equals("SERIES"))
                    {
                        bOnline = GetSopListForSeries(read, message);
                    }
                    else if (level.Equals("IMAGE"))
                    {
                        bOnline = GetSopListForSop(message);
                    }
                    else
                    {
                        Platform.Log(LogLevel.Error, "Unexpected Study Root Move Query/Retrieve level: {0}", level);

                        server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
                                                 DicomStatuses.QueryRetrieveIdentifierDoesNotMatchSOPClass);
                        finalResponseSent = true;
                        return true;
                    }

                    // Could not find an online/readable location for the requested objects to move.
					// Note that if the C-MOVE-RQ included a list of study instance uids, and some 
					// were online and some offline, we don't fail now (ie, the check on the Count)
					if (!bOnline && _theScu.StorageInstanceList.Count == 0)
                    {
                        Platform.Log(LogLevel.Error, "Unable to find online storage location for C-MOVE-RQ");

                        server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
                                                 DicomStatuses.QueryRetrieveUnableToPerformSuboperations);
                        finalResponseSent = true;
                    	_theScu.Dispose();
                        _theScu = null;
                        return true;
                    }

                    // No files were eligible for transfer, just send success and return
                    if (_theScu.StorageInstanceList.Count == 0)
                    {
                        server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
                                                 DicomStatuses.Success,
                                                 0, 0, 0, 0);
                        finalResponseSent = true;
						_theScu.Dispose();
						_theScu = null;
                        return true;
                    }

                    // set the preferred syntax lists
                    _theScu.LoadPreferredSyntaxes(read);

                	_theScu.ImageStoreCompleted += delegate(Object sender, StorageInstance instance)
                	                               	{
                	                               		StorageScu scu = (StorageScu) sender;
                	                               		DicomMessage msg = new DicomMessage();
                	                               		DicomStatus status;

                	                               		if (scu.RemainingSubOperations == 0)
                	                               		{
                	                               			foreach (StorageInstance sop in _theScu.StorageInstanceList)
                	                               			{
                	                               				if ((sop.SendStatus.Status != DicomState.Success)
                	                               				    && (sop.SendStatus.Status != DicomState.Warning))
                	                               					msg.DataSet[DicomTags.FailedSopInstanceUidList].
                	                               						AppendString(
                	                               						sop.SopInstanceUid);
                	                               			}
                	                               			if (scu.Status == ScuOperationStatus.Canceled)
                	                               				status = DicomStatuses.Cancel;
                	                               			else if (scu.Status == ScuOperationStatus.ConnectFailed)
                	                               				status = DicomStatuses.QueryRetrieveMoveDestinationUnknown;
                	                               			else if (scu.FailureSubOperations > 0)
                	                               				status =
                	                               					DicomStatuses.
                	                               						QueryRetrieveSubOpsOneOrMoreFailures;
															else if (!bOnline)
																status = DicomStatuses.QueryRetrieveUnableToPerformSuboperations;
                	                               			else
                	                               				status = DicomStatuses.Success;
                	                               		}
                	                               		else
                	                               		{
                	                               			status = DicomStatuses.Pending;

                	                               			if ((scu.RemainingSubOperations%5) != 0)
                	                               				return;
                	                               			// Only send a RSP every 5 to reduce network load
                	                               		}
                	                               		server.SendCMoveResponse(presentationID, message.MessageId,
                	                               		                         msg, status,
                	                               		                         (ushort) scu.SuccessSubOperations,
                	                               		                         (ushort) scu.RemainingSubOperations,
                	                               		                         (ushort) scu.FailureSubOperations,
                	                               		                         (ushort) scu.WarningSubOperations);
                	                               		if (scu.RemainingSubOperations == 0)
                	                               			finalResponseSent = true;
                	                               	};

                	_theScu.AssociationAccepted += delegate(Object sender, AssociationParameters parms)
                	                               	{
                	                               		AssociationAuditLogger.BeginInstancesTransferAuditLogger(
                	                               			_theScu.StorageInstanceList,
                	                               			parms);
                	                               	};
					
                    _theScu.BeginSend(
                        delegate(IAsyncResult ar)
                        	{
								if (_theScu != null)
								{
									_theScu.EndSend(ar);
									_theScu.Dispose();
									_theScu = null;
								}
                        	},
                        _theScu);


                    return true;
                } // end using()
            } 
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error,e,"Unexpected exception when processing C-MOVE-RQ");
                if (finalResponseSent == false)
                {
                    try
                    {
                        server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
												 DicomStatuses.QueryRetrieveUnableToProcess);
                    }
                    catch (Exception x)
                    {
                        Platform.Log(LogLevel.Error, x,
                                     "Unable to send final C-MOVE-RSP message on association from {0} to {1}",
                                     association.CallingAE, association.CalledAE);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Return a list of SOP Classes and Transfer Syntaxes supported by this extension.
        /// </summary>
        /// <returns></returns>
        public override IList<SupportedSop> GetSupportedSopClasses()
        {
            return _list;
        }

        #endregion

        #region Overridden BaseSCP methods

        protected override DicomPresContextResult OnVerifyAssociation(AssociationParameters association, byte pcid)
        {
            
            if (!Device.AllowRetrieve)
            {
                return DicomPresContextResult.RejectUser; 
            }

            return DicomPresContextResult.Accept;
            
        }

        #endregion Overridden BaseSCP methods
    }
}