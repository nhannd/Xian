using ClearCanvas.Common;
using ClearCanvas.Common.Audit;

namespace ClearCanvas.ImageServer.Enterprise.Audit
{
    /// <summary>
    /// An implementation of <see cref="IAuditSink"/> that sinks to the Application Log
    /// </summary>
    [ExtensionOf(typeof(AuditSinkExtensionPoint), Enabled = true)]
    public class ApplicationLogAuditSink : IAuditSink
    {
        #region IAuditSink Members

        /// <summary>
        /// Writes the specified entry to the sink.
        /// </summary>
        /// <param name="entry"></param>
        public void WriteEntry(AuditEntryInfo entry)
        {
            
        }

        #endregion
    }
}