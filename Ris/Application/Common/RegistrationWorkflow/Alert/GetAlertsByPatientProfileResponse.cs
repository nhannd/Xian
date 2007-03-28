using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.Alert
{
    [DataContract]
    public class GetAlertsByPatientProfileResponse : DataContractBase
    {
        public GetAlertsByPatientProfileResponse(List<AlertNotificationDetail> alertNotifications)
        {
            this.AlertNotifications = alertNotifications;
        }

        [DataMember]
        public List<AlertNotificationDetail> AlertNotifications;
    }
}
