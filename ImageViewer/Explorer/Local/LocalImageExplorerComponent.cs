using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Explorer.Local
{
	[ExtensionPoint()]
	public class LocalImageExplorerToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint()]
	public class LocalImageExplorerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public interface ILocalImageViewerToolContext : IToolContext
	{
		IEnumerable<string> SelectedPaths { get; }
		IDesktopWindow DesktopWindow { get; }
		ClickHandlerDelegate DefaultActionHandler { get; set; }
	}

	[AssociateView(typeof(LocalImageExplorerComponentViewExtensionPoint))]
	public class LocalImageExplorerComponent : ApplicationComponent
	{
		internal class LocalImageExplorerToolContext : ToolContext, ILocalImageViewerToolContext
		{
			private LocalImageExplorerComponent _component;

			public LocalImageExplorerToolContext(LocalImageExplorerComponent component)
			{
				_component = component;
			}

			#region ILocalImageViewerToolContext Members

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

		public ActionModelNode ContextMenuModel
		{
			get
			{
				return ActionModelRoot.CreateModel(this.GetType().FullName, "explorerlocal-contextmenu", _toolSet.Actions);
			}
		}

		public PropertyGetDelegate<IEnumerable<string>> GetSelectedPathsDelegate
		{
			get { return _getSelectedPathsDelegate; }
			set { _getSelectedPathsDelegate = value; }
		}

		public ClickHandlerDelegate DefaultActionHandler
		{
			get { return _defaultActionHandler; }
		}

		public override void Start()
		{
			base.Start();
			_toolSet = new ToolSet(new LocalImageExplorerToolExtensionPoint(), new LocalImageExplorerToolContext(this));
		}

		public override void Stop()
		{
			base.Stop();
			_toolSet = null;
		}
	}
}
