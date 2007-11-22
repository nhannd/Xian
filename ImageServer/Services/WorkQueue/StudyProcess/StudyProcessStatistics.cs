using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    
    /// <summary>
    /// Store performance statistics of a study processor.
    /// </summary>
    class StudyProcessStatistics:TimeSpanStatistics
    {
        #region Private members
        private string _modalityType;
        private string _studyInstanceUID;
        private int _numInstances = 0;
        private double _engineLoadTime;
        private double _engineTotalExecTime;
        private double _insertStreamTotalTime;
        private double _insertDBTotalTime;
        private double _dicomFileLoadTime;

        private double _totalFileSizeInMB;

        #endregion Private members

        #region public properties
        public string StudyInstanceUID
        {
            set { _studyInstanceUID = value;
                this["StudyInstanceUID"] = value;
            }
            get
            {
                return _studyInstanceUID;
            }
        }
        public string ModalityType
        {
            set
            {
                _modalityType = value;
                this["Modality"] = value;
            }
            get
            {
                return _modalityType;
            }
        }

        public int NumInstances
        {
            set
            {
                _numInstances = value;
                this["InstanceCount"] = value;
            }
            get
            {
                return _numInstances;
            }
        }

        public double EngineLoadTimeInMs
        {
            set
            {
                _engineLoadTime = value;
                this["EnginesLoadInMs"] = string.Format("{0:0.00}", value);
            }
            get
            {
                return _engineLoadTime;
            }
        }

        public double EngineExecutionTimeInMs
        {
            set
            {
                _engineTotalExecTime = value;
            }
            get
            {
                return _engineTotalExecTime;
            }
        }

        public double InsertStreamTotalTimeInMs
        {
            set
            {
                _insertStreamTotalTime = value;
                //this["InsertStreamTimeInMs"] = string.Format("{0}", value / 10000.0d);
            }
            get
            {
                return _insertStreamTotalTime;
            }
        }

        public double InsertDBTotalTimeInMs
        {
            set
            {
                _insertDBTotalTime = value;
                //this["DatabaseInsertTimeInMs"] = string.Format("{0}", value / 10000.0d);
            }
            get
            {
                return _insertDBTotalTime;
            }
        }

        public double DicomFileLoadtimeInMs
        {
            set
            {
                _dicomFileLoadTime = value;
                //this["DicomFileLoadTime"] = string.Format("{0}", value / 10000.0d);
            }
            get
            {
                return _dicomFileLoadTime;
            }
        }

        public double AverageProcessTimePerInstance
        {
            set
            {
                this["AverageInstanceProcessTimeInMs"] = string.Format("{0:0.00}", value);
            }
        }

        public double AverageEngineExecutionTimePerInstance
        {
            set
            {
                this["AverageEngineExecTimeInMs"] = string.Format("{0:0.00}", value);
            }
        }

        public double AverageInsertStreamPerInstance
        {
            set
            {
                this["AverageStreamInsertTimeInMs"] = string.Format("{0:0.00}", value);
            }
        }
        public double AverageInsertDBPerInstance
        {
            set
            {
                this["AverageDBInsertTimeInMs"] = string.Format("{0:0.00}", value);
            }
        }

        public double AverageDicomFileLoadTime
        {
            set
            {
                this["AverageDicomFileLoadTime"] = string.Format("{0:0.00}", value);
            }
        }

        public double TotalFileSizeInMB
        {
            set
            {
                _totalFileSizeInMB = value;
                //this["TotalFileSizeInMB"] = string.Format("{0:0.00}", value);
            }
            get
            {
                return _totalFileSizeInMB;
            }
        }
        public double AverageFileSizeInMB
        {
            set
            {
                this["AverageFileSizeInMB"] = string.Format("{0:0.00}", value);
            }
        }

        #endregion public properties

        #region Constructors
        public StudyProcessStatistics():base("Study Process")
        {

        }
        #endregion Constructors

        protected override void OnEnd()
        {
            base.OnEnd();

            if (NumInstances>0)
            {
                // derive the average process times
                AverageProcessTimePerInstance = (double)base.ElapsedTimeInMs / NumInstances;
                AverageDicomFileLoadTime = DicomFileLoadtimeInMs / NumInstances;
                AverageFileSizeInMB = TotalFileSizeInMB / NumInstances;
                AverageEngineExecutionTimePerInstance = (double)EngineExecutionTimeInMs / NumInstances;
                AverageInsertDBPerInstance = (double)InsertDBTotalTimeInMs / NumInstances;
                AverageInsertStreamPerInstance = (double)InsertStreamTotalTimeInMs / NumInstances;
            }

        }
    }
}
