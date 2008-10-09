using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Login
{
    [DataContract]
    public abstract class LoginServiceRequestBase : DataContractBase
    {
        protected LoginServiceRequestBase(string userName, string clientIP, string clientCpuID)
        {
            this.UserName = userName;
            this.ClientIP = clientIP;
            this.ClientCpuID = clientCpuID;
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

        /// <summary>
        /// CPU Id of the client workstation for auditing purposes. Optional.
        /// </summary>
        [DataMember]
        public string ClientCpuID;
    }
}
