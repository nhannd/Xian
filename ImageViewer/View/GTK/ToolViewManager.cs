using System;
using System.Collections.Generic;
using ClearCanvas.Workstation.Model.Tools;
using Gtk;

namespace ClearCanvas.Workstation.View.GTK
{
	public class ToolViewManager
	{
		private ToolManager _toolManager;
		private bool _active;
		private Dictionary<ToolViewProxy, ToolViewHostDialog> _viewHosts;
		private Window _parentWindow;
		
		public ToolViewManager(ToolManager toolManager, Window parentWindow)
		{
			_toolManager = toolManager;
			_active = false;
			_viewHosts = new Dictionary<ToolViewProxy, ToolViewHostDialog>();
			_parentWindow = parentWindow;
			
			foreach(ToolViewProxy view in _toolManager.ToolViews)
			{
				view.ActivationChanged += OnToolViewActivationChanged;
			}
			
		}
		
		public void Activate(bool active)
		{
			if(active == _active)
				return;
			
			_active = active;
			if(_active)
			{
				foreach(ToolViewProxy view in _toolManager.ToolViews)
				{
					UpdateViewHost(view);
				}
			}
			else
			{
				// hide all views
				foreach(ToolViewHostDialog host in _viewHosts.Values)
				{
					host.Hide();
				}
			}
		}
		
		private void OnToolViewActivationChanged(object sender, EventArgs e)
		{
			if(_active)
			{
				UpdateViewHost((ToolViewProxy)sender);
			}
		}
		
		private void UpdateViewHost(ToolViewProxy view)
		{
			if(view.Active)
			{
				ToolViewHostDialog host;
				if(_viewHosts.ContainsKey(view))
				{
					host = _viewHosts[view];
				}
				else
				{
					host = new ToolViewHostDialog(view, _parentWindow);
					_viewHosts.Add(view, host);
				}
				host.ShowAll();
			}
			else
			{
				if(_viewHosts.ContainsKey(view))
				{
					_viewHosts[view].Hide();
				}
			}
		}
	}
}
