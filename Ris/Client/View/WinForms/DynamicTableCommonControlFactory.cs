using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	public static class DynamicTableCommonControlFactory
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

		public static TextBox CreateTextBoxControl(string controlName, object defaultValue)
		{
			var control = new TextBox
				{
					Name = controlName,
					Dock = DockStyle.Fill,
					Text = defaultValue != null ? defaultValue.ToString() : null
				};
			return control;
		}

		public static TextBox CreateTextAreaControl(string controlName, object defaultValue)
		{
			var control = CreateTextBoxControl(controlName, defaultValue);
			control.Multiline = true;
			control.Height = 3 * control.Height;
			control.ScrollBars = ScrollBars.Vertical;
			return control;
		}

		public static NumericUpDown CreateNumericControl(string controlName, object defaultValue)
		{
			Decimal decimalValue = 0;
			var isParsed = defaultValue != null && Decimal.TryParse(defaultValue.ToString(), out decimalValue);

			var control = new NumericUpDown
				{
					Name = controlName,
					Dock = DockStyle.Fill,
					Maximum = Decimal.MaxValue,
					Minimum = Decimal.MinValue
				};

			if (isParsed)
				control.Value = decimalValue;
			else
				control.Text = null;

			return control;
		}

		public static CheckBox CreateCheckBoxControl(string controlName, object defaultValue, bool allowNull)
		{
			var control = new CheckBox
				{
					Name = controlName,
					Dock = DockStyle.Fill,
					Text = null
				};

			control.CheckStateChanged += delegate { control.Text = control.CheckState == CheckState.Indeterminate ? "Null" : control.Checked.ToString(); };
			control.ThreeState = allowNull;
			if (defaultValue == null)
				control.CheckState = allowNull ? CheckState.Indeterminate : CheckState.Unchecked;
			else
				control.Checked = defaultValue.ToString() == "1";

			return control;
		}

		public static DateTimePicker CreateDateTimeControl(string controlName, object defaultValue, bool allowNull, bool includeTime)
		{
			var dateTime = DateTime.Now;
			var isParsed = defaultValue != null && DateTime.TryParse(defaultValue.ToString(), out dateTime);

			var control = new DateTimePicker
				{
					Name = controlName,
					Dock = DockStyle.Fill,
					Format = DateTimePickerFormat.Custom,
					CustomFormat = includeTime ? Format.DateTimeFormat : Format.DateFormat,
					ShowCheckBox = allowNull,
					Value = dateTime,
					Checked = allowNull ? isParsed : true
				};

			return control;
		}

		public static ComboBox CreateComboBoxControl(string controlName, object defaultValue, List<object> valueChoices)
		{
			var control = new ComboBox
				{
					Name = controlName,
					Dock = DockStyle.Fill
				};
			control.Items.AddRange(valueChoices.ToArray());
			control.SelectedItem = defaultValue;
			control.Enabled = valueChoices.Count > 1 && defaultValue != null;
			return control;
		}

		public static LookupField CreateLookupControl(string controlName, object defaultValue, ILookupHandler lookupHandler)
		{
			var control = new LookupField
				{
					Name = controlName,
					Dock = DockStyle.Fill,
					LabelText = null,
					Value = defaultValue != null ? defaultValue.ToString() : null,
					LookupHandler = lookupHandler
				};
			return control;
		}

		public static object GetValue(Control control)
		{
			if (control is NumericUpDown)
			{
				if (string.IsNullOrEmpty(control.Text))
					return null;

				var numericUpDown = (NumericUpDown) control;
				return numericUpDown.Value;
			}

			if (control is CheckBox)
			{
				var checkBox = (CheckBox)control;
				if (checkBox.CheckState == CheckState.Indeterminate)
					return null;

				return checkBox.Checked;
			}

			if (control is DateTimePicker)
			{
				var dateTimePicker = (DateTimePicker)control;
				if (dateTimePicker.Checked)
					return dateTimePicker.Value;

				return null;
			}

			if (control is ComboBox)
			{
				var comboBox = (ComboBox) control;
				return comboBox.SelectedItem;
			}

			if (control is LookupField)
			{
				var lookupField = (LookupField) control;
				return lookupField.Value;
			}

			// Default case: TextBox
			return string.IsNullOrEmpty(control.Text) ? null : control.Text;
		}
	}
}
