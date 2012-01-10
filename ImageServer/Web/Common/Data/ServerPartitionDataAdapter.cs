#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.Web.Enterprise.Admin;

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
        public bool AddServerPartition(ServerPartition partition, List<string> groupsWithAccess)
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

                    UpdateDataAccess(ctx, insertPartition, groupsWithAccess);
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

        public DataAccessGroup FindDataAccessGroup(string oid)
        {
            DataAccessGroupSelectCriteria select = new DataAccessGroupSelectCriteria();
            select.AuthorityGroupOID.EqualTo(new ServerEntityKey("AuthorityGroupOID", new Guid(oid)));
            IDataAccessGroupEntityBroker broker = HttpContextData.Current.ReadContext.GetBroker<IDataAccessGroupEntityBroker>();
            return broker.FindOne(select);
        
        }

        private DataAccessGroup AddDataAccessIfNotExists(string oid)
        {
            DataAccessGroup theGroup = FindDataAccessGroup(oid);
            if (theGroup == null)
            {
                using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    DataAccessGroupUpdateColumns insert = new DataAccessGroupUpdateColumns
                    {
                        AuthorityGroupOID =
                            new ServerEntityKey("AuthorityGroupOID",
                                                new Guid(oid)),
                        Deleted = false
                    };
                    IDataAccessGroupEntityBroker broker = update.GetBroker<IDataAccessGroupEntityBroker>();
                    theGroup = broker.Insert(insert);
                    update.Commit();
                }
            }
            return theGroup;
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

        public IEnumerable<ServerPartitionDataAccess> GetServerPartitionDataAccessGroups(ServerPartition partition)
       {
           IServerPartitionDataAccessEntityBroker broker = HttpContextData.Current.ReadContext.GetBroker<IServerPartitionDataAccessEntityBroker>();
           ServerPartitionDataAccessSelectCriteria criteria = new ServerPartitionDataAccessSelectCriteria();
           criteria.ServerPartitionKey.EqualTo(partition.Key);
           return broker.Find(criteria);
       }
       

       private void UpdateDataAccess(IUpdateContext ctx, ServerPartition partition, List<string> groupsWithAccess)
       {
           IServerPartitionDataAccessEntityBroker broker = ctx.GetBroker<IServerPartitionDataAccessEntityBroker>();
           ServerPartitionDataAccessSelectCriteria criteria = new ServerPartitionDataAccessSelectCriteria();
           criteria.ServerPartitionKey.EqualTo(partition.Key);
           
           var existingGroups = broker.Find(criteria);
           if (existingGroups!=null)
           {
               foreach(var g in existingGroups)
               {
                   if (!groupsWithAccess.Contains(g.Key.ToString()))
                   {
                       broker.Delete(g.Key);
                   }
               }
           }

           if (groupsWithAccess!=null)
           {
               foreach (var g in groupsWithAccess)
               {
                   
                   if (!CollectionUtils.Contains(existingGroups, group => group.Key.ToString().Equals(g)))
                   {
                       var dataAccessGroup= AddDataAccessIfNotExists(g);
                       
                       var record = new ServerPartitionDataAccessUpdateColumns
                       {
                           DataAccessGroupKey = dataAccessGroup.Key,
                           ServerPartitionKey = partition.Key
                       };

                       broker.Insert(record);
                   }
               }
           }

          
       }

       public bool Update(ServerPartition partition, List<string> groupsWithAccess)
       {
           try
           {
               using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
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

                   IServerPartitionEntityBroker broker = context.GetBroker<IServerPartitionEntityBroker>();
                   if (!broker.Update(partition.Key, parms))
                       return false;

                   UpdateDataAccess(context, partition, groupsWithAccess);

                   context.Commit();
                   return true;
               }
           }
           catch (Exception ex)
           {
               throw;
           }
       }

        public IList<AuthorityGroupDetail> GetAuthorityGroupsForPartition(ServerEntityKey partitionKey, out IList<AuthorityGroupDetail> allStudiesGroup )
        {
            using (var service = new AuthorityRead())
            {
                IList<AuthorityGroupDetail> tokens = service.ListDataAccessAuthorityGroupDetails();
                IList<AuthorityGroupDetail> resultGroups = new List<AuthorityGroupDetail>();
                var internalAllStudiesGroup = new List<AuthorityGroupDetail>();

                CollectionUtils.ForEach(
                    tokens,
                    delegate(AuthorityGroupDetail group)
                    {
                        bool allPartitions = false;
                        bool allStudies = false;
                        foreach (var token in group.AuthorityTokens)
                        {
                            if (token.Name.Equals(ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens.DataAccess.AllPartitions))
                            {
                                allPartitions = true;                                
                            }
                            else if (token.Name.Equals(ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens.DataAccess.AllStudies))
                            {
                                allStudies = true;
                            }

                            if (allPartitions && allStudies) break;
                        }

                        if (allPartitions && allStudies)
                        {
                            internalAllStudiesGroup.Add(group);
                            return;
                        }

                        if (!allPartitions)
                        {
                            using (IReadContext readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                            {
                                var criteria = new ServerPartitionDataAccessSelectCriteria();
                                criteria.ServerPartitionKey.EqualTo(partitionKey);

                                var dataCriteria = new DataAccessGroupSelectCriteria();
                                dataCriteria.AuthorityGroupOID.EqualTo(new ServerEntityKey("AuthorityGroupOID", new Guid(group.AuthorityGroupRef.ToString(false, false))));
                                dataCriteria.ServerPartitionDataAccessRelatedEntityCondition.Exists(criteria);

                                var broker = readContext.GetBroker<IDataAccessGroupEntityBroker>();
                                if (broker.Count(dataCriteria) == 0)
                                    return;
                            }
                        }

                        if (allStudies)
                        {
                            internalAllStudiesGroup.Add(group);
                            return;
                        }
                        
                        resultGroups.Add(group);
                    });

                allStudiesGroup = internalAllStudiesGroup;
                return resultGroups;
            }
        }

        #endregion Public methods
    }
}

