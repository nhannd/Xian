using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Abstract base class providing most of the implementation of <see cref="IWorkspace"/>.  Concrete workspace
    /// classes are encourage to extend this class rather than implement <see cref="IWorkspace"/> directly.
    /// </summary>
	public abstract class Workspace : IWorkspace
	{
        private IDesktopWindow _desktopWindow;
		private string _title;
		private CommandHistory _commandHistory;

        private event EventHandler _titleChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="Workspace"/> class.
		/// </summary>
		public Workspace(string title)
		{
            _title = title;
            _commandHistory = new CommandHistory(100);
        }

        ~Workspace()
        {
            Dispose(false);
        }

        #region IWorkspace Members


        public virtual void Initialize(IDesktopWindow desktopWindow)
        {
            _desktopWindow = desktopWindow;
        }


        public IDesktopWindow DesktopWindow
        {
            get { return _desktopWindow; }
        }

        /// <summary>
        /// Gets or sets the title of this workspace.  The title will be displayed in the user-interface
        /// </summary>
		public string Title
		{
            get { return _title; }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    EventsHelper.Fire(_titleChanged, this, new EventArgs());
                }
            }
		}

        public virtual void Activate()
        {
            if (_desktopWindow == null)
                throw new InvalidOperationException();

            _desktopWindow.WorkspaceManager.ActiveWorkspace = this;
        }

		public bool Active
		{
            get { return (_desktopWindow == null) ? false : (_desktopWindow.ActiveWorkspace == this); }
		}

		/// <summary>
		/// Gets the workspace's <see cref="CommandHistory"/>.
		/// </summary>
		/// <value>The workspace's <see cref="CommandHistory"/>.</value>
		public CommandHistory CommandHistory
		{
			get { return _commandHistory; }
		}

        public abstract IActionSet Actions
        {
            get;
        }

        /// <summary>
        /// Fired when the title of the workspace is changed
        /// </summary>
        public event EventHandler TitleChanged
        {
            add { _titleChanged += value; }
            remove { _titleChanged -= value; }
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public virtual bool CanClose()
        {
            return true;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
