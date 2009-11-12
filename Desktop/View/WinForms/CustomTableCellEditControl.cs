using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.Tables;
using System.Drawing;

namespace ClearCanvas.Desktop.View.WinForms
{
	/*
	 * All code in this file was adapted from the following MSDN article:
	 * http://msdn.microsoft.com/en-us/library/aa730881%28VS.80%29.aspx
	 * 
	 */



	/// <summary>
	/// Defines a column that allows for an arbitrary custom editing control to be displayed.
	/// </summary>
	internal class CustomEditableTableViewColumn : DataGridViewColumn
	{
		internal CustomEditableTableViewColumn(ITable table, ITableColumn tableColumn)
			: base(new CustomEditableTableViewCell(table, tableColumn))
		{
		}

		/// <summary>
		/// Gets or sets the template used to create new cells.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Windows.Forms.DataGridViewCell"/> that all other cells in the column are modeled after. The default is null.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override DataGridViewCell CellTemplate
		{
			get
			{
				return base.CellTemplate;
			}
			set
			{
				// Ensure that the cell used for the template is a CustomEditableTableViewCell.
				if (value != null &&
					!value.GetType().IsAssignableFrom(typeof(CustomEditableTableViewCell)))
				{
					throw new InvalidCastException("Must be a CustomEditableTableViewCell");
				}
				base.CellTemplate = value;
			}
		}
	}

	/// <summary>
	/// Defines a cell that allows for an arbitrary custom editing control to be displayed.
	/// </summary>
	internal class CustomEditableTableViewCell : DataGridViewTextBoxCell
	{
		private readonly ITableColumn _tableColumn;
		private readonly ITable _table;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="tableColumn"></param>
		internal CustomEditableTableViewCell(ITable table, ITableColumn tableColumn)
		{
			_table = table;
			_tableColumn = tableColumn;
		}

		/// <summary>
		/// Creates an exact copy of this cell.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Object"/> that represents the cloned <see cref="CustomEditableTableViewCell"/>.
		/// </returns>
		/// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
		public override object Clone()
		{
			return new CustomEditableTableViewCell(_table, _tableColumn);
		}

		/// <summary>
		/// Attaches and initializes the hosted editing control.
		/// </summary>
		/// <param name="rowIndex">The index of the row being edited.
		///                 </param><param name="initialFormattedValue">The initial value to be displayed in the control.
		///                 </param><param name="dataGridViewCellStyle">A cell style that is used to determine the appearance of the hosted control.
		///                 </param><filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
		public override void InitializeEditingControl(int rowIndex, object
			initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
		{
			// Set the value of the editing control to the current cell value.
			base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

			var editor = _tableColumn.GetCellEditor();
			var ctl = DataGridView.EditingControl as CustomTableCellEditControl;

			if (ctl != null && editor != null)
			{
				// notify control that it is about to begin editing this column and item
				ctl.BeginEdit(_tableColumn, _table.Items[rowIndex]);
			}
		}

		/// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
		public override void DetachEditingControl()
		{
			var ctl = DataGridView.EditingControl as CustomTableCellEditControl;
			if (ctl != null)
			{
				// Editing controls get recycled. Indeed, when a DataGridViewNumericUpDownCell cell gets edited
				// after another DataGridViewNumericUpDownCell cell, the same editing control gets reused for 
				// performance reasons (to avoid an unnecessary control destruction and creation). 
				// Here the undo buffer of the TextBox inside the NumericUpDown control gets cleared to avoid
				// interferences between the editing sessions.
			}

			base.DetachEditingControl();
		}


		/// <summary>
		/// Gets the type of the cell's hosted editing control. 
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Type"/> representing the <see cref="T:System.Windows.Forms.DataGridViewTextBoxEditingControl"/> type.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override Type EditType
		{
			get { return typeof(CustomTableCellEditControl); }
		}

		/// <returns>
		/// A <see cref="T:System.Type"/> representing the data type of the value in the cell.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override Type ValueType
		{
			get { return _tableColumn.ColumnType; }
		}

		/// <summary>
		/// Gets the default value for a cell in the row for new records.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Object"/> representing the default value.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override object DefaultNewRowValue
		{
			get { return null; }
		}
	}

	/// <summary>
	/// Acts as host control for custom editing.
	/// </summary>
	internal class CustomTableCellEditControl : UserControl, IDataGridViewEditingControl
	{
		private ITableColumn _tableColumn;
		private ITableCellEditor _editor;
		private ITableCellEditorView _editorView;

		/// <summary>
		/// Constructor.
		/// </summary>
		public CustomTableCellEditControl()
		{
			EditingControlValueChanged = false;
		}

		/// <summary>
		/// Prepares this control to edit the specified column and item.
		/// </summary>
		/// <param name="tableColumn"></param>
		/// <param name="item"></param>
		public void BeginEdit(ITableColumn tableColumn, object item)
		{
			// remove any previous event subscription
			if (_editor != null)
			{
				_editor.ValueChanged -= ValueChangedEventHandler;
			}

			_tableColumn = tableColumn;

			// get cell editor for this column
			_editor = tableColumn.GetCellEditor();
			_editor.BeginEdit(item);

			// subscribe to event so we know when cell is dirtied
			_editor.ValueChanged += ValueChangedEventHandler;

			// create editor view only once
			if (_editorView == null)
			{
				_editorView = (ITableCellEditorView)ViewFactory.CreateAssociatedView(_editor.GetType());
				var control = (Control) _editorView.GuiElement;
				control.Dock = DockStyle.Fill;
				this.Controls.Add(control);
			}
			// associate view with current editor
			_editorView.SetEditor(_editor);
		}

		/// <summary>
		/// Gets or sets the formatted value of the cell being modified by the editor.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Object"/> that represents the formatted value of the cell.
		/// </returns>
		public object EditingControlFormattedValue
		{
			get
			{
				return _tableColumn.FormatValue(_editor.Value);
			}
			set
			{
				throw new NotImplementedException("EditingControlFormattedValue");
			}
		}

		/// <summary>
		/// Retrieves the formatted value of the cell.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Object"/> that represents the formatted version of the cell contents.
		/// </returns>
		/// <param name="context">A bitwise combination of <see cref="T:System.Windows.Forms.DataGridViewDataErrorContexts"/> values that specifies the context in which the data is needed.
		///                 </param>
		public object GetEditingControlFormattedValue(
			DataGridViewDataErrorContexts context)
		{
			return EditingControlFormattedValue;
		}

		/// <summary>
		/// Changes the control's user interface (UI) to be consistent with the specified cell style.
		/// </summary>
		/// <param name="dataGridViewCellStyle">The <see cref="T:System.Windows.Forms.DataGridViewCellStyle"/> to use as the model for the UI.
		///                 </param>
		public void ApplyCellStyleToEditingControl(
			DataGridViewCellStyle dataGridViewCellStyle)
		{
			// don't care
		}

		/// <summary>
		/// Gets or sets the index of the hosting cell's parent row.
		/// </summary>
		/// <returns>
		/// The index of the row that contains the cell, or –1 if there is no parent row.
		/// </returns>
		public int EditingControlRowIndex { get; set; }

		/// <summary>
		/// Determines whether the specified key is a regular input key that the editing control should process or a special key that the <see cref="T:System.Windows.Forms.DataGridView"/> should process.
		/// </summary>
		/// <returns>
		/// true if the specified key is a regular input key that should be handled by the editing control; otherwise, false.
		/// </returns>
		/// <param name="keyData">A <see cref="T:System.Windows.Forms.Keys"/> that represents the key that was pressed.
		///                 </param><param name="dataGridViewWantsInputKey">true when the <see cref="T:System.Windows.Forms.DataGridView"/> wants to process the <see cref="T:System.Windows.Forms.Keys"/> in <paramref name="keyData"/>; otherwise, false.
		///                 </param>
		public bool EditingControlWantsInputKey(
			Keys keyData, bool dataGridViewWantsInputKey)
		{
			// Let the editing control handle the keys listed
			// plus any keys that the dgv doesn't want
			switch (keyData & Keys.KeyCode)
			{
				case Keys.Left:
				case Keys.Up:
				case Keys.Down:
				case Keys.Right:
				case Keys.Home:
				case Keys.End:
				case Keys.PageDown:
				case Keys.PageUp:
					return true;
				default:
					return !dataGridViewWantsInputKey;
			}
		}

		/// <summary>
		/// Prepares the currently selected cell for editing.
		/// </summary>
		/// <param name="selectAll">true to select all of the cell's content; otherwise, false.
		///                 </param>
		public void PrepareEditingControlForEdit(bool selectAll)
		{
			// No preparation needs to be done.
		}

		/// <summary>
		/// Gets or sets a value indicating whether the cell contents need to be repositioned whenever the value changes.
		/// </summary>
		/// <returns>
		/// true if the contents need to be repositioned; otherwise, false.
		/// </returns>
		public bool RepositionEditingControlOnValueChange
		{
			get { return false; }
		}

		/// <summary>
		/// Gets or sets the <see cref="T:System.Windows.Forms.DataGridView"/> that contains the cell.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Windows.Forms.DataGridView"/> that contains the <see cref="T:System.Windows.Forms.DataGridViewCell"/> that is being edited; null if there is no associated <see cref="T:System.Windows.Forms.DataGridView"/>.
		/// </returns>
		public DataGridView EditingControlDataGridView { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the value of the editing control differs from the value of the hosting cell.
		/// </summary>
		/// <returns>
		/// true if the value of the control differs from the cell value; otherwise, false.
		/// </returns>
		public bool EditingControlValueChanged { get; set; }

		/// <summary>
		/// Gets the cursor used when the mouse pointer is over the <see cref="P:System.Windows.Forms.DataGridView.EditingPanel"/> but not over the editing control.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Windows.Forms.Cursor"/> that represents the mouse pointer used for the editing panel. 
		/// </returns>
		public Cursor EditingPanelCursor
		{
			get { return base.Cursor; }
		}

		private void ValueChangedEventHandler(object sender, EventArgs e)
		{
			// on value changed, mark the cell as being dirty
			this.EditingControlValueChanged = true;
			this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
		}
	}
}
