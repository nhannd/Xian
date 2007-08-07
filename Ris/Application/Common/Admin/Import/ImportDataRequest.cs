using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.Import
{
    [DataContract]
    public class ImportDataRequest : DataContractBase
    {
        public ImportDataRequest()
        {

        }

        public ImportDataRequest(string importer, List<string> rows)
        {
            this.Importer = importer;
            this.Rows = rows;
        }

        /// <summary>
        /// Name of the importer extension that will import the data.
        /// </summary>
        [DataMember]
        public string Importer;

        /// <summary>
        /// Collection of rows of data to import (e.g. from a CSV file).
        /// </summary>
        [DataMember]
        public List<string> Rows;
    }
}
