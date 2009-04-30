#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
//using ClearCanvas.ImageViewer.Tools;
using ClearCanvas.Desktop.Tools;
using Gtk;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
	public class ToolViewManager
	{
		//private ToolManager _toolManager;
		private ToolSet _toolManager;
		private bool _active;
		private Dictionary<ToolViewProxy, ToolViewHostDialog> _viewHosts;
		private Window _parentWindow;
		
		//public ToolViewManager(ToolManager toolManager, Window parentWindow)
		public ToolViewManager(ToolSet toolManager, Window parentWindow)
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
