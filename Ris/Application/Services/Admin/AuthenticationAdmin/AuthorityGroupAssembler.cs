using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services.Admin.AuthenticationAdmin
{
    internal class AuthorityGroupAssembler
    {
        internal AuthorityGroupSummary GetAuthorityGroupSummary(AuthorityGroup authorityGroup)
        {
            return new AuthorityGroupSummary(
                authorityGroup.GetRef(),
                authorityGroup.Name
                );
        }

        internal AuthorityGroupDetail GetAuthorityGroupDetail(AuthorityGroup authorityGroup)
        {
            List<AuthorityTokenSummary> tokens = new List<AuthorityTokenSummary>();

            AuthorityTokenAssembler assembler = new AuthorityTokenAssembler();
            foreach (AuthorityToken token in authorityGroup.AuthorityTokens)
            {
                tokens.Add(assembler.GetAuthorityTokenSummary(token));
            }

            return new AuthorityGroupDetail(
                authorityGroup.Name,
                tokens
                );
        }

        internal void UpdateAuthorityGroup(AuthorityGroup authorityGroup, AuthorityGroupDetail detail, IPersistenceContext persistenceContext)
        {
            authorityGroup.Name = detail.Name;

            authorityGroup.AuthorityTokens.Clear();
            foreach (AuthorityTokenSummary summary in detail.AuthorityTokens)
            {
                authorityGroup.AuthorityTokens.Add(persistenceContext.Load<AuthorityToken>(summary.EntityRef));
            }
        }
    }
}
