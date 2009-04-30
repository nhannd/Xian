#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.Import;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services.Admin.Import
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IImportService))]
    public class ImportService : ApplicationServiceBase, IImportService
    {
        #region IImportService Members

        public ListImportersResponse ListImporters(ListImportersRequest request)
        {
            List<string> importers = CollectionUtils.Map<ExtensionInfo, string, List<string>>(
                (new CsvDataImporterExtensionPoint()).ListExtensions(),
                delegate(ExtensionInfo info)
                {
                    return info.Name ?? info.FormalName;
                });

            return new ListImportersResponse(importers);
        }

        [UpdateOperation]
        public ImportCsvResponse ImportCsv(ImportCsvRequest request)
        {
            ICsvDataImporter importer = null;
            try
            {
                importer = (ICsvDataImporter)
                    (new CsvDataImporterExtensionPoint()).CreateExtension(
                                            delegate(ExtensionInfo info)
                                            {
                                                return info.Name == request.Importer || info.FormalName == request.Importer;
                                            });
            }
            catch (NotSupportedException)
            {
                throw new RequestValidationException(string.Format("{0} is not supported.", request.Importer));
            }

            importer.Import(request.Rows, (IUpdateContext)PersistenceContext);

            return new ImportCsvResponse();
        }

        #endregion
    }
}
