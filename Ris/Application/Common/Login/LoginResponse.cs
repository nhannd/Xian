using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Login
{
    [DataContract]
    public class LoginResponse : DataContractBase
    {
        public LoginResponse(string[] userAuthorityTokens)
        {
            this.UserAuthorityTokens = userAuthorityTokens;
        }

        /// <summary>
        /// Set of authority tokens granted to the user
        /// </summary>
        [DataMember]
        public string[] UserAuthorityTokens;
    }
}
