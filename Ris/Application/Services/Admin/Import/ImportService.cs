#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
