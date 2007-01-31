using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration.Standard;

namespace ClearCanvas.Desktop.View.WinForms.Configuration.Standard
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomConfigurationApplicationComponent"/>
    /// </summary>
    public partial class DateFormatApplicationComponentControl : UserControl
    {
		private DateFormatApplicationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DateFormatApplicationComponentControl(DateFormatApplicationComponent component)
        {
            InitializeComponent();
			
			BindingSource source = new BindingSource();
			source.DataSource = _component = component;

			_comboCustomDateFormat.DataSource = _component.AvailableCustomFormats;
			//for whatever reason, the combobox's initial value doesn't get set via the binding, so we'll just set it explicitly.
			_comboCustomDateFormat.SelectedIndex = _comboCustomDateFormat.Items.IndexOf(_component.SelectedCustomFormat);
			_comboCustomDateFormat.DataBindings.Add("SelectedValue", source, "SelectedCustomFormat", true, DataSourceUpdateMode.OnPropertyChanged);
			_comboCustomDateFormat.DataBindings.Add("Enabled", _radioCustom, "Checked", true, DataSourceUpdateMode.OnPropertyChanged);

			_dateSample.DataBindings.Add("Text", source, "SampleDate", true, DataSourceUpdateMode.OnPropertyChanged);

			Binding customBinding = new Binding("Checked", source, "FormatOption", true, DataSourceUpdateMode.OnPropertyChanged);
			Binding systemShortBinding = new Binding("Checked", source, "FormatOption", true, DataSourceUpdateMode.OnPropertyChanged);
			Binding systemLongBinding = new Binding("Checked", source, "FormatOption", true, DataSourceUpdateMode.OnPropertyChanged);
			
			_radioCustom.DataBindings.Add("Enabled", source, "CustomFormatsEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			
			_radioCustom.DataBindings.Add(customBinding);
			_radioSystemLongDate.DataBindings.Add(systemLongBinding);
			_radioSystemShortDate.DataBindings.Add(systemShortBinding);

			customBinding.Parse += new ConvertEventHandler(OnRadioBindingParse);
			systemLongBinding.Parse += new ConvertEventHandler(OnRadioBindingParse);
			systemShortBinding.Parse += new ConvertEventHandler(OnRadioBindingParse);

			//to use databindings on a group of radio buttons, this is essentially what you have to do.  You might also be
			//able to use a groupbox, but this is easy enough to do.
			customBinding.Format += delegate(object sender, ConvertEventArgs e)
			{
				if (e.DesiredType != typeof(bool))
					return;

				e.Value = ((DateFormatApplicationComponent.DateFormatOptions)e.Value) == DateFormatApplicationComponent.DateFormatOptions.Custom;
			};

			systemLongBinding.Format += delegate(object sender, ConvertEventArgs e)
			{
				if (e.DesiredType != typeof(bool))
					return;

				e.Value = ((DateFormatApplicationComponent.DateFormatOptions)e.Value) == DateFormatApplicationComponent.DateFormatOptions.SystemLong;
			};

			systemShortBinding.Format += delegate(object sender, ConvertEventArgs e)
			{
				if (e.DesiredType != typeof(bool))
					return;

				e.Value = ((DateFormatApplicationComponent.DateFormatOptions)e.Value) == DateFormatApplicationComponent.DateFormatOptions.SystemShort;
			};
		}

		void OnRadioBindingParse(object sender, ConvertEventArgs e)
		{
			if (e.DesiredType != typeof(DateFormatApplicationComponent.DateFormatOptions))
				return;

			if (_radioCustom.Checked)
				e.Value = DateFormatApplicationComponent.DateFormatOptions.Custom;
			else if (_radioSystemLongDate.Checked)
				e.Value = DateFormatApplicationComponent.DateFormatOptions.SystemLong;
			else
				e.Value = DateFormatApplicationComponent.DateFormatOptions.SystemShort;
		}
    }
}
