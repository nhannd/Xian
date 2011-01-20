#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class FilesystemQueueInsertParameters : ProcedureParameters
    {
        public FilesystemQueueInsertParameters()
            : base("InsertFilesystemQueue")
        {
        }

        public FilesystemQueueTypeEnum FilesystemQueueTypeEnum
        {
            set { SubCriteria["FilesystemQueueTypeEnum"] = new ProcedureParameter<ServerEnum>("FilesystemQueueTypeEnum", value); }
        }
        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }
        public ServerEntityKey FilesystemKey
        {
            set { SubCriteria["FilesystemKey"] = new ProcedureParameter<ServerEntityKey>("FilesystemKey", value); }
        }
        public DateTime ScheduledTime
        {
            set { SubCriteria["ScheduledTime"] = new ProcedureParameter<DateTime>("ScheduledTime", value); }
        }
        public string SeriesInstanceUid
        {
            set { SubCriteria["SeriesInstanceUid"] = new ProcedureParameter<string>("SeriesInstanceUid", value); }
        }
		public XmlDocument QueueXml
		{
			set { SubCriteria["QueueXml"] = new ProcedureParameter<XmlDocument>("QueueXml", value); }
		}
	}
}
