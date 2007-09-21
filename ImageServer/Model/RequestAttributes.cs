using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Model
{
    public class RequestAttributes : ServerEntity
    {
        #region Constructors
        public RequestAttributes()
            : base("RequestAttributes")
        {
        }
        #endregion

        #region Private Members
        private ServerEntityKey _seriesKey;
        private string _requestedProcedureId;
        private string _scheduledProcedureStepId;
        #endregion

        #region Public Properties
        public ServerEntityKey SeriesKey
        {
            get { return _seriesKey; }
            set { _seriesKey = value; }
        }

        [DicomField(DicomTags.RequestedProcedureId, DefaultValue = DicomFieldDefault.Null)]
        public string RequestedProcedureId
        {
            get { return _requestedProcedureId; }
            set { _requestedProcedureId = value; }
        }

        [DicomField(DicomTags.ScheduledProcedureStepId, DefaultValue = DicomFieldDefault.Null)]
        public string ScheduledProcedureStepId
        {
            get { return _scheduledProcedureStepId; }
            set { _scheduledProcedureStepId = value; }
        }
        #endregion

    }
}
