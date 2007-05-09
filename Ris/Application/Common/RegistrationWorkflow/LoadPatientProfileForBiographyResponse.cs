using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class LoadPatientProfileForBiographyResponse : DataContractBase
    {
        public LoadPatientProfileForBiographyResponse(
            EntityRef patientRef, 
            EntityRef patientProfileRef, 
            PatientProfileDetail patientDetail, 
            List<AlertNotificationDetail> alertNotifications,
            bool hasReconciliationCandidates)
        {
            this.PatientRef = patientRef;
            this.PatientProfileRef = patientProfileRef;
            this.PatientDetail = patientDetail;
            this.AlertNotifications = alertNotifications;
            this.HasReconciliationCandidates = hasReconciliationCandidates;
        }

        [DataMember]
        public EntityRef PatientRef;

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public PatientProfileDetail PatientDetail;

        [DataMember]
        public List<AlertNotificationDetail> AlertNotifications;

        [DataMember]
        public bool HasReconciliationCandidates;
    }
}
