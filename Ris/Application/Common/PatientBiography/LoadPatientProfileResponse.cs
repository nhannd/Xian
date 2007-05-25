using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.PatientBiography
{
    [DataContract]
    public class LoadPatientProfileResponse : DataContractBase
    {
        public LoadPatientProfileResponse(
            EntityRef patientRef, 
            EntityRef patientProfileRef, 
            PatientProfileDetail patientDetail, 
            List<AlertNotificationDetail> alertNotifications)
        {
            this.PatientRef = patientRef;
            this.PatientProfileRef = patientProfileRef;
            this.PatientDetail = patientDetail;
            this.AlertNotifications = alertNotifications;
        }

        [DataMember]
        public EntityRef PatientRef;

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public PatientProfileDetail PatientDetail;

        [DataMember]
        public List<AlertNotificationDetail> AlertNotifications;
    }
}
