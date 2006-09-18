using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Abstract class that provides the base implementation of <see cref="IWorkspace"/>.
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

        /// <summary>
        /// Implementation of the <see cref="IDisposable"/> pattern
        /// </summary>
        /// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
        protected virtual void Dispose(bool disposing)
        {
            // nothing to do
        }

        public virtual bool CanClose()
        {
            return true;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                // shouldn't throw anything from inside Dispose()
                Platform.Log(e);
            }
        }

        #endregion
    }
}
