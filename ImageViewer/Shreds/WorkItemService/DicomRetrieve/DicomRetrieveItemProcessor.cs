#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.DicomRetrieve
{
    /// <summary>
    ///  Class for procesing DICOM Retrieves.
    /// </summary>
    internal class DicomRetrieveItemProcessor : BaseItemProcessor<DicomRetrieveRequest, DicomRetrieveProgress>
    {
        #region Private Members

        private ImageViewerMoveScu _scu;
        private bool _cancelDueToDiskSpace = false;
        #endregion

        #region Public Properties

        public DicomRetrieveStudyRequest RetrieveStudy
        {
            get { return Request as DicomRetrieveStudyRequest; }
        }

        public DicomRetrieveSeriesRequest RetrieveSeries
        {
            get { return Request as DicomRetrieveSeriesRequest; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Override of Cancel() routine.
        /// </summary>
        /// <remarks>
        /// The Cancel must be overriden to call the SCU's Cancel routine.
        /// </remarks>
        public override void Cancel()
        {
            base.Cancel();
            _scu.Cancel();
        }

        /// <summary>
        /// Override of Stop() routine.
        /// </summary>
        /// <remarks>
        /// The Stop must be override to call the SCU's Cancel routine.
        /// </remarks>
        public override void Stop()
        {
            _scu.Cancel();
            base.Stop();
        }

        public override void Process()
        {
            EnsureMinLocalStorageSpace(0);

            DicomServerConfiguration configuration = GetServerConfiguration();
            var remoteAE = ServerDirectory.GetRemoteServerByName(Request.Source);
            if (remoteAE == null)
            {
                Proxy.Fail(string.Format("Unknown destination: {0}", Request.Source), WorkItemFailureType.Fatal);
                return;
            }

            if (RetrieveStudy != null)
                _scu = new ImageViewerMoveScu(configuration.AETitle, remoteAE, RetrieveStudy.Patient, RetrieveStudy.Study);
            else if (RetrieveSeries != null)
                _scu = new ImageViewerMoveScu(configuration.AETitle, remoteAE, RetrieveSeries.Patient, RetrieveSeries.Study, RetrieveSeries.SeriesInstanceUids);
            else
            {
                Proxy.Fail("Invalid request type.", WorkItemFailureType.Fatal);
                return;
            }
            
            Progress.ImagesToRetrieve = _scu.TotalSubOperations;
            Progress.FailureSubOperations = 0;
            Progress.WarningSubOperations = 0;
            Progress.SuccessSubOperations = 0;
            Progress.IsCancelable = false;
            Proxy.UpdateProgress();

            _scu.ImageMoveCompleted += OnMoveImage;
            
            _scu.Retrieve();

            Progress.ImagesToRetrieve = _scu.TotalSubOperations;
            Progress.SuccessSubOperations = _scu.SuccessSubOperations;
            Progress.FailureSubOperations = _scu.FailureSubOperations;
            Progress.WarningSubOperations = _scu.WarningSubOperations;
            Progress.StatusDetails = !string.IsNullOrEmpty(_scu.ErrorDescriptionDetails) ? _scu.ErrorDescriptionDetails : _scu.FailureDescription;            

            if (_scu.Canceled)
            {
                if (_cancelDueToDiskSpace)
                {
                    Progress.IsCancelable = true;
                    throw new NotEnoughStorageException(); // item will be failed
                }
                else if (StopPending)
                {
                    Progress.IsCancelable = true;
                    Proxy.Postpone();
                }
                else
                {
                    Proxy.Cancel();
                }
            }
            else if (_scu.FailureSubOperations > 0)
            {
                Progress.IsCancelable = true;
                Proxy.Fail(!string.IsNullOrEmpty(_scu.ErrorDescriptionDetails) ? _scu.ErrorDescriptionDetails : _scu.FailureDescription, WorkItemFailureType.NonFatal);
            }
            else
            {
                Proxy.Complete();
            }
        }

        #endregion

        #region Private Methods

        private void OnMoveImage(object sender, EventArgs storageInstance)
        {
            Progress.ImagesToRetrieve = _scu.TotalSubOperations;
            Progress.SuccessSubOperations = _scu.SuccessSubOperations;
            Progress.FailureSubOperations = _scu.FailureSubOperations;
            Progress.WarningSubOperations = _scu.WarningSubOperations;
            Progress.StatusDetails = !string.IsNullOrEmpty(_scu.ErrorDescriptionDetails) ? _scu.ErrorDescriptionDetails : _scu.FailureDescription;
            Proxy.UpdateProgress();

            if (!base.LocalStorageHasMinSpace(0))
            {
                _cancelDueToDiskSpace = true;
                _scu.Cancel();
            }
        }

        #endregion
    }
}
