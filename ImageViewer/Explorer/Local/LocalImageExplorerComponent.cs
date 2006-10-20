using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Explorer.Local
{
	public interface ILocalImageExplorerToolContext : IToolContext
	{
		IEnumerable<string> SelectedPaths { get; }
		IDesktopWindow DesktopWindow { get; }
		ClickHandlerDelegate DefaultActionHandler { get; set; }
	}

	[ExtensionPoint()]
	public class LocalImageExplorerToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public class LocalImageExplorerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(LocalImageExplorerComponentViewExtensionPoint))]
	public class LocalImageExplorerComponent : ApplicationComponent
	{
		protected class LocalImageExplorerToolContext : ToolContext, ILocalImageExplorerToolContext
		{
			private LocalImageExplorerComponent _component;

			public LocalImageExplorerToolContext(LocalImageExplorerComponent component)
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

		/// LocalImageExplorerComponent members

		private ToolSet _toolSet;
		private ClickHandlerDelegate _defaultActionHandler;
		private PropertyGetDelegate<IEnumerable<string>> _getSelectedPathsDelegate;

		public LocalImageExplorerComponent()
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
		
		public ActionModelNode ContextMenuModel
		{
			get
			{
				return ActionModelRoot.CreateModel(this.GetType().FullName, "explorerlocal-contextmenu", ToolSet.Actions);
			}
		}

		public override void Start()
		{
			base.Start();
			ToolSet = new ToolSet(new LocalImageExplorerToolExtensionPoint(), new LocalImageExplorerToolContext(this));
		}

		public override void Stop()
		{
			base.Stop();
			ToolSet.Dispose();
			ToolSet = null;
		}
	}
}
