using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="WindowLevelConfigurationApplicationComponent"/>
    /// </summary>
    public partial class WindowLevelConfigurationApplicationComponentControl : ApplicationComponentUserControl
    {
        private WindowLevelConfigurationApplicationComponent _component;
		private BindingSource _bindingSource;

        /// <summary>
        /// Constructor
        /// </summary>
        public WindowLevelConfigurationApplicationComponentControl(WindowLevelConfigurationApplicationComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

			_modalityCombo.DataSource = _component.ModalityList;
			_modalityCombo.ValueChanged += new EventHandler(OnModalityChanged);

			_bindingSource = new BindingSource();

			_bindingSource.DataSource = _component;
			_presetTable.DataBindings.Add("Table", _component, "SelectedPresetList", true, DataSourceUpdateMode.OnPropertyChanged);

			_presetTable.ToolStripItemDisplayStyle = ToolStripItemDisplayStyle.Image;
			_presetTable.AddClicked += new EventHandler(OnCreateClicked);
			_presetTable.EditClicked += new EventHandler(OnEditClicked);
			_presetTable.DeleteClicked += new EventHandler(OnDeleteClicked);
			_presetTable.SelectionChanged += new EventHandler(OnPresetSelectionChanged);

			_component.SetSelection(_presetTable.Selection);
		}

		void OnModalityChanged(object sender, EventArgs e)
		{
			_component.SelectedModality = _modalityCombo.Value.ToString();
		}

		void OnCreateClicked(object sender, EventArgs e)
		{
			_component.AddPreset();
		}
		
		void OnEditClicked(object sender, EventArgs e)
		{
			_component.EditPreset();
		}

		void OnDeleteClicked(object sender, EventArgs e)
		{
			_component.DeletePreset();
		}

		void OnPresetSelectionChanged(object sender, EventArgs e)
		{
			_component.SetSelection(_presetTable.Selection);
		}
	}
}
