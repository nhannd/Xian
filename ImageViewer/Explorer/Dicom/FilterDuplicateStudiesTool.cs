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