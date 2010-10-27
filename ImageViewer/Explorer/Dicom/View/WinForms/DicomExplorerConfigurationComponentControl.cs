#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomExplorerConfigurationApplicationComponent"/>
    /// </summary>
    public partial class DicomExplorerConfigurationComponentControl : ApplicationComponentUserControl
    {
        private DicomExplorerConfigurationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomExplorerConfigurationComponentControl(DicomExplorerConfigurationComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

			_selectDefaultServerOnStartup.DataBindings.Add("Checked", bindingSource, "SelectDefaultServerOnStartup", true, DataSourceUpdateMode.OnPropertyChanged);
			_showNumberOfImages.DataBindings.Add("Checked", bindingSource, "ShowNumberOfImagesInStudy", true, DataSourceUpdateMode.OnPropertyChanged);
			_showPhoneticIdeographicNames.DataBindings.Add("Checked", bindingSource, "ShowPhoneticIdeographicNames", true, DataSourceUpdateMode.OnPropertyChanged);
		}
    }
}
