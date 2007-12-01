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

using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.SelectBrokers;

namespace ClearCanvas.ImageServer.Model
{
    /// <summary>
    /// A model of the Series entity.
    /// </summary>
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
        private StudyStatusEnum _statusEnum;
        #endregion

        #region Public Properties
        public ServerEntityKey ServerPartitionKey
        {
            get { return _serverPartitionKey; }
            set { _serverPartitionKey = value; }
        }
        public ServerEntityKey StudyKey
        {
            get { return _studyKey; }
            set { _studyKey = value; }
        }

        [DicomField(DicomTags.SeriesInstanceUid, DefaultValue = DicomFieldDefault.Null)]
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

        [DicomField(DicomTags.NumberOfSeriesRelatedInstances, DefaultValue = DicomFieldDefault.Null)]
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
        public StudyStatusEnum StudyStatusEnum
        {
            get { return _statusEnum; }
            set { _statusEnum = value; }
        }
        #endregion

        #region Static Methods
        static public Series Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public Series Load(IReadContext read, ServerEntityKey key)
        {
            ISelectSeries broker = read.GetBroker<ISelectSeries>();
            Series theSeries = broker.Load(key);
            return theSeries;
        }
        #endregion
    }
}
