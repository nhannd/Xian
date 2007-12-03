#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Store performance statistics of a study processor.
    /// </summary>
    internal class StudyProcessStatistics : TimeSpanStatistics
    {
        #region Private members
        private string _modalityType;
        private string _studyInstanceUid;
        private int _numInstances = 0;
        private double _engineLoadTime;
        private double _engineTotalExecTime;
        private double _insertStreamTotalTime;
        private double _insertDBTotalTime;
        private double _dicomFileLoadTime;
        private double _totalFileSizeInMB;
        #endregion Private members

        #region Public Properties

        public string StudyInstanceUid
        {
            set
            {
                _studyInstanceUid = value;
                this["StudyInstanceUid"] = value;
            }
            get { return _studyInstanceUid; }
        }

        public string ModalityType
        {
            set
            {
                _modalityType = value;
                this["Modality"] = value;
            }
            get { return _modalityType; }
        }

        public int NumInstances
        {
            set
            {
                _numInstances = value;
                this["InstanceCount"] = value;
            }
            get { return _numInstances; }
        }

        public double EngineLoadTimeInMs
        {
            set
            {
                _engineLoadTime = value;
                this["EnginesLoadInMs"] = string.Format("{0:0.00}", value);
            }
            get { return _engineLoadTime; }
        }

        public double EngineExecutionTimeInMs
        {
            set { _engineTotalExecTime = value; }
            get { return _engineTotalExecTime; }
        }

        public double InsertStreamTotalTimeInMs
        {
            set
            {
                _insertStreamTotalTime = value;
                //this["InsertStreamTimeInMs"] = string.Format("{0}", value / 10000.0d);
            }
            get { return _insertStreamTotalTime; }
        }

        public double InsertDBTotalTimeInMs
        {
            set
            {
                _insertDBTotalTime = value;
                //this["DatabaseInsertTimeInMs"] = string.Format("{0}", value / 10000.0d);
            }
            get { return _insertDBTotalTime; }
        }

        public double DicomFileLoadTimeInMs
        {
            set
            {
                _dicomFileLoadTime = value;
                //this["DicomFileLoadTime"] = string.Format("{0}", value / 10000.0d);
            }
            get { return _dicomFileLoadTime; }
        }

        public double AverageProcessTimePerInstance
        {
            set { this["AverageInstanceProcessTimeInMs"] = string.Format("{0:0.00}", value); }
        }

        public double AverageEngineExecutionTimePerInstance
        {
            set { this["AverageEngineExecTimeInMs"] = string.Format("{0:0.00}", value); }
        }

        public double AverageInsertStreamPerInstance
        {
            set { this["AverageStreamInsertTimeInMs"] = string.Format("{0:0.00}", value); }
        }

        public double AverageInsertDBPerInstance
        {
            set { this["AverageDBInsertTimeInMs"] = string.Format("{0:0.00}", value); }
        }

        public double AverageDicomFileLoadTime
        {
            set { this["AverageDicomFileLoadTime"] = string.Format("{0:0.00}", value); }
        }

        public double TotalFileSizeInMB
        {
            set
            {
                _totalFileSizeInMB = value;
                //this["TotalFileSizeInMB"] = string.Format("{0:0.00}", value);
            }
            get { return _totalFileSizeInMB; }
        }

        public double AverageFileSizeInMB
        {
            set { this["AverageFileSizeInMB"] = string.Format("{0:0.00}", value); }
        }

        #endregion Public Properties

        #region Constructors
        public StudyProcessStatistics() : base("Study Process")
        {
        }
        #endregion Constructors

        #region Overrides
        protected override void OnEnd()
        {
            base.OnEnd();

            if (NumInstances > 0)
            {
                // derive the average process times
                AverageProcessTimePerInstance = (double) base.ElapsedTimeInMs/NumInstances;
                AverageDicomFileLoadTime = DicomFileLoadTimeInMs/NumInstances;
                AverageFileSizeInMB = TotalFileSizeInMB/NumInstances;
                AverageEngineExecutionTimePerInstance = EngineExecutionTimeInMs/NumInstances;
                AverageInsertDBPerInstance = InsertDBTotalTimeInMs/NumInstances;
                AverageInsertStreamPerInstance = InsertStreamTotalTimeInMs/NumInstances;
            }
        }
        #endregion
    }
}
