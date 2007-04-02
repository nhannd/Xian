using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomFileImportApplicationComponent"/>
    /// </summary>
    public partial class DicomFileImportApplicationComponentControl : UserControl
    {
        private DicomFileImportApplicationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomFileImportApplicationComponentControl(DicomFileImportApplicationComponent component)
        {
            InitializeComponent();

			_component = component;

			_importTable.ToolStripItemDisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
			_importTable.Table = _component.ImportTable;

			_importTable.ToolbarModel = _component.ToolbarModel;
			_importTable.MenuModel = _component.ContextMenuModel;

			_importTable.SelectionChanged += new EventHandler(OnSelectionChanged);

			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

			_titleBar.DataBindings.Add("Text", bindingSource, "Title", true, DataSourceUpdateMode.OnPropertyChanged);

			_importProgressControl.DataBindings.Add("StatusMessage", bindingSource, "SelectedStatusMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			_importProgressControl.DataBindings.Add("TotalToProcess", bindingSource, "SelectedTotalToProcess", true, DataSourceUpdateMode.OnPropertyChanged);
			_importProgressControl.DataBindings.Add("TotalProcessed", bindingSource, "SelectedTotalProcessed", true, DataSourceUpdateMode.OnPropertyChanged);
			_importProgressControl.DataBindings.Add("AvailableCount", bindingSource, "SelectedAvailableCount", true, DataSourceUpdateMode.OnPropertyChanged);
			_importProgressControl.DataBindings.Add("FailedSteps", bindingSource, "SelectedBadFiles", true, DataSourceUpdateMode.OnPropertyChanged);
			_importProgressControl.DataBindings.Add("CancelEnabled", bindingSource, "SelectedCancelEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_importProgressControl.CancelButtonClicked += delegate(object sender, EventArgs args) { _component.CancelSelected(); };
		}

		void OnSelectionChanged(object sender, EventArgs e)
		{
			_component.SetSelection(_importTable.Selection);
		}
	}
}
