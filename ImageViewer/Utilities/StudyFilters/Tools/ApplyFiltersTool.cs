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