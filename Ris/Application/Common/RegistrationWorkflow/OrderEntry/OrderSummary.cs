using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class OrderSummary : DataContractBase
    {
        public OrderSummary(EntityRef orderRef,
	  	        string mrnId,
                string mrnAssigningAuthority,
                PersonNameDetail patientName,
                string visitId,
                string visitAssigningAuthority,
                string accessionNumber,
                string diagnosticServiceName,
                string requestedProcedureName,
                string modalityProcedureStepName,
                string modalityName,
                EnumValueInfo orderPriority)
        {
            this.OrderRef = orderRef;
	  	    this.MrnId = mrnId;
            this.MrnAssigningAuthority = mrnAssigningAuthority;
            this.PatientName = patientName;
            this.VisitId = visitId;
            this.VisitAssigningAuthority = visitAssigningAuthority;
            this.AccessionNumber = accessionNumber;
            this.DiagnosticServiceName = diagnosticServiceName;
            this.RequestedProcedureName = requestedProcedureName;
            this.ModalityProcedureStepName = modalityProcedureStepName;
            this.ModalityName = modalityName;
            this.OrderPriority = orderPriority;
        }

        public OrderSummary()
        {
            this.PatientName = new PersonNameDetail();
            this.OrderPriority = new EnumValueInfo();
        }

        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
	  	public string MrnId;

        [DataMember]
        public string MrnAssigningAuthority;

        [DataMember]
        public PersonNameDetail PatientName;

        [DataMember]
        public string VisitId;

        [DataMember]
        public string VisitAssigningAuthority;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public string DiagnosticServiceName;

        [DataMember]
        public string RequestedProcedureName;

        [DataMember]
        public string ModalityProcedureStepName;

        [DataMember]
        public string ModalityName;

        [DataMember]
        public EnumValueInfo OrderPriority;
    }
}
