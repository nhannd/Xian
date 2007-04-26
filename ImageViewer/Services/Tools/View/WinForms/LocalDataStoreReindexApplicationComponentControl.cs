using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="LocalDataStoreReindexApplicationComponent"/>
    /// </summary>
	public partial class LocalDataStoreReindexApplicationComponentControl : ApplicationComponentUserControl
    {
        private LocalDataStoreReindexApplicationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public LocalDataStoreReindexApplicationComponentControl(LocalDataStoreReindexApplicationComponent component)
        {
            InitializeComponent();

            _component = component;

			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

			_reindexProgressControl.DataBindings.Add("StatusMessage", bindingSource, "StatusMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			_reindexProgressControl.DataBindings.Add("TotalToProcess", bindingSource, "TotalToProcess", true, DataSourceUpdateMode.OnPropertyChanged);
			_reindexProgressControl.DataBindings.Add("TotalProcessed", bindingSource, "TotalProcessed", true, DataSourceUpdateMode.OnPropertyChanged);
			_reindexProgressControl.DataBindings.Add("AvailableCount", bindingSource, "AvailableCount", true, DataSourceUpdateMode.OnPropertyChanged);
			_reindexProgressControl.DataBindings.Add("FailedSteps", bindingSource, "BadFiles", true, DataSourceUpdateMode.OnPropertyChanged);
			_reindexProgressControl.DataBindings.Add("CancelEnabled", bindingSource, "CancelEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_reindexProgressControl.CancelButtonClicked += delegate(object sender, EventArgs args) { _component.Cancel(); };
        }
    }
}
