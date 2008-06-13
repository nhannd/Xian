using System.IO;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// Defines the interface of study editting action commands.
    /// </summary>
    public interface IEditStudyActionCommand
    {
        /// <summary>
        /// Gets or sets the context of the command.
        /// </summary>
        EditStudyContext Context
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Base class for study editting action commands
    /// </summary>
    public abstract class EditStudyActionCommandBase : ServerCommand, IEditStudyActionCommand
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
