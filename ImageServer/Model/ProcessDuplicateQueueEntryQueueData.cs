
namespace ClearCanvas.ImageServer.Model
{
    /// <summary>
    /// Represents the contents in the Data column of the <see cref="WorkQueue"/> entry.
    /// </summary>
    public class ProcessDuplicateQueueEntryQueueData
    {
        #region Private Members
        private ProcessDuplicateAction _action;
        private string _duplicateSopFolder;

        #endregion

        public ProcessDuplicateAction Action
        {
            get { return _action; }
            set { _action = value; }
        }

        public string DuplicateSopFolder
        {
            get { return _duplicateSopFolder; }
            set { _duplicateSopFolder = value; }
        }
    }
}