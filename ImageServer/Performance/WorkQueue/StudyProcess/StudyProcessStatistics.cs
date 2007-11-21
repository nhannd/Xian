using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageServer.Performance.WorkQueue.StudyProcess
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
        private long _engineLoadTime;
        private long _engineTotalExecTime;
        private long _insertStreamTotalTime;
        private long _insertDBTotalTime;
        private long _dicomFileLoadTime;

        private long _engineLoadStart;
        private long _engineExecStart;
        private long _insertStreamStart;
        private long _insertDBStart;
        private long _dicomFileLoadStart;

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

        public long EngineLoadTime
        {
            set
            {
                _engineLoadTime = value;
                this["EnginesLoadInMs"] = string.Format("{0}", value/10000.0d);
            }
            get
            {
                return _engineLoadTime;
            }
        }

        public long EngineExecutionTime
        {
            set
            {
                _engineTotalExecTime = value;
                //this["RuleEngineExecInMs"] = string.Format("{0}", value/10000.0d);
            }
            get
            {
                return _engineTotalExecTime;
            }
        }

        public long InsertStreamTotalTime
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

        public long InsertDBTotalTime
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

        public long DicomFileLoadtime
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

        #region Public methods
        public void EngineLoadBegin()
        {
            _engineLoadStart = DateTime.Now.Ticks;
        }

        public void EngineLoadEnd()
        {
            EngineLoadTime += DateTime.Now.Ticks - _engineLoadStart;
        }

        public void EngineExecutionBegin()
        {
            _engineExecStart = DateTime.Now.Ticks;
        }

        public void EngineExecutionEnd()
        {
            EngineExecutionTime += DateTime.Now.Ticks - _engineExecStart;
        }

        public void InsertStreamBegin()
        {
            _insertStreamStart = DateTime.Now.Ticks;
        }

        public void InsertStreamEnd()
        {
            InsertStreamTotalTime += DateTime.Now.Ticks - _insertStreamStart;
        }
        public void InsertDBBegin()
        {
            _insertDBStart = DateTime.Now.Ticks;
        }

        public void InsertDBEnd()
        {
            InsertDBTotalTime += DateTime.Now.Ticks - _insertDBStart;
        }

        public void DicomFileLoadBegin(string path)
        {
            _dicomFileLoadStart = DateTime.Now.Ticks;
            System.IO.FileInfo fi = new System.IO.FileInfo(path);
            TotalFileSizeInMB += (fi.Length / (1024d * 1024d));
        }

        public void DicomFileLoadEnd(DicomFile file)
        {
            DicomFileLoadtime += DateTime.Now.Ticks - _dicomFileLoadStart;

            StudyInstanceUID = file.DataSet[DicomTags.StudyInstanceUid].GetString(0, "");
            ModalityType = file.DataSet[DicomTags.Modality].GetString(0, "");

        }

        protected override void OnEnd()
        {
            base.OnEnd();

            if (NumInstances>0)
            {
                // derive the average process times
                AverageProcessTimePerInstance = (double)base.ElapsedTimeInMs / NumInstances;
                AverageDicomFileLoadTime = DicomFileLoadtime / NumInstances / 10000d;
                AverageFileSizeInMB = TotalFileSizeInMB / NumInstances;
                AverageEngineExecutionTimePerInstance = (double)EngineExecutionTime / NumInstances / 10000d;
                AverageInsertDBPerInstance = (double)InsertDBTotalTime / NumInstances / 10000d;
                AverageInsertStreamPerInstance = (double)InsertStreamTotalTime / NumInstances / 10000d;
            }

        }

        #endregion Public methods
    }
}
