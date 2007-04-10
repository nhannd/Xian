using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;
using ClearCanvas.Enterprise.Authentication;

namespace ClearCanvas.Ris.Application.Services.Admin.AuthenticationAdmin
{
    internal class AuthorityTokenAssembler
    {
        internal AuthorityTokenSummary GetAuthorityTokenSummary(AuthorityToken authorityToken)
        {
            return new AuthorityTokenSummary(
                authorityToken.GetRef(),
                authorityToken.Name,
                authorityToken.Description
                );
        }
    }
}
