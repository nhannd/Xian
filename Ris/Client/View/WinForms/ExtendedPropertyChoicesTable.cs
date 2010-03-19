using System.Collections.Generic;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Defines a custom table that allows user to choose a list of value for each property.
	/// </summary>
	public class ExtendedPropertyChoicesTable : TableLayoutPanel
	{
		private static class ControlFactory
		{
			public static Label CreateLabelControl(string controlName, object defaultValue)
			{
				var control = new Label
								{
									Name = controlName,
									Dock = DockStyle.Fill,
									TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
									Text = defaultValue != null ? defaultValue.ToString() : null
								};
				return control;
			}

			public static ComboBox CreateComboBoxControl(string controlName, object defaultValue, List<object> valueChoices)
			{
				var control = new ComboBox {Name = controlName, Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
				control.Items.AddRange(valueChoices.ToArray());
				control.SelectedItem = defaultValue;
				control.Enabled = valueChoices.Count > 1 && defaultValue != null;
				return control;
			}
		}

		private const int ColumnProperty = 0;
		private const int ColumnValue = 1;
		private const float DefaultHeight = 25F;

		private readonly Dictionary<ExtendedPropertyChoicesTableData, Control> _propertiesMap;

		public ExtendedPropertyChoicesTable()
		{
			_propertiesMap = new Dictionary<ExtendedPropertyChoicesTableData, Control>();
		}

		#region Public Methods & Properties

		public void Clear()
		{
			// Redrawing controls one at a time will take time.  Set it to invisible first
			this.Visible = false;

			// Clear existing table
			this.RowCount = 1;
			this.Controls.Clear();
			_propertiesMap.Clear();

			this.Visible = true;
		}

		/// <summary>
		/// Refresh the properties table.
		/// </summary>
		/// <param name="properties"></param>
		public void Reload(List<ExtendedPropertyChoicesTableData> properties)
		{
			// Redrawing controls one at a time will take time.  Set it to invisible first
			this.Visible = false;

			// Clear existing table
			this.RowCount = 1;
			this.Controls.Clear();
			_propertiesMap.Clear();

			// Add header and all the rows for each property
			CollectionUtils.ForEach(properties, AddRow);

			this.RowStyles[0].Height = DefaultHeight;

			// Make this table visible again.
			this.Visible = true;
		}

		/// <summary>
		/// Gets a dictionary of the property name vs its current value in the control.
		/// </summary>
		public Dictionary<string, string> CurrentValues
		{
			get
			{
				var currentValues = new Dictionary<string, string>();
				CollectionUtils.ForEach(_propertiesMap,
					delegate(KeyValuePair<ExtendedPropertyChoicesTableData, Control> kvp)
						{
							var selectedItemText = kvp.Value.Text;
							currentValues.Add(kvp.Key.PropertyName, selectedItemText);
						});

				return currentValues;
			}
		}

		#endregion

		#region Private Helpers

		private void AddRow(ExtendedPropertyChoicesTableData property)
		{
			var row = this.RowCount - 1;

			// Add a Label control for field Name column
			var fieldControlName = GetControlName("Field", property.PropertyName);
			var fieldControl = ControlFactory.CreateLabelControl(fieldControlName, property.PropertyName);
			this.Controls.Add(fieldControl, ColumnProperty, row);

			var valueControlName = GetControlName("Value", property.PropertyName);
			var valueControl = ControlFactory.CreateComboBoxControl(valueControlName, property.ValueChoices[0], property.ValueChoices);
			this.Controls.Add(valueControl, ColumnValue, row);

			_propertiesMap.Add(property, valueControl);

			this.RowCount++;
		}

		private static string GetControlName(string column, string name)
		{
			return string.Format("{0}_{1}", column, name);	
		}
		
		#endregion
	}
}
