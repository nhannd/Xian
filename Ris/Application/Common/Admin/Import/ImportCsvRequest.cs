#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Serialization;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.Import
{
    [DataContract]
    public class ImportCsvRequest : DataContractBase
    {
        public ImportCsvRequest()
        {

        }

        public ImportCsvRequest(string importer, List<string> rows)
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
