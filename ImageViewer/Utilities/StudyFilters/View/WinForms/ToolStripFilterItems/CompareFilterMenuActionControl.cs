using System;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems
{
	public partial class CompareFilterMenuActionControl : UserControl
	{
		private CompareFilterMenuAction _action;

		public CompareFilterMenuActionControl(CompareFilterMenuAction action)
		{
			InitializeComponent();

			_action = action;
			_action.Refresh();
			_action.CurrentModeChanged += Action_CurrentModeChanged;

			_txtValue.DataBindings.Add("Text", _action, "Value", false, DataSourceUpdateMode.OnPropertyChanged);

			UpdateMode();
		}

		private void PerformDispose(bool disposing)
		{
			if (disposing)
			{
				_action.CurrentModeChanged -= Action_CurrentModeChanged;
			}
		}

		private void UpdateMode()
		{
			switch (_action.CurrentMode)
			{
				case CompareFilterMode.LessThan:
					_modeToggle.Text = "<";
					break;
				case CompareFilterMode.LessThenOrEquals:
					_modeToggle.Text = "\u2264";
					break;
				case CompareFilterMode.GreaterThan:
					_modeToggle.Text = ">";
					break;
				case CompareFilterMode.GreaterThanOrEquals:
					_modeToggle.Text = "\u2265";
					break;
				case CompareFilterMode.NotEquals:
					_modeToggle.Text = "\u2260";
					break;
				case CompareFilterMode.Equals:
				default:
					_modeToggle.Text = "=";
					break;
			}
		}

		private void Action_CurrentModeChanged(object sender, EventArgs e)
		{
			UpdateMode();
		}

		private void modeToggle_Click(object sender, EventArgs e)
		{
			_action.ToggleMode();
		}

		private void _txtValue_TextChanged(object sender, EventArgs e) {}
	}
}