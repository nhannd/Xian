using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Models how images are logically and physically organized.
	/// </summary>
	/// <remarks>
	/// A <b>Workspace</b> is comprised of a <see cref="LogicalWorkspace"/> and a
	/// <see cref="PhysicalWorkspace"/> and thus models how images are logically
	/// and physically organized.  Each <b>Workspace</b> also has its own set of
	/// workspace tools and <see cref="CommandHistory"/>.  This allows workspaces to
	/// function independently of each other.
	/// </remarks>
	public abstract class Workspace : IWorkspace
	{
		private string _title;
		private bool _isActivated;
		private CommandHistory _commandHistory;

		private event EventHandler<ActivationChangedEventArgs> _activationChangedEvent;
        private event EventHandler _titleChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="Workspace"/> class.
		/// </summary>
		public Workspace(string title)
		{
            _title = title;
			CreateCommandHistory();
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

		public virtual bool IsActivated
		{
			get { return _isActivated; }
			set
			{
				if (_isActivated != value)
				{
					_isActivated = value;
                    this.ToolSet.Activate(_isActivated);
					EventsHelper.Fire(_activationChangedEvent, this, new ActivationChangedEventArgs(value));
				}
			}
		}

		/// <summary>
		/// Gets the workspace's <see cref="CommandHistory"/>.
		/// </summary>
		/// <value>The workspace's <see cref="CommandHistory"/>.</value>
		public CommandHistory CommandHistory
		{
			get { return _commandHistory; }
		}

        public abstract IToolSet ToolSet
        {
            get;
        }

		public abstract IWorkspaceView View
		{
			get;
		}

		public event EventHandler<ActivationChangedEventArgs> ActivationChangedEvent
		{
			add { _activationChangedEvent += value; }
			remove { _activationChangedEvent -= value; }
		}

        /// <summary>
        /// Fired when the title of the workspace is changed
        /// </summary>
        public event EventHandler TitleChanged
        {
            add { _titleChanged += value; }
            remove { _titleChanged -= value; }
        }

		public abstract void Cleanup();

		private void CreateCommandHistory()
		{
			int maxHistorySize = 100;
			_commandHistory = new CommandHistory(maxHistorySize);
		}
	}
}
