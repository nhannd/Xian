using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin
{
    [DataContract]
    public class BatchImportRequest : DataContractBase
    {
        public BatchImportRequest(List<string[]> importData)
        {
            this.ImportData = importData;
        }

        [DataMember]
        public List<string[]> ImportData;
    }
}