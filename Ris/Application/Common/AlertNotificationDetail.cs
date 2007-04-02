using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class AlertNotificationDetail : DataContractBase
    {
        public AlertNotificationDetail(string representation, string severity, string type, List<string> reasons)
        {
            this.Representation = representation;
            this.Severity = severity;
            this.Type = type;
            this.Reasons = reasons;
        }

        public AlertNotificationDetail()
        {
        }

        [DataMember]
        public string Representation;

        [DataMember]
        public string Severity;

        [DataMember]
        public string Type;

        [DataMember]
        public List<string> Reasons;
    }
}
