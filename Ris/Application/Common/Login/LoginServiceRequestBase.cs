using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Login
{
    [DataContract]
    public abstract class LoginServiceRequestBase : DataContractBase
    {
        protected LoginServiceRequestBase(string userName, string clientIP)
        {
            this.UserName = userName;
            this.ClientIP = clientIP;
        }

        /// <summary>
        /// UserName. Required.
        /// </summary>
        [DataMember]
        public string UserName;

        /// <summary>
        /// IP address of the client workstation for auditing purposes. Optional.
        /// </summary>
        [DataMember]
        public string ClientIP;
    }
}
