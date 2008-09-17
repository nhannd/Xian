using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudy
{
    /// <summary>
    /// Defines the common interface of commands used by 'ReconcileStudy' processor
    /// </summary>
    public interface IReconcileServerCommand : IServerCommand
    {
        /// <summary>
        /// Gets or sets the context of the reconciliation.
        /// </summary>
        ReconcileStudyProcessorContext Context { get; set; }
    }
}