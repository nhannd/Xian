using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	//[ButtonAction("show", DefaultToolbarActionSite + "/ToolbarEditFilters", "Show")]
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
			
		}
	}
}