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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Explorer.Local;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	public abstract class LocalExplorerStudyFilterToolProxy<T> : StudyFilterTool
		where T : ToolBase, new()
	{
		private T _baseTool;

		protected LocalExplorerStudyFilterToolProxy()
		{
			_baseTool = new T();
		}

		protected T BaseTool
		{
			get { return _baseTool; }
		}

		protected IActionSet BaseActions
		{
			get { return _baseTool.Actions; }
		}

		public override void Initialize()
		{
			base.Initialize();
			_baseTool.SetContext(new ToolContextProxy(this));
			_baseTool.Initialize();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _baseTool != null)
			{
				_baseTool.Dispose();
				_baseTool = null;
			}
			base.Dispose(disposing);
		}

		private class ToolContextProxy : ILocalImageExplorerToolContext
		{
			private readonly LocalExplorerStudyFilterToolProxy<T> _owner;
			private ClickHandlerDelegate _defaultActionHandler;

			public ToolContextProxy(LocalExplorerStudyFilterToolProxy<T> owner)
			{
				_owner = owner;
			}

			public event EventHandler SelectedPathsChanged
			{
				add { _owner.Context.SelectedItems.SelectionChanged += value; }
				remove { _owner.Context.SelectedItems.SelectionChanged -= value; }
			}

			public Selection<string> SelectedPaths
			{
				get
				{
					var selection = new List<string>();
					foreach (IStudyItem item in _owner.SelectedItems)
						selection.Add(item.Filename);
					return new Selection<string>(selection);
				}
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _owner.Context.DesktopWindow; }
			}

			public ClickHandlerDelegate DefaultActionHandler
			{
				get { return _defaultActionHandler; }
				set { _defaultActionHandler = value; }
			}
		}
	}
}