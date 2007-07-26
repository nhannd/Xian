using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Model
{
    public class Patient : ProcedureEntity
    {
        #region Private Members
        private EntityRef _serverPartitionRef;
        private String _patientName;
        private String _patientId;
        private String _issuerOfPatientId;
        private int _numberOfPatientRelatedStudies;
        private int _numberOfPatientRelatedSeries;
        private int _numberOfPatientRelatedInstances;
        #endregion

        #region Public Properties
        public EntityRef ServerPartitionRef
        {
            get { return _serverPartitionRef; }
            set { _serverPartitionRef = value; }
        }

        public String PatientName
        {
            get { return _patientName; }
            set { _patientName = value; }
        }

        public String PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
                 
        }

        public String IssuerOfPatientId
        {
            get { return _issuerOfPatientId; }
            set { _issuerOfPatientId = value; }
        }

        public int NumberOfPatientRelatedStudies
        {
            get { return _numberOfPatientRelatedStudies; }
            set { _numberOfPatientRelatedStudies = value; }
        }
        public int NumberOfPatientRelatedSeries
        {
            get { return _numberOfPatientRelatedSeries; }
            set { _numberOfPatientRelatedSeries = value; }
        }
        public int NumberOfPatientRelatedInstances
        {
            get { return _numberOfPatientRelatedInstances; }
            set { _numberOfPatientRelatedInstances = value; }
        }
        #endregion

    }
}
