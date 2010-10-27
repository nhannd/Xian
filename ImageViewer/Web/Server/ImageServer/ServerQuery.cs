#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;


namespace ClearCanvas.ImageViewer.Web.Server.ImageServer
{
    abstract class ServerQuery : IDisposable
    {
        public delegate void ServerQueryResultDelegate(DicomAttributeCollection row);

        #region Private members
        private bool _cancelReceived;
        private readonly object _syncLock = new object();
        #endregion

        
        

        #region Public Properties
        public ServerPartition Partition
        {
            get;
            set;
        }
        
        public bool CancelReceived
        {
            get
            {
                lock (_syncLock)
                    return _cancelReceived;
            }
            set
            {
                lock (_syncLock)
                    _cancelReceived = value;
            }
        }
        #endregion

        #region Constructors
        protected ServerQuery(ServerPartition partition)
        {
            Partition = partition;
        }
        #endregion

        public abstract void Query(DicomAttributeCollection query, ServerQueryResultDelegate del);

        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for a <see cref="ServerEntityKey"/> reference.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="vals"></param>
        protected static void SetKeyCondition(ISearchCondition<ServerEntityKey> cond, ServerEntityKey[] vals)
        {
            if (vals == null || vals.Length == 0)
                return;

            if (vals.Length == 1)
                cond.EqualTo(vals[0]);
            else
                cond.In(vals);
        }

        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for an array of matching string values.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="vals"></param>
        protected static void SetStringArrayCondition(ISearchCondition<string> cond, string[] vals)
        {
            if (vals == null || vals.Length == 0)
                return;

            if (vals.Length == 1)
                cond.EqualTo(vals[0]);
            else
                cond.In(vals);
        }

        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for a DICOM range matching string value.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="val"></param>
        protected static void SetRangeCondition(ISearchCondition<string> cond, string val)
        {
            if (val.Length == 0)
                return;

            if (val.Contains("-"))
            {
                string[] vals = val.Split(new[] {'-'});
                if (val.IndexOf('-') == 0)
                    cond.LessThanOrEqualTo(vals[1]);
                else if (val.IndexOf('-') == val.Length - 1)
                    cond.MoreThanOrEqualTo(vals[0]);
                else
                    cond.Between(vals[0], vals[1]);
            }
            else
                cond.EqualTo(val);
        }

        /// <summary>
        /// Set a <see cref="ISearchCondition{T}"/> for DICOM string based (wildcard matching) value.
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="val"></param>
        protected static void SetStringCondition(ISearchCondition<string> cond, string val)
        {
            if (val.Length == 0)
                return;

            if (val.Contains("*") || val.Contains("?"))
            {
				String value = val.Replace("%", "[%]").Replace("_", "[_]");
				value = value.Replace('*', '%');
                value = value.Replace('?', '_');
                cond.Like(value);
            }
            else
                cond.EqualTo(val);
        }

        /// <summary>
        /// Find the <see cref="ServerEntityKey"/> reference for a given Study Instance UID and Server Partition.
        /// </summary>
        /// <param name="read">The connection to use to read the values.</param>
        /// <param name="studyInstanceUid">The list of Study Instance Uids for which to retrieve the table keys.</param>
        /// <returns>A list of <see cref="ServerEntityKey"/>s.</returns>
        protected List<ServerEntityKey> LoadStudyKey(IPersistenceContext read, string[] studyInstanceUid)
        {
            IStudyEntityBroker find = read.GetBroker<IStudyEntityBroker>();

            StudySelectCriteria criteria = new StudySelectCriteria();

            if (Partition!=null)
                criteria.ServerPartitionKey.EqualTo(Partition.Key);

            if (studyInstanceUid.Length > 1)
                criteria.StudyInstanceUid.In(studyInstanceUid);
            else
                criteria.StudyInstanceUid.EqualTo(studyInstanceUid[0]);

            IList<Study> list = find.Find(criteria);

            List<ServerEntityKey> serverList = new List<ServerEntityKey>();

            foreach (Study row in list)
                serverList.Add(row.GetKey());

            return serverList;
        }

        public void Dispose()
        {
            
        }
    }
}



