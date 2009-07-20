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
using System.Globalization;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	public partial class ColumnPickerComponentPanel : UserControl
	{
		private readonly ColumnPickerComponent _component;

		public ColumnPickerComponentPanel(ColumnPickerComponent component)
		{
			InitializeComponent();

			_component = component;

			foreach (StudyFilterColumn.ColumnDefinition column in component.Columns)
			{
				_lstSelectedColumns.Items.Add(column);
			}

			foreach (StudyFilterColumn.ColumnDefinition column in StudyFilterColumn.SpecialColumnDefinitions)
			{
				_lstSpecialColumns.Items.Add(column);
			}

			foreach (StudyFilterColumn.ColumnDefinition column in StudyFilterColumn.DicomTagColumnDefinitions)
			{
				_lstDicomColumns.Items.Add(column);
			}
		}

		#region Special Columns

		private void OnAddSpecialColumnClick(object sender, EventArgs e)
		{
			if (_lstSpecialColumns.SelectedItems != null && _lstSpecialColumns.SelectedItems.Count > 0)
			{
				foreach (object selectedItem in _lstSpecialColumns.SelectedItems)
				{
					StudyFilterColumn.ColumnDefinition column = selectedItem as StudyFilterColumn.ColumnDefinition;
					if (column == null)
						continue;
					if (_lstSelectedColumns.Items.Contains(column))
						continue;

					_lstSelectedColumns.Items.Add(column);
					_component.Columns.Add(column);
				}
			}
		}

		private void _lstSpecialColumns_DoubleClick(object sender, EventArgs e)
		{
			_btnAddSpecialColumn.PerformClick();
		}

		private void _lstSpecialColumns_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				e.IsInputKey = true;
		}

		private void _lstSpecialColumns_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				_btnAddSpecialColumn.PerformClick();
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		#endregion

		#region Dicom Columns

		private void OnAddDicomColumnClick(object sender, EventArgs e)
		{
			if (_lstDicomColumns.SelectedItems != null && _lstDicomColumns.SelectedItems.Count > 0)
			{
				foreach (object selectedItem in _lstDicomColumns.SelectedItems)
				{
					StudyFilterColumn.ColumnDefinition column = selectedItem as StudyFilterColumn.ColumnDefinition;
					if (column == null)
						continue;
					if (_lstSelectedColumns.Items.Contains(column))
						continue;

					_lstSelectedColumns.Items.Add(column);
					_component.Columns.Add(column);
				}
			}
		}

		private void _lstDicomColumns_DoubleClick(object sender, EventArgs e)
		{
			_btnAddDicomColumn.PerformClick();
		}

		private void _txtFilterDicomColumns_TextChanged(object sender, EventArgs e)
		{
			int index = _lstDicomColumns.FindString(_txtFilterDicomColumns.Text);
			if (index >= 0)
			{
				_lstDicomColumns.ClearSelected();
				_lstDicomColumns.SelectedIndex = index;
			}
		}

		private void _txtFilterDicomColumns_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
				e.IsInputKey = true;
		}

		private void _txtFilterDicomColumns_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				_btnAddDicomColumn.PerformClick();
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			else if (e.KeyCode == Keys.Up)
			{
				int index = _lstDicomColumns.SelectedIndex;
				if (index >= 0)
				{
					_lstDicomColumns.ClearSelected();
					_lstDicomColumns.SelectedIndex = Math.Max(0, index - 1);
				}
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			else if (e.KeyCode == Keys.Down)
			{
				int index = _lstDicomColumns.SelectedIndex;
				if (index >= 0)
				{
					_lstDicomColumns.ClearSelected();
					_lstDicomColumns.SelectedIndex = Math.Min(index + 1, _lstDicomColumns.Items.Count - 1);
				}
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void _lstDicomColumns_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				_btnAddDicomColumn.PerformClick();
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void _lstDicomColumns_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				e.IsInputKey = true;
		}

		#endregion

		#region Custom Dicom Columns

		private void OnAddDicomTagClick(object sender, EventArgs e)
		{
			try
			{
				ushort group = ushort.Parse(_txtDicomTagGroup.Text, NumberStyles.AllowHexSpecifier);
				ushort element = ushort.Parse(_txtDicomTagElement.Text, NumberStyles.AllowHexSpecifier);
				uint tag = (uint) (group << 16) + element;

				StudyFilterColumn.ColumnDefinition column = StudyFilterColumn.GetColumnDefinition(tag);
				if (_lstSelectedColumns.Items.Contains(column))
					return;

				_lstSelectedColumns.Items.Add(column);
				_component.Columns.Add(column);
			}
			catch (FormatException) {}
		}

		private void _txtDicomTagGroup_TextChanged(object sender, EventArgs e)
		{
			if (_txtDicomTagGroup.TextLength == 4)
			{
				_txtDicomTagElement.SelectAll();
				_txtDicomTagElement.Focus();
			}
		}

		private void _txtDicomTagElement_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				e.IsInputKey = true;
		}

		private void _txtDicomTagElement_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				_btnAddDicomTag.PerformClick();
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void _txtDicomTagGroup_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				e.IsInputKey = true;
		}

		private void _txtDicomTagGroup_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				_btnAddDicomTag.PerformClick();
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		#endregion

		#region Selected List

		private void _btnDelColumn_Click(object sender, EventArgs e)
		{
			if (_lstSelectedColumns.SelectedItems != null && _lstSelectedColumns.SelectedItems.Count > 0)
			{
				List<StudyFilterColumn.ColumnDefinition> list = new List<StudyFilterColumn.ColumnDefinition>();
				foreach (object column in _lstSelectedColumns.SelectedItems)
				{
					list.Add((StudyFilterColumn.ColumnDefinition) column);
				}
				foreach (StudyFilterColumn.ColumnDefinition column in list)
				{
					_lstSelectedColumns.Items.Remove(column);
					_component.Columns.Remove(column);
				}
			}
		}

		private void _btnMoveColumnUp_Click(object sender, EventArgs e)
		{
			if (_lstSelectedColumns.SelectedItem != null)
			{
				int index = _lstSelectedColumns.SelectedIndex;
				int newIndex = index - 1;
				StudyFilterColumn.ColumnDefinition column = (StudyFilterColumn.ColumnDefinition) _lstSelectedColumns.SelectedItem;
				if (newIndex >= 0)
				{
					_lstSelectedColumns.Items.RemoveAt(index);
					_lstSelectedColumns.Items.Insert(newIndex, column);
					_lstSelectedColumns.SelectedIndex = newIndex;
					_component.Columns.RemoveAt(index);
					_component.Columns.Insert(newIndex, column);
				}
			}
		}

		private void _btnMoveColumnDown_Click(object sender, EventArgs e)
		{
			if (_lstSelectedColumns.SelectedItem != null)
			{
				int index = _lstSelectedColumns.SelectedIndex;
				int newIndex = index + 1;
				StudyFilterColumn.ColumnDefinition column = (StudyFilterColumn.ColumnDefinition) _lstSelectedColumns.SelectedItem;
				if (newIndex < _lstSelectedColumns.Items.Count)
				{
					_lstSelectedColumns.Items.RemoveAt(index);
					_lstSelectedColumns.Items.Insert(newIndex, column);
					_lstSelectedColumns.SelectedIndex = newIndex;
					_component.Columns.RemoveAt(index);
					_component.Columns.Insert(newIndex, column);
				}
			}
		}

		#endregion
	}
}