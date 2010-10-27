#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
		event EventHandler SelectedPathsChanged;
		Selection<string> SelectedPaths { get; }
		IDesktopWindow DesktopWindow { get; }
		ClickHandlerDelegate DefaultActionHandler { get; set; }
	}

	[ExtensionPoint()]
	public sealed class LocalImageExplorerToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public sealed class LocalImageExplorerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
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

			public event EventHandler SelectedPathsChanged
			{
				add { _component.SelectionChanged += value; }
				remove { _component.SelectionChanged -= value; }
			}

			public Selection<string> SelectedPaths
			{
				get { return _component.Selection; }
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

		private event EventHandler _selectionChanged;
		private Selection<string> _selection;

		private ToolSet _toolSet;
		private ClickHandlerDelegate _defaultActionHandler;

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

		public Selection<string> Selection
		{
			get { return _selection ?? Selection<string>.Empty; }
			set
			{
				if (_selection != value)
				{
					_selection = value;
					OnSelectionChanged();
				}
			}
		}

		public event EventHandler SelectionChanged
		{
			add { _selectionChanged += value; }
			remove { _selectionChanged -= value; }
		}

		protected void OnSelectionChanged()
		{
			EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
		}

		public void DefaultAction()
		{
			if (this.DefaultActionHandler != null)
				this.DefaultActionHandler();
		}

		public IDesktopWindow DesktopWindow
		{
			get { return base.Host.DesktopWindow; }
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
