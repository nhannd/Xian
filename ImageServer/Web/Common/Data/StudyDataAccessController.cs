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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.Web.Enterprise.Admin;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class StudyDataAccessSummary
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string AuthorityGroupOID { set; get; }

        public StudyDataAccess StudyDataAccess { get; set; }
    }

    public class StudyDataAccessController
    {
        //private readonly StudyDataAccessAdaptor _adaptor = new StudyDataAccessAdaptor();


        private static Dictionary<ServerEntityKey,AuthorityGroupSummary> LoadAuthorityGroups(out List<AuthorityGroupSummary> otherSummaries )
        {
            Dictionary<ServerEntityKey, AuthorityGroupSummary> dic = new Dictionary<ServerEntityKey, AuthorityGroupSummary>();
            var summaries = new List<AuthorityGroupSummary>();

            using (var service = new AuthorityRead())
            {
                IList<AuthorityGroupSummary> tokens = service.ListDataAccessAuthorityGroups();
                CollectionUtils.ForEach(tokens, delegate(AuthorityGroupSummary group)
                                                    {
                                                        DataAccessGroupSelectCriteria select = new DataAccessGroupSelectCriteria();
                                                        select.AuthorityGroupOID.EqualTo(new ServerEntityKey("AuthorityGroupOID", new Guid(group.AuthorityGroupRef.ToString(false,false))));
                                                        IDataAccessGroupEntityBroker broker = HttpContextData.Current.ReadContext.GetBroker<IDataAccessGroupEntityBroker>();
                                                        DataAccessGroup accessGroup = broker.FindOne(select);
                                                        if (accessGroup != null)
                                                        {
                                                            dic.Add(accessGroup.Key, group);    
                                                        }
                                                        else
                                                        {
                                                            summaries.Add(group);
                                                        }
                                                    });
            }

            otherSummaries = summaries;

            return dic;
        }

        public DataAccessGroup FindDataAccessGroup(string oid)
        {
            DataAccessGroupSelectCriteria select = new DataAccessGroupSelectCriteria();
            select.AuthorityGroupOID.EqualTo(new ServerEntityKey("AuthorityGroupOID", new Guid(oid)));
            IDataAccessGroupEntityBroker broker = HttpContextData.Current.ReadContext.GetBroker<IDataAccessGroupEntityBroker>();
            return broker.FindOne(select);
        }

        public StudyDataAccess FindStudyDataAccess(ServerEntityKey studyStorageKey, ServerEntityKey dataAccessKey)
        {
            StudyDataAccessSelectCriteria select = new StudyDataAccessSelectCriteria();
            select.StudyStorageKey.EqualTo(studyStorageKey);
            select.DataAccessGroupKey.EqualTo(dataAccessKey);
            IStudyDataAccessEntityBroker broker = HttpContextData.Current.ReadContext.GetBroker<IStudyDataAccessEntityBroker>();
            return broker.FindOne(select);
        }

        public DataAccessGroup AddDataAccessIfNotExists(string oid)
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

        public IList<StudyDataAccessSummary> LoadStudyDataAccess(ServerEntityKey studyStorageKey)
        {
            List<AuthorityGroupSummary> nonAddedAuthorityGroups;
            Dictionary<ServerEntityKey, AuthorityGroupSummary> dictionary = LoadAuthorityGroups(out nonAddedAuthorityGroups);
            return LoadStudyDataAccess(dictionary, studyStorageKey);
        }

        public IList<StudyDataAccessSummary> LoadStudyDataAccess(Dictionary<ServerEntityKey, AuthorityGroupSummary> dictionary, ServerEntityKey studyStorageKey)
        {
            List<StudyDataAccessSummary> summaryList = new List<StudyDataAccessSummary>();

            StudyDataAccessSelectCriteria select = new StudyDataAccessSelectCriteria();
            select.StudyStorageKey.EqualTo(studyStorageKey);
            IStudyDataAccessEntityBroker broker = HttpContextData.Current.ReadContext.GetBroker<IStudyDataAccessEntityBroker>();             

            broker.Find(select, delegate(StudyDataAccess dataAccess)
                                              {
                                                  AuthorityGroupSummary groupSummary;
                                                  if (dictionary.TryGetValue(dataAccess.DataAccessGroupKey, out groupSummary))
                                                  {
                                                      summaryList.Add(new StudyDataAccessSummary
                                                                          {
                                                                              Description = groupSummary.Description,
                                                                              Name = groupSummary.Name,
                                                                              AuthorityGroupOID = groupSummary.AuthorityGroupRef.ToString(false,false), 
                                                                              StudyDataAccess = dataAccess
                                                                          });
                                                  }
                                              });

            return summaryList;              
        }

        public bool UpdateStudyAuthorityGroups(string studyInstanceUid, string accessionNumber, ServerEntityKey studyStorageKey, IList<string> assignedGroupOids)
        {
            List<AuthorityGroupSummary> nonAddedAuthorityGroups;
            Dictionary<ServerEntityKey,AuthorityGroupSummary> dic = LoadAuthorityGroups(out nonAddedAuthorityGroups);
            IList<StudyDataAccessSummary> assignedList = LoadStudyDataAccess(dic, studyStorageKey);

            List<string> groupList = new List<string>();
            foreach (StudyDataAccessSummary summary in assignedList)
            {
                bool found = false;
                foreach (var oid in assignedGroupOids)
                {
                    if (summary.AuthorityGroupOID.Equals(oid))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        IStudyDataAccessEntityBroker broker = update.GetBroker<IStudyDataAccessEntityBroker>();
                        broker.Delete(summary.StudyDataAccess.Key);
                        update.Commit();
                    }

                    groupList.Add(summary.Description);
                }
            }

            if (groupList.Count > 0)
            {
                ServerAuditHelper.RemoveAuthorityGroupAccess(studyInstanceUid, accessionNumber, groupList);
                groupList.Clear();
            }

            foreach (var oid in assignedGroupOids)
            {
                bool found = false;
                foreach (StudyDataAccessSummary summary in assignedList)
                {
                    if (summary.AuthorityGroupOID.Equals(oid))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    DataAccessGroup accessGroup = AddDataAccessIfNotExists(oid);

                    using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        StudyDataAccessUpdateColumns insertColumns = new StudyDataAccessUpdateColumns
                                                                         {
                                                                             DataAccessGroupKey = accessGroup.Key,
                                                                             StudyStorageKey = studyStorageKey
                                                                         };

                        IStudyDataAccessEntityBroker insert = updateContext.GetBroker<IStudyDataAccessEntityBroker>();
                        insert.Insert(insertColumns);
                        updateContext.Commit();
                    }

                    foreach (AuthorityGroupSummary group in nonAddedAuthorityGroups)
                    {
                        if (group.AuthorityGroupRef.ToString(false,false).Equals(accessGroup.AuthorityGroupOID.Key.ToString()))
                            groupList.Add(group.Name);
                    }
                }
            }

            if (groupList.Count > 0)
                ServerAuditHelper.AddAuthorityGroupAccess(studyInstanceUid, accessionNumber, groupList);

            return true;
        }

        public bool AddStudyAuthorityGroups(string studyInstanceUid, string accessionNumber, ServerEntityKey studyStorageKey, IList<string> assignedGroupOids)
        {
            List<AuthorityGroupSummary> nonAddedSummaries;
            Dictionary<ServerEntityKey, AuthorityGroupSummary> dic = LoadAuthorityGroups(out nonAddedSummaries);
            IList<StudyDataAccessSummary> assignedList = LoadStudyDataAccess(dic, studyStorageKey);

            List<string> assignedGroups = new List<string>();
            foreach (var oid in assignedGroupOids)
            {
                bool found = false;
                foreach (StudyDataAccessSummary summary in assignedList)
                {
                    if (summary.AuthorityGroupOID.Equals(oid))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    DataAccessGroup accessGroup = AddDataAccessIfNotExists(oid);

                    using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        StudyDataAccessUpdateColumns insertColumns = new StudyDataAccessUpdateColumns
                        {
                            DataAccessGroupKey = accessGroup.Key,
                            StudyStorageKey = studyStorageKey
                        };

                        IStudyDataAccessEntityBroker insert = updateContext.GetBroker<IStudyDataAccessEntityBroker>();
                        insert.Insert(insertColumns);
                        updateContext.Commit();
                    }

                      AuthorityGroupSummary groupSummary;
                      if (dic.TryGetValue(accessGroup.Key, out groupSummary))
                      {
                          assignedGroups.Add(groupSummary.Name);
                      }
                }
            }
            if (assignedGroups.Count > 0)
                ServerAuditHelper.AddAuthorityGroupAccess(studyInstanceUid, accessionNumber, assignedGroups);
            return true;
        }
    }
}
