using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ListProcedureExtendedPropertiesResponse : DataContractBase
    {
        public ListProcedureExtendedPropertiesResponse()
        {
            ProcedureExtendedProperties = new List<Dictionary<string, string>>();
        }

        [DataMember]
        public List<Dictionary<string, string>> ProcedureExtendedProperties;
    }
}