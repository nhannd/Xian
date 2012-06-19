using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using System;
using ClearCanvas.ImageViewer.Common.Auditing;

namespace ClearCanvas.ImageViewer.Common.StudyManagement
{
    internal class RemoteStudyRootQuery : IStudyRootQuery, IDisposable
    {
        private readonly IApplicationEntity _remoteServer;
        private DicomStudyRootQuery _real;

        public RemoteStudyRootQuery(IApplicationEntity remoteServer)
        {
            _remoteServer = remoteServer;
            _real = new DicomStudyRootQuery(DicomServer.DicomServer.AETitle, remoteServer);
        }

        /// <summary>
        /// Performs a STUDY level query.
        /// </summary>
        /// <exception cref="FaultException{TDetail}">Thrown when some part of the data in the request is poorly formatted.</exception>
        /// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
        public IList<StudyRootStudyIdentifier> StudyQuery(StudyRootStudyIdentifier queryCriteria)
        {
            var results = _real.StudyQuery(queryCriteria);

            AuditHelper.LogQueryIssued(_remoteServer.AETitle, _remoteServer.ScpParameters.HostName, EventSource.CurrentUser,
                           EventResult.Success, SopClass.StudyRootQueryRetrieveInformationModelFindUid,
                           queryCriteria.ToDicomAttributeCollection());

            return results;
        }

        /// <summary>
        /// Performs a SERIES level query.
        /// </summary>
        /// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
        /// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
        public IList<SeriesIdentifier> SeriesQuery(SeriesIdentifier queryCriteria)
        {
            var results = _real.SeriesQuery(queryCriteria);

            AuditHelper.LogQueryIssued(_remoteServer.AETitle, _remoteServer.ScpParameters.HostName, EventSource.CurrentUser,
                           EventResult.Success, SopClass.StudyRootQueryRetrieveInformationModelFindUid,
                           queryCriteria.ToDicomAttributeCollection());

            return results;
        }

        /// <summary>
        /// Performs an IMAGE level query.
        /// </summary>
        /// <exception cref="FaultException{DataValidationFault}">Thrown when some part of the data in the request is poorly formatted.</exception>
        /// <exception cref="FaultException{QueryFailedFault}">Thrown when the query fails.</exception>
        public IList<ImageIdentifier> ImageQuery(ImageIdentifier queryCriteria)
        {
            var results = _real.ImageQuery(queryCriteria);

            AuditHelper.LogQueryIssued(_remoteServer.AETitle, _remoteServer.ScpParameters.HostName, EventSource.CurrentUser,
                           EventResult.Success, SopClass.StudyRootQueryRetrieveInformationModelFindUid,
                           queryCriteria.ToDicomAttributeCollection());

            return results;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_real == null)
                return;

            _real.Dispose();
            _real = null;
        }

        #endregion
    }
}
