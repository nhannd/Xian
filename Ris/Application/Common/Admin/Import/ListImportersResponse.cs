using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.Import
{
    [DataContract]
    public class ListImportersResponse : DataContractBase
    {
        public ListImportersResponse()
        {

        }

        public ListImportersResponse(List<string> importers)
        {
            this.Importers = importers;
        }

        /// <summary>
        /// A list of supporter Importer extensions.
        /// </summary>
        [DataMember]
        public List<string> Importers;
    }
}
