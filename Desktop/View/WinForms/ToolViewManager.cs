using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;

using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Docking;
using Crownwood.DotNetMagic.Controls;

namespace ClearCanvas.Desktop.View.WinForms
{
	class ToolViewManager
	{
		private ToolManager _toolManager;
		// We're using the ToolViewProxy's Type as the key as opposed the ToolViewProxy
		// itself since we want to search on the Type, not the instance.
		private Dictionary<Type, Content> _dockingWindows = new Dictionary<Type, Content>();
		private DockingManager _dockingManager;

		public ToolViewManager(DockingManager dockingManager)
		{
			Platform.CheckForNullReference(dockingManager, "dockingManager");

			_dockingManager = dockingManager;
			_dockingManager.ContentHidden += new DockingManager.ContentHandler(OnContentHidden);
		}

		public ToolManager ToolManager
		{
			get { return _toolManager; }
			set
			{
				// Disconnect handler from old tool views
				if (_toolManager != null)
				{
					foreach (ToolViewProxy toolViewProxy in _toolManager.ToolViews)
						toolViewProxy.ActivationChanged -= new EventHandler<ActivationChangedEventArgs>(OnToolViewActivationChanged);
				}

				_toolManager = value;

				// If null, then assume we want to destroy all docking windows
				// associated with tool views.
				if (_toolManager == null)
				{
					foreach (KeyValuePair<Type, Content> pair in _dockingWindows)
						_dockingManager.Contents.Remove(pair.Value);

					_dockingWindows.Clear();
					return;
				}

				foreach (ToolViewProxy toolViewProxy in _toolManager.ToolViews)
				{
					// Connect handler to new tool views
					toolViewProxy.ActivationChanged += new EventHandler<ActivationChangedEventArgs>(OnToolViewActivationChanged);

					// Connect existing docking windows to new tool view controls
					if (_dockingWindows.ContainsKey(toolViewProxy.View.GetType()))
						_dockingWindows[toolViewProxy.View.GetType()].Control = toolViewProxy.View.GuiElement as Control;

					if (toolViewProxy.Active)
						ShowDockingWindow(toolViewProxy);
				}
			}
		}

		private void OnToolViewActivationChanged(object sender, ActivationChangedEventArgs e)
		{
			ToolViewProxy toolViewProxy = sender as ToolViewProxy;

			if (toolViewProxy.Active)
				ShowDockingWindow(toolViewProxy);
			else
				HideDockingWindow(toolViewProxy);
		}

		private void ShowDockingWindow(ToolViewProxy toolViewProxy)
		{
			// If we can't find the toolViewProxy, that means we haven't created it
			// yet, so let's create it now.
			if (!_dockingWindows.ContainsKey(toolViewProxy.View.GetType()))
			{
				Control control = toolViewProxy.View.GuiElement as Control;
				Platform.CheckForInvalidCast(control, "toolViewProxy.View.GuiElement", "Control");
				string title = toolViewProxy.Title;
				ToolViewDisplayHint hint = toolViewProxy.DisplayHint;

				// Make sure the window is the size as it's been defined by the tool
				Content content = _dockingManager.Contents.Add(control, title);

				if ((hint & ToolViewDisplayHint.MaximizeOnDock) != 0)
					content.DisplaySize = _dockingManager.Container.Size;
				else
					content.DisplaySize = control.Size;

				content.AutoHideSize = control.Size;
				content.FloatingSize = control.Size;
				content.CloseOnHide = false;
				content.Tag = toolViewProxy;

				if ((hint & ToolViewDisplayHint.DockAutoHide) != 0)
					_dockingManager.Container.SuspendLayout();

				// Dock the window on the correct edge
				if ((hint & ToolViewDisplayHint.DockTop) != 0)
					_dockingManager.AddContentWithState(content, State.DockTop);
				else if ((hint & ToolViewDisplayHint.DockBottom) != 0)
					_dockingManager.AddContentWithState(content, State.DockBottom);
				else if ((hint & ToolViewDisplayHint.DockLeft) != 0)
					_dockingManager.AddContentWithState(content, State.DockLeft);
				else if ((hint & ToolViewDisplayHint.DockRight) != 0)
					_dockingManager.AddContentWithState(content, State.DockRight);
				else
					_dockingManager.AddContentWithState(content, State.Floating);

				if ((hint & ToolViewDisplayHint.DockAutoHide) != 0)
				{
					_dockingManager.ToggleContentAutoHide(content);
					_dockingManager.Container.ResumeLayout();
					_dockingManager.BringAutoHideIntoView(content);
				}

				// Keep trace of the dock content we just created
				_dockingWindows.Add(toolViewProxy.View.GetType(), content);
			}
			else
			{
				// Find the existing dock content and show it
				Content content = _dockingWindows[toolViewProxy.View.GetType()];

				if (!content.Visible)
					_dockingManager.ShowContent(content);
			}
		}

		private void HideDockingWindow(ToolViewProxy toolViewProxy)
		{
			if (_dockingWindows.ContainsKey(toolViewProxy.View.GetType()))
			{
				Content content = _dockingWindows[toolViewProxy.View.GetType()];

				if (content.Visible)
					_dockingManager.HideContent(content, true, true);
			}
		}

		private void OnContentHidden(Content c, EventArgs cea)
		{
			// Tell the tool this docking window is being closed
			// by the user so we stay synchronized.
			ToolViewProxy toolViewProxy = c.Tag as ToolViewProxy;
			toolViewProxy.Active = false;
		}
	}
}
