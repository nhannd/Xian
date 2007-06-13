using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ReportingWorklistPreview : DataContractBase
    {
        [DataMember]
        public string ReportContent;

        [DataMember]
        public MrnDetail Mrn;

        [DataMember]
        public PersonNameDetail Name;

        [DataMember]
        public DateTime? DateOfBirth;

        [DataMember]
        public string Sex;
        
        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public string RequestedProcedureName;

        [DataMember]
        public string VisitNumberId;

        [DataMember]
        public string VisitNumberAssigningAuthority;
    }
}

