#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.DicomServices;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Model.SelectBrokers;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.ImageServer.Services.Dicom;

namespace ClearCanvas.ImageServer.Services.Dicom
{
    ///<summary>
    /// Plugin for handling DICOM Retrieve Requests implementing the <see cref="IDicomScp"/> interface.
    ///</summary>
    [ExtensionOf(typeof(DicomScpExtensionPoint))]
    public class MoveScpExtension : BaseScp, IDicomScp
    {
        #region Private members
        private List<SupportedSop> _list = new List<SupportedSop>();
        private StorageScu _theScu;
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

        /// <summary>
        /// Load all of the images in a given series directory into a list that can be used by the <see cref="StorageScu"/> component.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="seriesStream"></param>
        /// <param name="seriesPath"></param>
        private static void LoadSeriesFromStream(List<StorageInstance> list, string seriesPath, SeriesXml seriesStream)
        {
            foreach (InstanceXml instanceStream in seriesStream)
            {
                string instancePath = Path.Combine(seriesPath, instanceStream.SopInstanceUid + ".dcm");
                StorageInstance instance = new StorageInstance(instancePath);
                list.Add(instance);

                instance.SopClass = instanceStream.SopClass;
                instance.TransferSyntax = instanceStream.TransferSyntax;
            }
        }

        /// <summary>
        /// Load all of the images in a given directory into a list that can be used by the <see cref="StorageScu"/> component.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="studyStream"></param>
        /// <param name="studyPath"></param>
        private static void LoadStudyFromStream(List<StorageInstance> list, string studyPath, StudyXml studyStream)
        {
            foreach (SeriesXml seriesStream in studyStream)
            {
                string seriesPath = Path.Combine(studyPath, seriesStream.SeriesInstanceUid);

                LoadSeriesFromStream(list, seriesPath, seriesStream);
            }
        }

        /// <summary>
        /// Create a list of SOP Instances to move based on a Patient level C-MOVE-RQ.
        /// </summary>
        /// <param name="read"></param>
        /// <param name="msg"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        private bool GetSopListForPatient(IReadContext read, DicomMessage msg, out List<StorageInstance> fileList)
        {
            List<StorageInstance> list = new List<StorageInstance>();
            fileList = list;

            string patientId = msg.DataSet[DicomTags.PatientId].GetString(0, "");

            ISelectStudy select = read.GetBroker<ISelectStudy>();

            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.PatientId.EqualTo(patientId);

            IList<Study> studyList = select.Find(criteria);


            foreach (Study study in studyList)
            {
                StudyStorageLocation location;
                
                if (false == GetStudyStorageLocation(study.StudyInstanceUid, out location))
                    return false;

                StudyXml theStream = LoadStudyStream(location);

                LoadStudyFromStream(list, location.GetStudyPath(), theStream);
            }

            fileList = list;
            return true;
        }

        /// <summary>
        /// Create a list of DICOM SOP Instances to move based on a Study level C-MOVE-RQ.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        private bool GetSopListForStudy(DicomMessage msg, out List<StorageInstance> fileList)
        {
            List<StorageInstance> list = new List<StorageInstance>();
            fileList = list;
            string[] studyList = (string[]) msg.DataSet[DicomTags.StudyInstanceUid].Values;

            // Now get the storage location
            foreach (string studyInstanceUid in studyList)
            {
                StudyStorageLocation location;

                if (false == GetStudyStorageLocation(studyInstanceUid, out location))
                    return false;

                StudyXml theStream = LoadStudyStream(location);

                LoadStudyFromStream(list, location.GetStudyPath(), theStream);
            }

            fileList = list;
            return true;
        }

        /// <summary>
        /// Create a list of DICOM SOP Instances to move based on a Series level C-MOVE-RQ
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        private bool GetSopListForSeries(DicomMessage msg, out List<StorageInstance> fileList)
        {
            List<StorageInstance> list = new List<StorageInstance>();
            fileList = list;

            string studyInstanceUid = msg.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
            string[] seriesList = (string[])msg.DataSet[DicomTags.SeriesInstanceUid].Values;

            // Now get the storage location
            StudyStorageLocation location;

            if (false == GetStudyStorageLocation(studyInstanceUid, out location))
                return false;

            StudyXml studyStream = LoadStudyStream(location);

            foreach (string seriesInstanceUid in seriesList)
            {
                LoadSeriesFromStream(list, Path.Combine(location.GetStudyPath(), seriesInstanceUid), studyStream[seriesInstanceUid]);
            }

            fileList = list;
            return true;
        }

        /// <summary>
        /// Create a list of DICOM SOP Instances to move based on an Image level C-MOVE-RQ.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        private bool GetSopListForSop(DicomMessage msg, out List<StorageInstance> fileList)
        {
            List<StorageInstance> list = new List<StorageInstance>();
            fileList = list;

            string studyInstanceUid = msg.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
            string seriesInstanceUid = msg.DataSet[DicomTags.SeriesInstanceUid].GetString(0, "");
            string[] sopInstanceUidArray = (string[])msg.DataSet[DicomTags.SopInstanceUid].Values;

            // Now get the storage location
            StudyStorageLocation location;

            if (false == GetStudyStorageLocation(studyInstanceUid, out location))
                return false;

            // There can be multiple SOP Instance UIDs in the move request
            foreach (string sopInstanceUid in sopInstanceUidArray)
            {
                string path = Path.Combine(location.GetStudyPath(), seriesInstanceUid);
                path = Path.Combine(path, sopInstanceUid + ".dcm");
                list.Add(new StorageInstance(path));
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
        private static Device LoadRemoteHost(IReadContext read, ServerPartition partition, string remoteAe)
        {
            IQueryDevice select = read.GetBroker<IQueryDevice>();

            // Setup the select parameters.
            DeviceQueryParameters selectParms = new DeviceQueryParameters();
            selectParms.AeTitle = remoteAe;
            selectParms.ServerPartitionKey = partition.GetKey();

            IList<Device> list = select.Execute(selectParms);

            if (list.Count == 0)
                return null;

            return list[0];
        }

        /// <summary>
        /// Load a list of preferred SOP Classes and Transfer Syntaxes for a Device.
        /// </summary>
        /// <param name="read"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        private static IList<SupportedSop> LoadPreferredSyntaxes(IReadContext read, Device device)
        {
            IQueryDevicePreferredTransferSyntax select = read.GetBroker<IQueryDevicePreferredTransferSyntax>();

            // Setup the select parameters.
            DevicePreferredTransferSyntaxQueryParameters selectParms = new DevicePreferredTransferSyntaxQueryParameters();
            selectParms.DeviceKey = device.GetKey();

            IList<DevicePreferredTransferSyntax> list = select.Execute(selectParms);
            
            // Translate the list returned into the database into a list that is supported by the Storage SCU Component
            List<SupportedSop> sopList = new List<SupportedSop>();
            foreach (DevicePreferredTransferSyntax preferred in list)
            {
                SupportedSop sop = new SupportedSop();
                sop.SopClass = SopClass.GetSopClass(preferred.GetServerSopClass().SopClassUid);
                sop.AddSyntax(TransferSyntax.GetTransferSyntax(preferred.GetServerTransferSyntax().Uid));

                sopList.Add(sop);
            }

            return sopList;
        }

        #region IDicomScp Members

        /// <summary>
        /// Main routine for processing C-MOVE-RQ messages.  Called by the <see cref="DicomScp"/> component.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="association"></param>
        /// <param name="presentationID"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool OnReceiveRequest(DicomServer server, ServerAssociationParameters association, byte presentationID, DicomMessage message)
        {
            IReadContext read = null;
            bool finalResponseSent = false;

            try
            {
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
                string localAe = Partition.AeTitle;

                // Open a DB Connection
                read = _store.OpenReadContext();

                // Load remote device information fromt he database.
                Device device = LoadRemoteHost(read, Partition, remoteAe);
                if (device == null)
                {
                    Platform.Log(LogLevel.Error, "Unknown move destination \"{0}\", failing C-MOVE-RQ from {1} to {2}",
                                 remoteAe, association.CallingAE, association.CalledAE);

                    server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
                                             DicomStatuses.QueryRetrieveMoveDestinationUnknown);
                    finalResponseSent = true;
                    read.Dispose();
                    read = null;
                    return true;
                }

                // If the remote node is a DHCP node, use its IP address from the connection information, else
                // use what is configured.  Always use the configured port.
                string remoteIp = device.IpAddress;
                if (device.Dhcp)
                    remoteIp = association.RemoteEndPoint.Address.ToString();

                // Get the list of preferred SOP Classes & Transfer syntaxes from the database.
                IList<SupportedSop> preferredSyntaxList = LoadPreferredSyntaxes(read, device);

                // Now create the list of SOPs to send
                List<StorageInstance> fileList;
                bool bOnline;

                if (level.Equals("PATIENT"))
                {
                    bOnline = GetSopListForPatient(read, message, out fileList);
                }
                else if (level.Equals("STUDY"))
                {
                    bOnline = GetSopListForStudy(message, out fileList);
                }
                else if (level.Equals("SERIES"))
                {
                    bOnline = GetSopListForSeries(message, out fileList);
                }
                else if (level.Equals("IMAGE"))
                {
                    bOnline = GetSopListForSop(message, out fileList);
                }
                else
                {
                    Platform.Log(LogLevel.Error, "Unexpected Study Root Move Query/Retrieve level: {0}", level);

                    server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
                                             DicomStatuses.QueryRetrieveIdentifierDoesNotMatchSOPClass);
                    finalResponseSent = true;
                    read.Dispose();
                    return true;
                }

                // Could not find an online/readable location for the requested objects to move.
                if (!bOnline)
                {
                    Platform.Log(LogLevel.Error, "Unable to find online storage location for C-MOVE-RQ");

                    server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
                                             DicomStatuses.QueryRetrieveUnableToProcess);
                    finalResponseSent = true;
                    read.Dispose();
                    return true;
                }

                // No files were eligible for transfer, just send success and return
                if (fileList.Count == 0)
                {
                    server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
                                             DicomStatuses.Success,
                                             0, 0, 0, 0);
                    finalResponseSent = true;
                    _theScu = null;
                    read.Dispose();
                    return true;
                }

                // Now setup the StorageSCU component, and do the actual transfer.
                _theScu = new StorageScu(localAe, remoteAe, remoteIp, device.Port,
                                         association.CallingAE, message.MessageId);

                // set the preferred syntax lists
                _theScu.SetPreferredSyntaxList(preferredSyntaxList);

                // set the list of files to transfer
                _theScu.AddStorageInstanceList(fileList);
                
                _theScu.ImageStoreCompleted += delegate(Object sender, StorageInstance instance)
                                                   {
                                                       StorageScu scu = (StorageScu)sender;
                                                       DicomMessage msg = new DicomMessage();
                                                       DicomStatus status;

                                                       if (scu.RemainingSubOperations == 0)
                                                       {
                                                           foreach (StorageInstance sop in fileList)
                                                           {
                                                               if ((sop.SendStatus.Status != DicomState.Success)
                                                                   && (sop.SendStatus.Status != DicomState.Warning))
                                                                   msg.DataSet[DicomTags.FailedSopInstanceUidList].AppendString(
                                                                       sop.SopInstanceUid);
                                                           }
                                                           if (scu.Status == ScuOperationStatus.Canceled)
                                                               status = DicomStatuses.Cancel;
                                                           else if (scu.FailureSubOperations > 0)
                                                               status = DicomStatuses.QueryRetrieveSubOpsOneOrMoreFailures;
                                                           else
                                                               status = DicomStatuses.Success;

                                                           _theScu = null;
                                                       }
                                                       else
                                                       {
                                                           status = DicomStatuses.Pending;

                                                           if ((scu.RemainingSubOperations%5) != 0)
                                                               return; // Only send a RSP every 5 to reduce network load
                                                       }
                                                       server.SendCMoveResponse(presentationID, message.MessageId, msg, status,
                                                                                (ushort) scu.SuccessSubOperations,
                                                                                (ushort) scu.RemainingSubOperations,
                                                                                (ushort) scu.FailureSubOperations,
                                                                                (ushort) scu.WarningSubOperations);
                                                       if (scu.RemainingSubOperations == 0)
                                                           finalResponseSent = true;
                                                   };

                _theScu.BeginSend(
                    delegate(IAsyncResult result)
                        {
                            //NOOP
                        },
                    _theScu);


                read.Dispose();
                read = null;
                return true;
            } 
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error,e,"Unexpected exception when processing C-MOVE-RQ");
                if (finalResponseSent == false)
                {
                    try
                    {
                        server.SendCMoveResponse(presentationID, message.MessageId, new DicomMessage(),
                                                 DicomStatuses.ProcessingFailure);
                    }
                    catch (Exception x)
                    {
                        Platform.Log(LogLevel.Error, x,
                                     "Unable to send final C-MOVE-RSP message on association from {0} to {1}",
                                     association.CallingAE, association.CalledAE);
                    }
                }
            }
            finally
            {
                if (read != null)
                    read.Dispose();
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
    }
}