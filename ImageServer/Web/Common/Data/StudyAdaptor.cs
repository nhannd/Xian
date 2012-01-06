#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Threading;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class StudyAdaptor : BaseAdaptor<Study, IStudyEntityBroker, StudySelectCriteria, StudyUpdateColumns>
    {
        protected override void OnQuerying(IPersistenceContext context, StudySelectCriteria criteria)
        {
            StudyDataAccessSelectCriteria subCriteria = DataAccessHelper.GetDataAccessSubCriteriaForUser(context, Thread.CurrentPrincipal);
            if (subCriteria != null)
                criteria.StudyDataAccessRelatedEntityCondition.Exists(subCriteria);
        }
        
    }

    
}
