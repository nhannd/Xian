#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomServerConfigurationComponent"/>
    /// </summary>
    public partial class DicomServerConfigurationComponentControl : ApplicationComponentUserControl
    {
        private DicomServerConfigurationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerConfigurationComponentControl(DicomServerConfigurationComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

			_aeTitle.DataBindings.Add("Value", _component, "AETitle", true, DataSourceUpdateMode.OnPropertyChanged);
			Binding portBinding = new Binding("Value", _component, "Port", true, DataSourceUpdateMode.OnPropertyChanged);
			portBinding.Format += new ConvertEventHandler(OnPortBindingFormat);
			portBinding.Parse += new ConvertEventHandler(OnPortBindingParse);
			_port.DataBindings.Add(portBinding);
        }

		void OnPortBindingFormat(object sender, ConvertEventArgs e)
		{
			if (e.DesiredType != typeof(string))
				return;

			if ((int)e.Value <= 0)
				e.Value = "";
			else
				e.Value = e.Value.ToString();
		}

		void OnPortBindingParse(object sender, ConvertEventArgs e)
		{
			if (e.DesiredType != typeof(int))
				return;

			int value;
			if (!(e.Value is string) || !int.TryParse((string)e.Value, out value))
			{
				e.Value = 0;
			}
			else
			{
				e.Value = value;
			}
		}
    }
}
