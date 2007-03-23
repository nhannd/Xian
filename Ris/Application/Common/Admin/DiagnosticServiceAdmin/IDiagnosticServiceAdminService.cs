using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin
{
   /// <summary>
    /// Importing a batch of services for the <see cref="ImportDiagnosticServicesComponent"/>
    /// </summary>
     [ServiceContract]
    public interface IDiagnosticServiceAdminService
    {
        /// <summary>
        /// Imports a batch of diagnostic services in CSV format.
        /// </summary>
        /// <param name="request">
        /// Each string[] in the list must contain 8 elements, as follows:
        ///     0 - Diagnostic Service ID
        ///     1 - Diagnostic Service Name
        ///     2 - Requested Procedure Type ID
        ///     3 - Requested Procedure Type Name
        ///     4 - Modality Procedure Step Type ID
        ///     5 - Modality Procedure Step Type Name
        ///     6 - Default Modality ID
        ///     7 - Default Modality Name
        /// </param>
        [OperationContract]
        BatchImportResponse BatchImport(BatchImportRequest request);
    }
}
