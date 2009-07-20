using System;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.Properties;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems
{
	public partial class ListFilterControl : UserControl
	{
		private event EventHandler _resetDropDownFocus;
		private readonly IListFilterDataSource _dataSource;
		private bool _ignoreItemCheck = false;

		public ListFilterControl(IListFilterDataSource dataSource)
		{
			InitializeComponent();

			_dataSource = dataSource;

			_ignoreItemCheck = true;

			_listBox.Items.Add(new SelectAllValues());
			foreach (object value in dataSource.Values)
			{
				_listBox.Items.Add(new ValueWrapper(value), dataSource.GetSelectedState(value));
			}
			_listBox.SetItemCheckState(0, GetCombinedCheckState());
			_ignoreItemCheck = false;
		}

		public event EventHandler ResetDropDownFocus
		{
			add { _resetDropDownFocus += value; }
			remove { _resetDropDownFocus -= value; }
		}

		private CheckState GetCombinedCheckState()
		{
			int value = 0;
			for (int n = 1; n < _listBox.Items.Count; n++)
				value |= _listBox.GetItemChecked(n) ? 2 : 1;
			if (value == 2)
				return CheckState.Checked;
			else if (value == 1)
				return CheckState.Unchecked;
			else return CheckState.Indeterminate;
		}

		private void _listBox_MouseLeave(object sender, EventArgs e)
		{
			EventsHelper.Fire(_resetDropDownFocus, this, EventArgs.Empty);
		}

		private void _listBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (_ignoreItemCheck)
				return;

			_ignoreItemCheck = true;
			try
			{
				ValueWrapper wrapper = (ValueWrapper) _listBox.Items[e.Index];
				if (e.Index == 0)
				{
					if (e.NewValue == CheckState.Checked)
					{
						for (int n = 1; n < _listBox.Items.Count; n++)
							_listBox.SetItemChecked(n, true);
						_dataSource.SetAllSelectedState(true);
					}
					else if (e.NewValue == CheckState.Unchecked)
					{
						for (int n = 1; n < _listBox.Items.Count; n++)
							_listBox.SetItemChecked(n, false);
						_dataSource.SetAllSelectedState(false);
					}
				}
				else
				{
					bool diff = false;

					for (int n = 1; n < _listBox.Items.Count; n++)
					{
						if (n != e.Index && e.NewValue != _listBox.GetItemCheckState(n))
						{
							diff = true;
							break;
						}
					}

					if (diff)
						_listBox.SetItemCheckState(0, CheckState.Indeterminate);
					else
						_listBox.SetItemCheckState(0, e.NewValue);

					_dataSource.SetSelectedState(wrapper.Value, e.NewValue == CheckState.Checked);
				}
			}
			finally
			{
				_ignoreItemCheck = false;
			}
		}

		private class ValueWrapper
		{
			public readonly object Value;

			public ValueWrapper(object value)
			{
				this.Value = value;
			}

			public virtual bool IsMetaValue
			{
				get { return false; }
			}

			public virtual bool ValueEquals(object value)
			{
				return (value == null && this.Value == null) || (value != null && value.Equals(this.Value));
			}

			public override string ToString()
			{
				if (this.Value == null)
				{
					return Resources.LabelNullValue;
				}
				return this.Value.ToString();
			}
		}

		private class SelectAllValues : ValueWrapper
		{
			public SelectAllValues() : base(null) {}

			public override bool IsMetaValue
			{
				get { return true; }
			}

			public override bool ValueEquals(object value)
			{
				return false;
			}

			public override string ToString()
			{
				return Resources.LabelSelectAll;
			}
		}
	}
}