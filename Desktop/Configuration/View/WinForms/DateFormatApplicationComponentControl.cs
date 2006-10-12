using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Configuration.View.WinForms
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
			
			_component = component;

			_comboCustomDateFormat.DataSource = _component.AvailableCustomDateFormats;
			_comboCustomDateFormat.SelectedIndex = _comboCustomDateFormat.Items.IndexOf(_component.SelectedCustomDateFormat);

			//I tried to do this using databindings on the radio buttons, but it just didn't work right so I gave up.

			_comboCustomDateFormat.SelectedIndexChanged += delegate(object sender, EventArgs e)
			{
				_component.SelectedCustomDateFormat = (string)_comboCustomDateFormat.Items[_comboCustomDateFormat.SelectedIndex];
				_dateSample.Text = _component.SampleDate;
			};

			_radioCustom.CheckedChanged += delegate(object sender, EventArgs e)
			{
				_comboCustomDateFormat.Enabled = _radioCustom.Checked;
				
				_component.UseCustomDate = _radioCustom.Checked;
				_dateSample.Text = _component.SampleDate;
			};

			_radioSystemShortDate.CheckedChanged += delegate(object sender, EventArgs e)
			{
				_component.UseSystemShortDate = _radioSystemShortDate.Checked;
				_dateSample.Text = _component.SampleDate;
			};

			_radioSystemLongDate.CheckedChanged += delegate(object sender, EventArgs e)
			{
				_component.UseSystemLongDate = _radioSystemLongDate.Checked;
				_dateSample.Text = _component.SampleDate;
			};

			_radioUnchanged.CheckedChanged += delegate(object sender, EventArgs e)
			{
				_component.UseUnchangedDate = _radioUnchanged.Checked;
				_dateSample.Text = _component.SampleDate;
			};
		
			_radioSystemShortDate.Checked = _component.UseSystemShortDate;
			_radioSystemLongDate.Checked = _component.UseSystemLongDate;
			_radioUnchanged.Checked = _component.UseUnchangedDate;
			_radioCustom.Checked = _component.UseCustomDate;
		}
    }
}
