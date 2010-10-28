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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Used to create/update/delete server partition entries in the database.
    /// </summary>
    public class ServerPartitionDataAdapter :
        BaseAdaptor
            <ServerPartition, IServerPartitionEntityBroker, ServerPartitionSelectCriteria, ServerPartitionUpdateColumns>
    {
        #region Public methods

        /// <summary>
        /// Gets a list of all server partitions.
        /// </summary>
        /// <returns></returns>
        public IList<ServerPartition> GetServerPartitions()
        {
            return Get();
        }

        public IList<ServerPartition> GetServerPartitions(ServerPartitionSelectCriteria criteria)
        {
            return Get(criteria);
        }

        public ServerPartition GetServerPartition(ServerEntityKey key)
        {
            return Get(key);
        }

        /// <summary>
        /// Creats a new server parition.
        /// </summary>
        /// <param name="partition"></param>
        public bool AddServerPartition(ServerPartition partition)
        {
            bool ok;

            using (IUpdateContext ctx = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IInsertServerPartition insert = ctx.GetBroker<IInsertServerPartition>();
                ServerPartitionInsertParameters parms = new ServerPartitionInsertParameters();
                parms.AeTitle = partition.AeTitle;
                parms.Description = partition.Description;
                parms.Enabled = partition.Enabled;
                parms.PartitionFolder = partition.PartitionFolder;
                parms.Port = partition.Port;
                parms.DefaultRemotePort = partition.DefaultRemotePort;
                parms.AutoInsertDevice = partition.AutoInsertDevice;
                parms.AcceptAnyDevice = partition.AcceptAnyDevice;
                parms.DuplicateSopPolicyEnum = partition.DuplicateSopPolicyEnum;
                parms.MatchPatientsName = partition.MatchPatientsName;
                parms.MatchPatientId = partition.MatchPatientId;
                parms.MatchPatientsBirthDate = partition.MatchPatientsBirthDate;
                parms.MatchAccessionNumber = partition.MatchAccessionNumber;
                parms.MatchIssuerOfPatientId = partition.MatchIssuerOfPatientId;
                parms.MatchPatientsSex = partition.MatchPatientsSex;
                parms.AuditDeleteStudy = partition.AuditDeleteStudy;
                try
                {
                    ServerPartition insertPartition = insert.FindOne(parms);
					ok = insertPartition != null;
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Error while inserting server partition.");
                    ok = false;
                }

                if (ok)
                    ctx.Commit();
            }

            return ok;
        }

        public bool Update(ServerPartition partition)
        {
            ServerPartitionUpdateColumns parms = new ServerPartitionUpdateColumns();
            parms.AeTitle = partition.AeTitle;
            parms.Description = partition.Description;
            parms.Enabled = partition.Enabled;
            parms.PartitionFolder = partition.PartitionFolder;
            parms.Port = partition.Port;
            parms.AcceptAnyDevice = partition.AcceptAnyDevice;
            parms.AutoInsertDevice = partition.AutoInsertDevice;
            parms.DefaultRemotePort = partition.DefaultRemotePort;
            parms.DuplicateSopPolicyEnum = partition.DuplicateSopPolicyEnum;
            parms.MatchPatientsName = partition.MatchPatientsName;
            parms.MatchPatientId = partition.MatchPatientId;
            parms.MatchPatientsBirthDate = partition.MatchPatientsBirthDate;
            parms.MatchAccessionNumber = partition.MatchAccessionNumber;
            parms.MatchIssuerOfPatientId = partition.MatchIssuerOfPatientId;
            parms.MatchPatientsSex = partition.MatchPatientsSex;
            parms.AuditDeleteStudy = partition.AuditDeleteStudy;

            return Update(partition.Key, parms);
        }

        #endregion Public methods
    }
}

