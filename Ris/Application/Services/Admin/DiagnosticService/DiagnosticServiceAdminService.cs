using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Services.Admin.DiagnosticService
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class DiagnosticServiceAdminService : ApplicationServiceBase, IDiagnosticServiceAdminService
    {
        [UpdateOperation]
        public void BatchImport(IList<string[]> data)
        {
            DiagnosticServiceBatchImporter.Import((IUpdateContext)this.CurrentContext, data);
        }
    }
}
