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
using System.Threading;
using System.Web.Caching;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.Web.Enterprise.Authentication;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class StudyAdaptor : BaseAdaptor<Study, IStudyEntityBroker, StudySelectCriteria, StudyUpdateColumns>
    {
        private const string CacheKeyDataAccessSubCriteria = "DataAccessSubCriteria";
        private readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(15);


        protected override void OnQuerying(IPersistenceContext context, StudySelectCriteria criteria)
        {
            StudyDataAccessSelectCriteria subCriteria = GetDataAccessSubCriteriaForUser(context);
            if (subCriteria != null)
                criteria.StudyDataAccessRelatedEntityCondition.Exists(subCriteria);
        }
        
        private StudyDataAccessSelectCriteria GetDataAccessSubCriteriaForUser(IPersistenceContext context)
        {
            if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.DataAccess.AllStudies))
            {
                return null;
            }

            var principal = Thread.CurrentPrincipal as CustomPrincipal;
            if (principal == null)
                return null;

            // check the cache first
            var subCriteria = System.Web.HttpContext.Current.Cache[GetDataAccessSubCriteriaCacheID(principal)] as StudyDataAccessSelectCriteria;
            if (subCriteria != null)
                return subCriteria;

            
            var oidList = new List<ServerEntityKey>();
            foreach (var oid in principal.Credentials.DataAccessAuthorityGroups)
                oidList.Add(new ServerEntityKey("OID", oid));
            var dataAccessGroupSelectCriteria = new DataAccessGroupSelectCriteria();
            dataAccessGroupSelectCriteria.AuthorityGroupOID.In(oidList);
                    
            IList<DataAccessGroup> groups;
            var broker = context.GetBroker<IDataAccessGroupEntityBroker>();
            groups = broker.Find(dataAccessGroupSelectCriteria);


            var entityList = new List<ServerEntityKey>();
            foreach (DataAccessGroup group in groups)
            {
                entityList.Add(group.Key);
            }

            subCriteria = new StudyDataAccessSelectCriteria();
            subCriteria.DataAccessGroupKey.In(entityList);

            // put into cache for re-use
            System.Web.HttpContext.Current.Cache.Add(GetDataAccessSubCriteriaCacheID(principal), subCriteria, null,
                                                     Cache.NoAbsoluteExpiration,
                                                     CacheDuration, CacheItemPriority.Normal, null);

            return subCriteria;
        }

        private static string GetDataAccessSubCriteriaCacheID(CustomPrincipal principal)
        {
            return CacheKeyDataAccessSubCriteria + principal.SessionTokenId;
        }
    }
}
