using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model
{
    public class InstanceKeys : ServerEntity
    {
        #region Constructors
        public InstanceKeys()
            : base("InstanceKeys")
        {
        }
        #endregion

        #region Private Members
        private ServerEntityKey _serverPartitionKey;
        private ServerEntityKey _patientKey;
        private ServerEntityKey _studyKey;
        private ServerEntityKey _seriesKey;
        #endregion

        #region Public Properties
        private ServerEntityKey ServerPartitionKey
        {
            get { return _serverPartitionKey; }
            set { _serverPartitionKey = value; }
        }
        private ServerEntityKey PatientKey
        {
            get { return _patientKey; }
            set { _patientKey = value; }
        }
        private ServerEntityKey StudyKey
        {
            get { return _studyKey; }
            set { _studyKey = value; }
        }
        private ServerEntityKey SeriesKey
        {
            get { return _seriesKey; }
            set { _seriesKey = value; }
        }
        #endregion
    }
}
