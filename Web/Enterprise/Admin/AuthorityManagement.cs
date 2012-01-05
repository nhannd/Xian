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
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;

namespace ClearCanvas.Web.Enterprise.Admin
{
    /// <summary>
    /// Wrapper for <see cref="IAuthorityGroupAdminService"/> service.
    /// </summary>
    public sealed class AuthorityManagement : IDisposable
    {
        private IAuthorityGroupAdminService _service;

        public AuthorityManagement()
        {
            _service =  Platform.GetService<IAuthorityGroupAdminService>();
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
            return _service.ListAuthorityGroups(new ListAuthorityGroupsRequest()).AuthorityGroups;
        }

        public IList<AuthorityGroupDetail> ListAllAuthorityGroupDetails()
        {
            return _service.ListAuthorityGroups(new ListAuthorityGroupsRequest {Details = true}).AuthorityGroupDetails;
        }

        public IList<AuthorityGroupSummary> ListDataAccessAuthorityGroups()
        {
            var rq = new ListAuthorityGroupsRequest
                         {
                             DataGroup = true
                         };

            return _service.ListAuthorityGroups(rq).AuthorityGroups;
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

        public void AddAuthorityGroup(string name, string description, bool dataGroup, List<AuthorityTokenSummary> tokens)
        {
            var details = new AuthorityGroupDetail
                              {
                                  Name = name,
                                  Description = description,
                                  DataGroup = dataGroup,
                                  AuthorityTokens = tokens
                              };
            _service.AddAuthorityGroup(new AddAuthorityGroupRequest(details));
        }

        public void UpdateAuthorityGroup(AuthorityGroupDetail detail, string password)
        {
            _service.UpdateAuthorityGroup(new UpdateAuthorityGroupRequest(detail) {Password = password});
        }

        public void DeleteAuthorityGroup(EntityRef entityRef, bool checkIfGroupIsEmpty)
        {
            try
            {
                _service.DeleteAuthorityGroup(new DeleteAuthorityGroupRequest(entityRef) { DeleteOnlyWhenEmpty = checkIfGroupIsEmpty });
            }
            catch(FaultException<ConcurrentModificationException> ex)
            {
                throw ex.Detail;
            }
            catch (FaultException<AuthorityGroupIsNotEmptyException> ex)
            {
                throw ex.Detail;
            }
            catch (FaultException<RequestValidationException> ex)
            {
                throw ex.Detail;
            }
        }

        public void ImportAuthorityTokens(List<AuthorityTokenSummary> tokens)
        {
            _service.ImportAuthorityTokens(new ImportAuthorityTokensRequest(tokens));
        }

        public AuthorityGroupDetail LoadAuthorityGroupDetail(EntityRef entityRef)
        {
            return _service.LoadAuthorityGroupForEdit(new LoadAuthorityGroupForEditRequest(entityRef)).AuthorityGroupDetail;
        }

        public IList<AuthorityTokenSummary> ListAuthorityTokens()
        {
            return _service.ListAuthorityTokens(new ListAuthorityTokensRequest()).AuthorityTokens;
        }

        public bool ImportAuthorityGroups(List<AuthorityGroupDetail> groups)
        {
            var request = new ImportAuthorityGroupsRequest(groups);
            return _service.ImportAuthorityGroups(request)!=null;
        }
    }
}