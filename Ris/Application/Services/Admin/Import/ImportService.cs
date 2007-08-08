using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
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
                (new DataImporterExtensionPoint()).ListExtensions(),
                delegate(ExtensionInfo info)
                {
                    return info.Name ?? info.FormalName;
                });

            return new ListImportersResponse(importers);
        }

        [UpdateOperation]
        public ImportCsvResponse ImportCsv(ImportCsvRequest request)
        {
            IDataImporter importer = null;
            try
            {
                importer = (IDataImporter)
                    (new DataImporterExtensionPoint()).CreateExtension(
                                            delegate(ExtensionInfo info)
                                            {
                                                return info.Name == request.Importer || info.FormalName == request.Importer;
                                            });
            }
            catch (NotSupportedException)
            {
                throw new RequestValidationException(string.Format("{0} is not supported.", request.Importer));
            }

            importer.ImportCsv(request.Rows, (IUpdateContext)PersistenceContext);

            return new ImportCsvResponse();
        }

        #endregion
    }
}
