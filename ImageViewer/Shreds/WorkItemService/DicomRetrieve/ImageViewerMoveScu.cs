#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.Auditing;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.DicomRetrieve
{
    internal class ImageViewerMoveScu : StudyRootMoveScu
    {
        #region Private Fields

        private readonly IPatientRootData _patientToRetrieve;
        private readonly IStudyIdentifier _studyToRetrieve;
        private readonly IEnumerable<string> _seriesInstanceUids;
        private string _errorDescriptionDetails;

        #endregion

        #region Public Properties

        public string ErrorDescriptionDetails { get { return _errorDescriptionDetails; } }
        
        #endregion

        #region Public Constructor

        public ImageViewerMoveScu(string localAETitle, IDicomServiceNode remoteAEInfo, IPatientRootData patient, IStudyIdentifier studiesToRetrieve)
            : base(localAETitle, remoteAEInfo.AETitle, remoteAEInfo.ScpParameters.HostName, remoteAEInfo.ScpParameters.Port, localAETitle)
        {
            Platform.CheckForEmptyString(localAETitle, "localAETitle");
            Platform.CheckForEmptyString(remoteAEInfo.AETitle, "AETitle");
            Platform.CheckForEmptyString(remoteAEInfo.ScpParameters.HostName, "HostName");
            Platform.CheckForNullReference(studiesToRetrieve, "studiesToRetrieve");

            _studyToRetrieve = studiesToRetrieve;
            _patientToRetrieve = patient;
            _errorDescriptionDetails = string.Empty;
        }

        public ImageViewerMoveScu(string localAETitle, IDicomServiceNode remoteAEInfo, IPatientRootData patient, IStudyIdentifier studyInformation, IEnumerable<string> seriesInstanceUids)
            : this(localAETitle, remoteAEInfo, patient, studyInformation)
        {
            _seriesInstanceUids = seriesInstanceUids;
        }

        #endregion

        #region Private Methods

        private void AuditRetrieveOperation(bool noExceptions)
        {
            var receivedInstances = new AuditedInstances();
            receivedInstances.AddInstance(_patientToRetrieve.PatientId, _patientToRetrieve.PatientsName, _studyToRetrieve.StudyInstanceUid);
            if (noExceptions)
                AuditHelper.LogReceivedInstances(RemoteAE, RemoteHost, receivedInstances, EventSource.CurrentProcess, EventResult.Success, EventReceiptAction.ActionUnknown);
            else
                AuditHelper.LogReceivedInstances(RemoteAE, RemoteHost, receivedInstances, EventSource.CurrentProcess, EventResult.MajorFailure, EventReceiptAction.ActionUnknown);
        }

        #endregion

        #region Public Methods

        public override void OnReceiveResponseMessage(DicomClient client, ClientAssociationParameters association, byte presentationID, ClearCanvas.Dicom.DicomMessage message)
        {
            base.OnReceiveResponseMessage(client, association, presentationID, message);

            if (message.Status.Status == DicomState.Warning)
            {
                _errorDescriptionDetails = String.Format("Remote server returned a warning status ({0}: {1}).",
                     RemoteAE, message.Status.Description);
            }
        }

        public void Retrieve()
        {
            AddStudyInstanceUid(_studyToRetrieve.StudyInstanceUid);

            if (_seriesInstanceUids != null)
            {
                foreach (string seriesInstanceUid in _seriesInstanceUids)
                    AddSeriesInstanceUid(seriesInstanceUid);
            }

            //do this rather than use BeginSend b/c it uses thread pool threads which can be exhausted.

            try
            {
                Move();

                Join(new TimeSpan(0, 0, 0, 0, 1000));

                if (Status == ScuOperationStatus.Canceled)
                {
                    _errorDescriptionDetails = string.Format("The Move operation was cancelled ({0}).", RemoteAE);
                }
                else if (Status == ScuOperationStatus.ConnectFailed)
                {
                    _errorDescriptionDetails = String.Format("Unable to connect to remote server ({0}: {1}).",
                        RemoteAE, FailureDescription ?? "no failure description provided");
                }
                else if (Status == ScuOperationStatus.AssociationRejected)
                {
                    _errorDescriptionDetails = String.Format("Association rejected ({0}: {1}).",
                        RemoteAE, FailureDescription ?? "no failure description provided");
                }
                else if (Status == ScuOperationStatus.Failed)
                {
                    _errorDescriptionDetails = String.Format("The Move operation failed ({0}: {1}).",
                        RemoteAE, FailureDescription ?? "no failure description provided");
                }
                else if (Status == ScuOperationStatus.TimeoutExpired)
                {
                    //ignore, because this is the scu, we don't want users to think an error has occurred
                    //in retrieving.
                }
                else if (Status == ScuOperationStatus.UnexpectedMessage)
                {
                    //ignore, because this is the scu, we don't want users to think an error has occurred
                    //in retrieving.
                }
                else if (Status == ScuOperationStatus.NetworkError)
                {
                    //ignore, because this is the scu, we don't want users to think an error has occurred
                    //in retrieving.
                }

                AuditRetrieveOperation(true);
            }
            catch (Exception e)
            {
                if (Status == ScuOperationStatus.ConnectFailed)
                {
                    _errorDescriptionDetails = String.Format("Unable to connect to remote server ({0}: {1}).",
                        RemoteAE, FailureDescription ?? "no failure description provided");
                }
                else
                {
                    _errorDescriptionDetails = String.Format("An unexpected error has occurred in the Move Scu: {0}:{1}:{2} -> {3}; {4}",
                                                  RemoteAE, RemoteHost, RemotePort, ClientAETitle, e.Message);
                }

                AuditRetrieveOperation(false);
            }
            finally
            {
                //OnRetrieveComplete(this);
            }
        }

        #endregion
    }

}
