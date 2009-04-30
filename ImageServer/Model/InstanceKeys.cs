#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using ClearCanvas.ImageServer.Enterprise;

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
        private bool _insertPatient;
        private bool _insertStudy;
        private bool _insertSeries;
        #endregion

        #region Public Properties
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "ServerPartitionGUID")]
		public ServerEntityKey ServerPartitionKey
		{
			get { return _serverPartitionKey; }
			set { _serverPartitionKey = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "PatientGUID")]
		public ServerEntityKey PatientKey
		{
			get { return _patientKey; }
			set { _patientKey = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "StudyGUID")]
		public ServerEntityKey StudyKey
		{
			get { return _studyKey; }
			set { _studyKey = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "SeriesGUID")]
		public ServerEntityKey SeriesKey
		{
			get { return _seriesKey; }
			set { _seriesKey = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "InsertPatient")]
		public bool InsertPatient
		{
			get { return _insertPatient; }
			set { _insertPatient = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "InsertStudy")]
		public bool InsertStudy
		{
			get { return _insertStudy; }
			set { _insertStudy = value; }
		}
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "InsertSeries")]
		public bool InsertSeries
		{
			get { return _insertSeries; }
			set { _insertSeries = value; }
		}
        #endregion
    }
}
