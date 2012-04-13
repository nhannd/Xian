﻿#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.DicomSend
{
    internal class ImageViewerStorageScu : StorageScu
    {
        public ImageViewerStorageScu(string localAETitle, DicomSendRequest request)
            : base(localAETitle, request.AeTitle, request.Host, request.Port)
        {
        }

        /// <summary>
        /// Load all of the instances in a given <see cref="SeriesXml"/> file into the component for sending.
        /// </summary>
        /// <param name="seriesXml"></param>
        /// <param name="location"></param>
        /// <param name="patientsName"></param>
        /// <param name="patientId"></param>
        /// <param name="studyXml"></param>
        private void LoadSeriesFromSeriesXml(StudyXml studyXml, StudyLocation location, SeriesXml seriesXml,
                                             string patientsName, string patientId)
        {
            foreach (InstanceXml instanceXml in seriesXml)
            {
                var instance =
                    new StorageInstance(location.GetSopInstancePath(seriesXml.SeriesInstanceUid,
                                                                    instanceXml.SopInstanceUid));

                AddStorageInstance(instance);
                instance.SopClass = instanceXml.SopClass;
                instance.TransferSyntax = instanceXml.TransferSyntax;
                instance.SopInstanceUid = instanceXml.SopInstanceUid;
                instance.PatientId = patientId;
                instance.PatientsName = patientsName;
                instance.StudyInstanceUid = studyXml.StudyInstanceUid;
            }
        }

        /// <summary>
        /// Load all of the instances in a given <see cref="StudyXml"/> file into the component for sending.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="studyXml">The <see cref="StudyXml"/> file to load from</param>
        public void LoadStudyFromStudyXml(StudyLocation location, StudyXml studyXml)
        {
            foreach (SeriesXml seriesXml in studyXml)
            {
                LoadSeriesFromSeriesXml(studyXml, location, seriesXml, studyXml.PatientsName, studyXml.PatientId);
            }
        }

        /// <summary>
        /// Load all of the instances in a given <see cref="StudyXml"/> file into the component for sending.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="seriesInstanceUid"></param>
        /// <param name="studyXml">The <see cref="StudyXml"/> file to load from</param>
        public void LoadSeriesFromStudyXml(StudyLocation location, StudyXml studyXml, string seriesInstanceUid)
        {
            foreach (SeriesXml seriesXml in studyXml)
            {
                if (seriesInstanceUid.Equals(seriesXml.SeriesInstanceUid))
                {
                    LoadSeriesFromSeriesXml(studyXml, location, seriesXml, studyXml.PatientsName, studyXml.PatientId);
                    break;
                }
            }
        }

        public void DoSend()
        {
            Failed = false;
            try
            {
                SendInternal();
                AuditSendOperation(true);
            }
            catch (Exception e)
            {
                // set the connection failure flag
                Failed = true;

                if (Status == ScuOperationStatus.ConnectFailed)
                {
                    OnSendError(String.Format("Unable to connect to remote server ({0}: {1}).",
                                              RemoteAE, FailureDescription ?? "no failure description provided"));
                }
                else if (StorageInstanceList.Count == 0)
                {
                    // if the storage instance count is zero, we know exactly why the storage operation failed
                    OnSendError(String.Format("Store operation failed (nothing to store)."));
                }
                else
                {
                    OnSendError(String.Format(
                        "An unexpected error occurred while processing the Store operation ({0}).", e.Message));
                }

                AuditSendOperation(false);
            }
        }

        private void AuditSendOperation(bool noExceptions)
        {
            if (noExceptions)
            {
                var sentInstances = new AuditedInstances();
                var failedInstances = new AuditedInstances();
                foreach (StorageInstance instance in StorageInstanceList)
                {
                    if (instance.SendStatus.Status == DicomState.Success)
                        sentInstances.AddInstance(instance.PatientId, instance.PatientsName, instance.StudyInstanceUid);
                    else
                        failedInstances.AddInstance(instance.PatientId, instance.PatientsName, instance.StudyInstanceUid);
                }
                AuditHelper.LogSentInstances(RemoteAE, RemoteHost, sentInstances, EventSource.CurrentProcess,
                                             EventResult.Success);
                AuditHelper.LogSentInstances(RemoteAE, RemoteHost, failedInstances, EventSource.CurrentProcess,
                                             EventResult.MinorFailure);
            }
            else
            {
                var sentInstances = new AuditedInstances();
                foreach (StorageInstance instance in StorageInstanceList)
                    sentInstances.AddInstance(instance.PatientId, instance.PatientsName, instance.StudyInstanceUid);
                AuditHelper.LogSentInstances(RemoteAE, RemoteHost, sentInstances, EventSource.CurrentProcess,
                                             EventResult.MajorFailure);
            }
        }

        private void SendInternal()
        {
     
            base.Send();

            Join(new TimeSpan(0, 0, 0, 0, 1000));

            if (Status == ScuOperationStatus.Canceled)
            {
                OnSendError(String.Format("The Store operation has been cancelled ({0}).", RemoteAE));
            }
            else if (Status == ScuOperationStatus.ConnectFailed)
            {
                OnSendError(String.Format("Unable to connect to remote server ({0}: {1}).",
                                          RemoteAE, FailureDescription ?? "no failure description provided"));
            }
            else if (Status == ScuOperationStatus.AssociationRejected)
            {
                OnSendError(String.Format("Association rejected ({0}: {1}).",
                                          RemoteAE, FailureDescription ?? "no failure description provided"));
            }
            else if (Status == ScuOperationStatus.Failed)
            {
                OnSendError(String.Format("The Store operation failed ({0}: {1}).",
                                          RemoteAE, FailureDescription ?? "no failure description provided"));
            }
            else if (Status == ScuOperationStatus.TimeoutExpired)
            {
                OnSendError(String.Format("The connection timeout has expired ({0}: {1}).",
                                          RemoteAE, FailureDescription ?? "no failure description provided"));
            }
            else if (Status == ScuOperationStatus.UnexpectedMessage)
            {
                OnSendError("Unexpected message received; aborted association.");
            }
            else if (Status == ScuOperationStatus.NetworkError)
            {
                OnSendError("An unexpected network error has occurred.");
            }
        }


        private void OnSendError(string message)
        {
            FailureDescription = message;
        }

        #region IStorageScu Members

        /// <summary>
        /// Gets a value indicating whether or not the operation as a whole (as opposed to an individual sub-operation) has failed.
        /// </summary>
        /// <remarks>
        /// Typically, this refers to exceptions being thrown on the connection socket.
        /// </remarks>
        public bool Failed { get; private set; }

        #endregion
    }
}


