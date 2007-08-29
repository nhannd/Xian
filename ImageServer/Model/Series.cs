using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Model
{
    public class Series: ServerEntity
    {
        #region Constructors
        public Series()
            : base("Series")
        {
        }
        #endregion

        #region Private Members
        private ServerEntityKey _serverPartitionKey;
        private ServerEntityKey _studyKey;
        private string _seriesInstanceUid;
        private string _modality;
        private string _seriesNumber;
        private string _seriesDescription;
        private int _numberOfSeriesRelatedInstances;
        private string _performedProcedureStepStartDate;
        private string _performedProcedureStepStartTime;
        private short _statusEnum;
        #endregion

        #region Public Properties
        private ServerEntityKey ServerPartitionKey
        {
            get { return _serverPartitionKey; }
            set { _serverPartitionKey = value; }
        }
        private ServerEntityKey StudyKey
        {
            get { return _studyKey; }
            set { _studyKey = value; }
        }

        [DicomField(DicomTags.SeriesInstanceUID, DefaultValue = DicomFieldDefault.Null)]
        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }

        [DicomField(DicomTags.Modality, DefaultValue = DicomFieldDefault.Null)]
        public string Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }

        [DicomField(DicomTags.SeriesNumber, DefaultValue = DicomFieldDefault.Null)]
        public string SeriesNumber
        {
            get { return _seriesNumber; }
            set { _seriesNumber = value; }
        }

        [DicomField(DicomTags.SeriesDescription, DefaultValue = DicomFieldDefault.Null)]
        public string SeriesDescription
        {
            get { return _seriesDescription; }
            set { _seriesDescription = value; }
        }

        [DicomField(DicomTags.NumberofSeriesRelatedInstances, DefaultValue = DicomFieldDefault.Null)]
        public int NumberOfSeriesRelatedInstances
        {
            get { return _numberOfSeriesRelatedInstances; }
            set { _numberOfSeriesRelatedInstances = value; }
        }

        [DicomField(DicomTags.PerformedProcedureStepStartDate, DefaultValue = DicomFieldDefault.Null)]
        public string PerformedProcedureStepStartDate
        {
            get { return _performedProcedureStepStartDate; }
            set { _performedProcedureStepStartDate = value; }
        }

        [DicomField(DicomTags.PerformedProcedureStepStartTime, DefaultValue = DicomFieldDefault.Null)]
        public string PerformedProcedureStepStartTime
        {
            get { return _performedProcedureStepStartTime; }
            set { _performedProcedureStepStartTime = value; }
        }
        public short StatusEnum
        {
            get { return _statusEnum; }
            set { _statusEnum = value; }
        }
        #endregion
    }
}
