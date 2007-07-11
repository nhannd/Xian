using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PreviewService
{
    [DataContract]
    public class GetPatientAlertsResponse : DataContractBase
    {
        public GetPatientAlertsResponse(List<AlertNotificationDetail> alertNotifications)
        {
            this.AlertNotifications = alertNotifications;
        }

        public GetPatientAlertsResponse()
        {
        }

        [DataMember]
        public List<AlertNotificationDetail> AlertNotifications;
    }
}
