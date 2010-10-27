#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
		private ServerEntityKey _studyStorageKey;
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
		[EntityFieldDatabaseMappingAttribute(TableName = "InstanceKeys", ColumnName = "StudyStorageGUID")]
		public ServerEntityKey StudyStorageKey
		{
			get { return _studyStorageKey; }
			set { _studyStorageKey = value; }
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
