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

using System.Windows.Forms;
using ClearCanvas.Desktop.Configuration.Standard;

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
