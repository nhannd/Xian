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
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.AutoFilters.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.Properties;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems
{
	public partial class ListFilterControl : UserControl, IClickableHostedControl
	{
		public event EventHandler ResetDropDownFocusRequested;
		public event EventHandler CloseDropDownRequested;

		private readonly Size _defaultSize;
		private readonly ListFilterMenuAction _action;
		private bool _ignoreItemCheck = false;

		public ListFilterControl(ListFilterMenuAction action)
		{
			InitializeComponent();
			_defaultSize = base.Size;

			_action = action;

			ResetListBox();
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			Size size = base.GetPreferredSize(proposedSize);
			return new Size(Math.Max(_defaultSize.Width, size.Width), Math.Max(_defaultSize.Height, size.Height));
		}

		private void ResetListBox()
		{
			_ignoreItemCheck = true;
			try
			{
				_listBox.Items.Add(new SelectAllValues());
				foreach (object value in _action.DataSource.Values)
				{
					_listBox.Items.Add(new ValueWrapper(value), _action.DataSource.GetSelectedState(value));
				}
				_listBox.SetItemCheckState(0, GetCombinedCheckState());
			}
			finally
			{
				_ignoreItemCheck = false;
			}
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

		private void CloseDropDown()
		{
			EventsHelper.Fire(this.CloseDropDownRequested, this, EventArgs.Empty);
		}

		private void _listBox_MouseLeave(object sender, EventArgs e)
		{
			EventsHelper.Fire(this.ResetDropDownFocusRequested, this, EventArgs.Empty);
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
					}
					else if (e.NewValue == CheckState.Unchecked)
					{
						for (int n = 1; n < _listBox.Items.Count; n++)
							_listBox.SetItemChecked(n, false);
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
				}
			}
			finally
			{
				_ignoreItemCheck = false;
			}
		}

		private void _btnOk_Click(object sender, EventArgs e)
		{
			CheckState combined = GetCombinedCheckState();
			switch (combined)
			{
				case CheckState.Checked:
					_action.DataSource.SetAllSelectedState(true);
					break;
				case CheckState.Unchecked:
					_action.DataSource.SetAllSelectedState(false);
					break;
				default:
					for (int n = 1; n < _listBox.Items.Count; n++)
						_action.DataSource.SetSelectedState(((ValueWrapper) _listBox.Items[n]).Value, _listBox.GetItemChecked(n));
					break;
			}
			CloseDropDown();
		}

		private void _btnCancel_Click(object sender, EventArgs e)
		{
			CloseDropDown();
			ResetListBox();
		}

		private class ValueWrapper
		{
			public readonly object Value;

			public ValueWrapper(object value)
			{
				this.Value = value;
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

		private sealed class SelectAllValues : ValueWrapper
		{
			public SelectAllValues() : base(null) {}

			public override string ToString()
			{
				return Resources.LabelSelectAll;
			}
		}
	}
}