namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// List of recovery mechanisms used by different work queue processors when the entry
    /// is failed because of mismatch number of instances in the study xml and the database.
    /// </summary>
    public enum RecoveryModes
    {
        /// <summary>
        /// Users will handle it manually.
        /// </summary>
        Manual,

        /// <summary>
        /// The server will trigger a reprocess of the study.
        /// </summary>
        Automatic
    }
}