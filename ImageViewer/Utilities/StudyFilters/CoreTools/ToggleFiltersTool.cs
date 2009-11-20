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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.CoreTools
{
	[ButtonAction("toggle", DefaultToolbarActionSite + "/ToolbarFiltersAreOn", "Toggle")]
	[IconSet("toggle", IconScheme.Colour, "Icons.ToggleFiltersToolSmall.png", "Icons.ToggleFiltersToolMedium.png", "Icons.ToggleFiltersToolLarge.png")]
	[EnabledStateObserver("toggle", "Enabled", "EnabledChanged")]
	[CheckedStateObserver("toggle", "Checked", "CheckedChanged")]
	[LabelValueObserver("toggle", "Label", "CheckedChanged")]
	[TooltipValueObserver("toggle", "Tooltip", "CheckedChanged")]
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class ToggleFiltersTool : StudyFilterTool
	{
		public event EventHandler EnabledChanged;
		public event EventHandler CheckedChanged;

		private bool _enabled;

		public bool Enabled
		{
			get { return _enabled; }
			private set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(this.EnabledChanged, this, EventArgs.Empty);

					if (_enabled) // if enabled changes from off to on, also toggle filters on
					{
						base.StudyFilter.FilterPredicatesEnabled = true;
					}
				}
			}
		}

		public bool Checked
		{
			get { return base.StudyFilter.FilterPredicatesEnabled; }
		}

		public string Label
		{
			get { return base.StudyFilter.FilterPredicatesEnabled ? SR.ToolbarFiltersAreOn : SR.ToolbarFiltersAreOff; }
		}

		public string Tooltip
		{
			get { return base.StudyFilter.FilterPredicatesEnabled ? SR.TooltipFiltersAreOn : SR.TooltipFiltersAreOff; }
		}

		public void Toggle()
		{
			base.StudyFilter.FilterPredicatesEnabled = !base.StudyFilter.FilterPredicatesEnabled;
			base.RefreshTable();
		}

		private bool AtLeastOneFilter
		{
			get
			{
				foreach (StudyFilterColumn column in (IEnumerable<StudyFilterColumn>) base.Columns)
				{
					if (column.IsColumnFiltered)
						return true;
				}
				return false;
			}
		}

		private void StudyFilter_FilterPredicatesEnabledChanged(object sender, EventArgs e)
		{
			EventsHelper.Fire(this.CheckedChanged, this, EventArgs.Empty);
		}

		private void StudyFilter_FilterPredicatesChanged(object sender, EventArgs e)
		{
			this.Enabled = this.AtLeastOneFilter;
		}

		public override void Initialize()
		{
			base.Initialize();
			base.StudyFilter.FilterPredicatesChanged += StudyFilter_FilterPredicatesChanged;
			base.StudyFilter.FilterPredicatesEnabledChanged += StudyFilter_FilterPredicatesEnabledChanged;
		}

		protected override void Dispose(bool disposing)
		{
			base.StudyFilter.FilterPredicatesEnabledChanged -= StudyFilter_FilterPredicatesEnabledChanged;
			base.StudyFilter.FilterPredicatesChanged -= StudyFilter_FilterPredicatesChanged;
			base.Dispose(disposing);
		}
	}
}