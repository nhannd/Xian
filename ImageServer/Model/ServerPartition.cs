#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model
{
    public class StudyCompareOptions
    {
        private bool _matchIssuerOfPatientId;
        private bool _matchPatientId;
        private bool _matchPatientsName;
        private bool _matchPatientsBirthDate;
        private bool _matchPatientsSex;
        private bool _matchAccessionNumber;

        public bool MatchIssuerOfPatientId
        {
            get { return _matchIssuerOfPatientId; }
            set { _matchIssuerOfPatientId = value; }
        }

        public bool MatchPatientId
        {
            get { return _matchPatientId; }
            set { _matchPatientId = value; }
        }

        public bool MatchPatientsName
        {
            get { return _matchPatientsName; }
            set { _matchPatientsName = value; }
        }

        public bool MatchPatientsBirthDate
        {
            get { return _matchPatientsBirthDate; }
            set { _matchPatientsBirthDate = value; }
        }

        public bool MatchPatientsSex
        {
            get { return _matchPatientsSex; }
            set { _matchPatientsSex = value; }
        }

        public bool MatchAccessionNumber
        {
            get { return _matchAccessionNumber; }
            set { _matchAccessionNumber = value; }
        }
    }

    public partial class ServerPartition
    {
        private readonly object _syncLock = new object();
        bool _dataAccessInfoloaded = false;

        private Dictionary<DataAccessGroup, ServerEntityKey> _mapDataAccessGroupsAuthorityGroups = null;

        IEnumerable<ServerPartitionDataAccess> _dataAccessGroups = null;
         

        public StudyCompareOptions GetComparisonOptions()
        {
            StudyCompareOptions options = new StudyCompareOptions();
            options.MatchAccessionNumber = MatchAccessionNumber;
            options.MatchIssuerOfPatientId = MatchIssuerOfPatientId;
            options.MatchPatientId = MatchPatientId;
            options.MatchPatientsBirthDate = MatchPatientsBirthDate;
            options.MatchPatientsName = MatchPatientsName;
            options.MatchPatientsSex = MatchPatientsSex;

            return options;
        }

        
        public IEnumerable<ServerPartitionDataAccess> DataAccessGroups
        {
            get
            {
                if (_dataAccessGroups==null)
                {
                    using(var ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                    {
                        LoadDataAcessInformation(ctx);    
                    }
                    
                }
                return _dataAccessGroups;
            }
        }

        private Dictionary<DataAccessGroup, ServerEntityKey> MapDataAccessGroupsAuthorityGroups
        {
            get
            {
                if (_mapDataAccessGroupsAuthorityGroups==null)
                {
                    using (var ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                    {

                        LoadAuthorityGroup(ctx);
                    }
                    
                }

                return _mapDataAccessGroupsAuthorityGroups;
            }
        }

        public bool IsAccessAllowed(string authorityGroupOID)
        {
            var dataAccessGroup = FindDataAccessGroup(authorityGroupOID);
            if (dataAccessGroup == null)
                return false;

            var existingGroups = DataAccessGroups;
            if (existingGroups == null)
                return false;

            return CollectionUtils.Contains(existingGroups, g => g.DataAccessGroupKey.Equals(dataAccessGroup.Key)); 
        }

        public void LoadDataAcessInformation(IPersistenceContext context)
        {
            lock(_syncLock)
            {
                if (_dataAccessInfoloaded)
                    return;

                IServerPartitionDataAccessEntityBroker broker = context.GetBroker<IServerPartitionDataAccessEntityBroker>();
                ServerPartitionDataAccessSelectCriteria criteria = new ServerPartitionDataAccessSelectCriteria();
                criteria.ServerPartitionKey.EqualTo(this.Key);
                _dataAccessGroups = broker.Find(criteria);

                _dataAccessInfoloaded = true;
            }
            
           
        }

        private DataAccessGroup FindDataAccessGroup(string authorityGroupOID)
        {
            foreach(var entry in MapDataAccessGroupsAuthorityGroups)
            {
                if (entry.Value.Key.ToString().Equals(authorityGroupOID))
                    return entry.Key;
            }

            return null;
        }

        private void LoadAuthorityGroup(IPersistenceContext context)
        {
            lock (_syncLock)
            {
                _mapDataAccessGroupsAuthorityGroups = new Dictionary<DataAccessGroup, ServerEntityKey>();

                IDataAccessGroupEntityBroker dataAccessBroker = context.GetBroker<IDataAccessGroupEntityBroker>();
                DataAccessGroupSelectCriteria all = new DataAccessGroupSelectCriteria();
                var dataAccessGroups = dataAccessBroker.Find(all);

                foreach (var group in dataAccessGroups)
                {
                    _mapDataAccessGroupsAuthorityGroups.Add(group, group.AuthorityGroupOID);
                }
            }
        }

    }
}
