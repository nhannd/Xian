using System.IO;
using ClearCanvas.ImageServer.Common.CommandProcessor;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// Base command class for actions in a study edit operation.
    /// </summary>
    public abstract class EditStudyActionCommandBase : ServerCommand
    {
        #region Private Memeber
        private EditStudyContext _context = null;
        #endregion

        #region constructors
        public EditStudyActionCommandBase(string description, EditStudyContext context)
            : base(description, true)
        {
            _context = context;
        }

        #endregion

        #region IEditStudyCommand Members

        public EditStudyContext Context
        {
            get
            {
                return _context;
            }
            set
            {
                _context = value;
            }
        }


        public string TempOutFolder
        {
            get
            {
                return Path.Combine(Context.TempOutRootFolder, "Out");
            }
        }

        protected string TempStudyFolder
        {
            get
            {
                return Path.Combine(TempOutFolder, Context.Study.StudyInstanceUid);
            }
        }

        protected string DesinationStudyFolder
        {
            get
            {
                return Context.StorageLocation.GetStudyPath();
            }
        }


        #endregion

        #region Overridden Protected Methods
        protected override void OnUndo()
        {
            // NO-OP
        }

        #endregion

    }


}
