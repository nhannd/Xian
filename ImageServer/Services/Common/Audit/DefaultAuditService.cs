using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.Audit;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Services.Common.Audit
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class DefaultAuditService : IApplicationServiceLayer, IAuditService
    {
        #region IAuditService Members

        public WriteEntryResponse WriteEntry(WriteEntryRequest request)
        {
            StringBuilder sb = new StringBuilder();
            
            Platform.Log(LogLevel.Info, request.LogEntry.Details);

            return new WriteEntryResponse();
        }

        #endregion
    }
}
