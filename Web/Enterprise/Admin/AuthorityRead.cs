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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;

namespace ClearCanvas.Web.Enterprise.Admin
{   
     /// <summary>
    /// Wrapper for <see cref="IAuthorityGroupReadService"/> service.
    /// </summary>
    public sealed class AuthorityRead : IDisposable
    {
        private IAuthorityGroupReadService _service;

        public AuthorityRead()
        {
            _service =  Platform.GetService<IAuthorityGroupReadService>();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_service != null && _service is IDisposable)
            {
                (_service as IDisposable).Dispose();
                _service = null;
            }
        }

        #endregion

        public IList<AuthorityGroupSummary> ListAllAuthorityGroups()
        {
            return _service.ListAuthorityGroups(new ListAuthorityGroupsRequest(){ }).AuthorityGroups;
        }

        public IList<AuthorityGroupDetail> ListAllAuthorityGroupDetails()
        {
            return _service.ListAuthorityGroups(new ListAuthorityGroupsRequest() { Details = true }).AuthorityGroupDetails;
        }

        public IList<AuthorityGroupDetail> ListDataAccessAuthorityGroupDetails()
        {
            var rq = new ListAuthorityGroupsRequest
                         {
                             DataGroup = true,
                             Details = true
                         };

            return _service.ListAuthorityGroups(rq).AuthorityGroupDetails;
        }

        public IList<AuthorityGroupSummary> ListDataAccessAuthorityGroups()
        {
            var rq = new ListAuthorityGroupsRequest
                         {
                             DataGroup = true
                         };

            return _service.ListAuthorityGroups(rq).AuthorityGroups;
        }
    }
}
