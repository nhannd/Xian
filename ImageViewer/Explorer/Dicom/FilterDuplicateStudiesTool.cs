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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/FilterDuplicateStudies", "Toggle", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/FilterDuplicateStudies", "Toggle", Flags = ClickActionFlags.CheckAction)]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[VisibleStateObserver("activate", "Visible", "VisibleChanged")]
	[CheckedStateObserver("activate", "Checked", "CheckedChanged")]
	[TooltipValueObserver("activate", "Tooltip", "CheckedChanged")]
	[LabelValueObserver("activate", "Label", "CheckedChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.ShowAllStudiesToolSmall.png", "Icons.ShowAllStudiesToolSmall.png", "Icons.ShowAllStudiesToolSmall.png")]
	internal class FilterDuplicateStudiesTool : StudyBrowserTool
	{
		private readonly StudyBrowserComponent _parent;
		private bool _checked;
		private event EventHandler _checkedChanged;

		private bool _visible;
		private event EventHandler _visibleChanged;

		public FilterDuplicateStudiesTool(StudyBrowserComponent parent)
		{
			_parent = parent;
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			UpdateState();
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			UpdateState();
		}

		private void UpdateState()
		{
			if (!_parent.SelectedServerGroup.IsLocalDatastore && _parent.SelectedServerGroup.Servers.Count > 1)
			{
				if (_parent.CurrentSearchResult != null)
					this.Enabled = _parent.CurrentSearchResult.HasDuplicates;
			
				this.Visible = true;
			}
			else
			{
				this.Visible = false;
				this.Enabled = false;
			}

			this.Checked = _parent.FilterDuplicateStudies;
		}

		public bool Visible
		{
			get { return _visible; }
			set
			{
				if (_visible != value)
				{
					_visible = value;
					EventsHelper.Fire(_visibleChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler VisibleChanged
		{
			add { this._visibleChanged += value; }
			remove { this._visibleChanged -= value; }
		}

		public bool Checked
		{
			get { return _checked; }
			set
			{
				if (_checked != value)
				{
					_checked = value;
					EventsHelper.Fire(_checkedChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler CheckedChanged
		{
			add { this._checkedChanged += value; }
			remove { this._checkedChanged -= value; }
		}

		public string Label
		{
			get { return Tooltip; }
		}
		public string Tooltip
		{
			get
			{
				if (Checked)
					return SR.TooltipShowAllStudies;
				else
					return SR.TooltipHideDuplicateStudies;
			}
		}

		public void Toggle()
		{
			if (Visible && Enabled)
			{
				this._parent.FilterDuplicateStudies = !this._parent.FilterDuplicateStudies;
				UpdateState();
			}
		}
	}
}