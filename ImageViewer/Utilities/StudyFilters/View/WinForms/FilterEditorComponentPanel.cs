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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.FilterNodes;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	public partial class FilterEditorComponentPanel : UserControl
	{
		private readonly FilterEditorComponent _component;

		private FilterEditorComponentPanel()
		{
			InitializeComponent();
		}

		public FilterEditorComponentPanel(FilterEditorComponent component)
			: this()
		{
			_component = component;

			foreach (FilterNodeBase filterNodeBase in component.Filters)
			{
				_lstFilters.Items.Add(filterNodeBase);
			}

			foreach (StudyFilterColumn column in StudyFilterColumn.GetSpecialColumns())
			{
				_lstSpecialColumns.Items.Add(column);
			}

			foreach (DicomTag dicomTag in DicomTagDictionary.GetDicomTagList())
			{
				_lstDicomColumns.Items.Add(StudyFilterColumn.GetDicomTagColumn(dicomTag));
			}

			foreach (object @operator in FilterBuilder.ListOperators())
			{
				_cboDicomColumnsOperator.Items.Add(@operator);
				_cboDicomTagOperators.Items.Add(@operator);
				_cboSpecialColumnOperators.Items.Add(@operator);
			}
		}

		#region Special Columns

		private void OnAddSpecialColumnClick(object sender, EventArgs e)
		{
			if (_lstSpecialColumns.SelectedItems != null && _lstSpecialColumns.SelectedItems.Count > 0 && _cboSpecialColumnOperators.SelectedItem != null)
			{
				foreach (object selectedItem in _lstSpecialColumns.SelectedItems)
				{
					StudyFilterColumn column = selectedItem as StudyFilterColumn;
					if (column == null)
						continue;
					if (_lstFilters.Items.Contains(column))
						continue;

					FilterNodeBase filter = FilterBuilder.BuildFilterNode(column, _cboSpecialColumnOperators.SelectedItem, _txtSpecialColumnsValue.Text);
					_lstFilters.Items.Add(filter);
					_component.Filters.Add(filter);
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
			if (_lstDicomColumns.SelectedItems != null && _lstDicomColumns.SelectedItems.Count > 0 && _cboDicomColumnsOperator.SelectedItem != null)
			{
				foreach (object selectedItem in _lstDicomColumns.SelectedItems)
				{
					StudyFilterColumn column = selectedItem as StudyFilterColumn;
					if (column == null)
						continue;
					if (_lstFilters.Items.Contains(column))
						continue;

					FilterNodeBase filter = FilterBuilder.BuildFilterNode(column, _cboDicomColumnsOperator.SelectedItem, _txtDicomColumnsValue.Text);
					_lstFilters.Items.Add(filter);
					_component.Filters.Add(filter);
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
			if (_cboDicomTagOperators.SelectedItem == null)
				return;

			try
			{
				ushort group = ushort.Parse(_txtDicomTagGroup.Text, NumberStyles.AllowHexSpecifier);
				ushort element = ushort.Parse(_txtDicomTagElement.Text, NumberStyles.AllowHexSpecifier);
				uint tag = (uint) (group << 16) + element;

				StudyFilterColumn column = StudyFilterColumn.GetDicomTagColumn(tag);
				if (_lstFilters.Items.Contains(column))
					return;

				FilterNodeBase filter = FilterBuilder.BuildFilterNode(column, _cboDicomTagOperators.SelectedItem, _txtDicomTagValue.Text);
				_lstFilters.Items.Add(filter);
				_component.Filters.Add(filter);
			}
			catch (Exception) {}
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
			if (_lstFilters.SelectedItems != null && _lstFilters.SelectedItems.Count > 0)
			{
				List<FilterNodeBase> list = new List<FilterNodeBase>();
				foreach (object filter in _lstFilters.SelectedItems)
				{
					list.Add((FilterNodeBase)filter);
				}
				foreach (FilterNodeBase filter in list)
				{
					_lstFilters.Items.Remove(filter);
					_component.Filters.Remove(filter);
				}
			}
		}

		private void _btnMoveColumnUp_Click(object sender, EventArgs e)
		{
			if (_lstFilters.SelectedItem != null)
			{
				int index = _lstFilters.SelectedIndex;
				int newIndex = index - 1;
				FilterNodeBase filter = (FilterNodeBase)_lstFilters.SelectedItem;
				if (newIndex >= 0)
				{
					_lstFilters.Items.RemoveAt(index);
					_lstFilters.Items.Insert(newIndex, filter);
					_lstFilters.SelectedIndex = newIndex;
					_component.Filters.RemoveAt(index);
					_component.Filters.Insert(newIndex, filter);
				}
			}
		}

		private void _btnMoveColumnDown_Click(object sender, EventArgs e)
		{
			if (_lstFilters.SelectedItem != null)
			{
				int index = _lstFilters.SelectedIndex;
				int newIndex = index + 1;
				FilterNodeBase filter = (FilterNodeBase)_lstFilters.SelectedItem;
				if (newIndex < _lstFilters.Items.Count)
				{
					_lstFilters.Items.RemoveAt(index);
					_lstFilters.Items.Insert(newIndex, filter);
					_lstFilters.SelectedIndex = newIndex;
					_component.Filters.RemoveAt(index);
					_component.Filters.Insert(newIndex, filter);
				}
			}
		}

		#endregion
	}
}