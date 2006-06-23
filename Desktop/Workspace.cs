using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Application.Tools;
using ClearCanvas.Common.Application.Actions;

namespace ClearCanvas.Common.Application
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
	public abstract class Workspace
	{
		private string _title;
		private bool _isActivated;
		private CommandHistory _commandHistory;
        private ToolManager _toolManager;
        private ToolContext _toolContext;

		private event EventHandler<ActivationChangedEventArgs> _activationChangedEvent;

		/// <summary>
		/// Initializes a new instance of the <see cref="Workspace"/> class.
		/// </summary>
		public Workspace()
		{
			CreateCommandHistory();
			CreateWorkspaceTools();
		}

		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		public virtual bool IsActivated
		{
			get { return _isActivated; }
			set
			{
				if (_isActivated != value)
				{
					_isActivated = value;
                    _toolContext.Activate(_isActivated);
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

        public ToolManager ToolManager
        {
            get { return _toolManager; }
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

		public abstract void Cleanup();

		private void CreateCommandHistory()
		{
			int maxHistorySize = 100;
			_commandHistory = new CommandHistory(maxHistorySize);
		}

		private void CreateWorkspaceTools()
        {
            _toolContext = CreateToolContext();
            _toolManager = new ToolManager(_toolContext);
		}

        protected abstract ToolContext CreateToolContext();
	}
}
