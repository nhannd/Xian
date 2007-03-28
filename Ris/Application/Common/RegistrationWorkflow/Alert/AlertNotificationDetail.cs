using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.Alert
{
    [DataContract]
    public class AlertNotificationDetail : DataContractBase
    {
        public AlertNotificationDetail(string representation, string severity, string type)
        {
            this.Representation = representation;
            this.Severity = severity;
            this.Type = type;
        }

        [DataMember]
        public string Representation;

        [DataMember]
        public string Severity;

        [DataMember]
        public string Type;
    }
}
