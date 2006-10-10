using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Explorer.Local
{
	public interface ILocalImageExplorerToolContext : IToolContext
	{
		IEnumerable<string> SelectedPaths { get; }
		IDesktopWindow DesktopWindow { get; }
		ClickHandlerDelegate DefaultActionHandler { get; set; }
	}

	public abstract class LocalImageExplorerComponentBase : ApplicationComponent
	{
		protected class LocalImageExplorerToolContext : ToolContext, ILocalImageExplorerToolContext
		{
			private LocalImageExplorerComponentBase _component;

			public LocalImageExplorerToolContext(LocalImageExplorerComponentBase component)
			{
				_component = component;
			}

			#region LocalImageExplorerToolContext Members

			public IEnumerable<string> SelectedPaths
			{
				get
				{
					if (_component._getSelectedPathsDelegate == null)
						return new List<string>(); //an empty list

					return _component._getSelectedPathsDelegate();
				}
			}

			public IDesktopWindow DesktopWindow
			{
				get
				{
					return _component.Host.DesktopWindow;
				}
			}

			public ClickHandlerDelegate DefaultActionHandler
			{
				get { return _component._defaultActionHandler; }
				set { _component._defaultActionHandler = value; }
			}

			#endregion
		}

		/// LocalImageExplorerComponentBase members

		private ToolSet _toolSet;		
		private ClickHandlerDelegate _defaultActionHandler;
		private PropertyGetDelegate<IEnumerable<string>> _getSelectedPathsDelegate;

		public LocalImageExplorerComponentBase()
		{
		}

		protected ToolSet ToolSet
		{
			get { return _toolSet; }
			set { _toolSet = value; }
		}

		public ClickHandlerDelegate DefaultActionHandler
		{
			get { return _defaultActionHandler; }
			set { _defaultActionHandler = value; }
		}

		public PropertyGetDelegate<IEnumerable<string>> GetSelectedPathsDelegate
		{
			get { return _getSelectedPathsDelegate; }
			set { _getSelectedPathsDelegate = value; }
		}

		public void DefaultAction()
		{
			if (this.DefaultActionHandler != null)
				this.DefaultActionHandler();
		}

		public abstract ActionModelNode ContextMenuModel { get; }
		public abstract ActionModelNode ToolbarModel { get; }
	}
}
