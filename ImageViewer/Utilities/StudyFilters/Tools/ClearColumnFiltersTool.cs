using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	[MenuAction("clear", "studyfilters-columnfilters/MenuClearFilters", "Clear")]
	[EnabledStateObserver("clear", "Enabled", "EnabledChanged")]
	[IconSet("clear", IconScheme.Colour, "Icons.ClearFiltersSmall.png", "Icons.ClearFiltersSmall.png", "Icons.ClearFiltersSmall.png")]
	[ExtensionOf(typeof (StudyFilterColumnToolExtensionPoint))]
	public class ClearColumnFiltersTool : StudyFilterColumnTool
	{
		public event EventHandler EnabledChanged;

		private bool _enabled;

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(this.EnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public void Clear()
		{
			base.Column.ColumnFilterRoot.Predicates.Clear();
			base.StudyFilter.Refresh();
		}

		private void StudyFilter_FilterPredicatesChanged(object sender, EventArgs e)
		{
			this.Enabled = base.Column.IsColumnFiltered;
		}

		public override void Initialize()
		{
			base.Initialize();
			base.StudyFilter.FilterPredicatesChanged += StudyFilter_FilterPredicatesChanged;
		}

		protected override void Dispose(bool disposing)
		{
			base.StudyFilter.FilterPredicatesChanged -= StudyFilter_FilterPredicatesChanged;
			base.Dispose(disposing);
		}
	}
}