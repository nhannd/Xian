using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.DiagnosticServiceAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class DiagnosticServiceAdminService : ApplicationServiceBase, IDiagnosticServiceAdminService
    {
        [UpdateOperation]
        public BatchImportResponse BatchImport(BatchImportRequest request)
        {
            DiagnosticServiceBatchImporter.Import((IUpdateContext)this.PersistenceContext, request.ImportData);
            return new BatchImportResponse();
        }
    }
}
