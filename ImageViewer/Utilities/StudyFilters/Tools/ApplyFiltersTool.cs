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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.FilterNodes;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	[ButtonAction("show", DefaultToolbarActionSite + "/ToolbarEditFilters", "Show")]
	[ButtonAction("toggle", DefaultToolbarActionSite + "/ToolbarFilter", "ToggleFilter")]
	[CheckedStateObserver("toggle", "Checked", "CheckedChanged")]
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class ApplyFiltersTool : StudyFilterTool
	{
		public event EventHandler CheckedChanged;

		public bool Checked
		{
			get { return base.Component.Filtered; }
			set { base.Component.Filtered = value; }
		}

		public override void Initialize()
		{
			base.Initialize();
			base.Component.FilteredChanged += Component_FilteredChanged;
		}

		protected override void Dispose(bool disposing)
		{
			base.Component.FilteredChanged -= Component_FilteredChanged;
			base.Dispose(disposing);
		}

		public void ToggleFilter()
		{
			this.Checked = !this.Checked;
		}

		private void Component_FilteredChanged(object sender, EventArgs e)
		{
			EventsHelper.Fire(this.CheckedChanged, this, new EventArgs());
		}

		public void Show()
		{
			FilterEditorComponent component = new FilterEditorComponent(base.Columns, base.Component.FilterPredicates);
			SimpleComponentContainer container = new SimpleComponentContainer(component);
			DialogBoxAction action = base.DesktopWindow.ShowDialogBox(container, SR.EditFilters);
			if (action == DialogBoxAction.Ok)
			{
				base.Component.FilterPredicates.Clear();
				foreach (FilterNodeBase filter in component.Filters)
				{
					base.Component.FilterPredicates.Add(filter);
				}
				base.Component.Refresh();
			}
		}
	}
}