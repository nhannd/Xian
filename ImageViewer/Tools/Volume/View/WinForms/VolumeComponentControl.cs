using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Volume.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="VolumeComponent"/>
    /// </summary>
    public partial class VolumeComponentControl : CustomUserControl
    {
		private BindingSource _bindingSource;
		private VolumeComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public VolumeComponentControl(VolumeComponent component)
        {
			_component = component;

			InitializeComponent();

			_component.TissueSettingsCollection.ItemAdded += new EventHandler<TissueSettingsEventArgs>(OnTissueSettingsAdded);
			AddDefaultTissueSettings();
			this._createVolumeButton.Click += new EventHandler(OnCreateVolumeButtonClick);
			_tabControl.Selected += new TabControlEventHandler(OnTabSelected);

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _component;

			_component.SubjectChanged += new EventHandler(OnSubjectChanged);
			_createVolumeButton.DataBindings.Add("Enabled", _bindingSource, "CreateVolumeEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_tabControl.DataBindings.Add("Enabled", _bindingSource, "VolumeSettingsEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
		}


		private void AddDefaultTissueSettings()
		{
			_component.TissueSettingsCollection.Add(new TissueSettings());
			_component.TissueSettingsCollection.Add(new TissueSettings());
			_component.TissueSettingsCollection.Add(new TissueSettings());
		}

		void OnTissueSettingsAdded(object sender, TissueSettingsEventArgs e)
		{
			TabPage tabPage = new TabPage("Tissue");
			TissueControl control = new TissueControl(e.TissueSettings);
			tabPage.Controls.Add(control);
			control.Dock = DockStyle.Fill;
			_tabControl.TabPages.Add(tabPage);
		}

		void OnCreateVolumeButtonClick(object sender, EventArgs e)
		{
			_component.CreateVolume();
		}

		void OnTabSelected(object sender, TabControlEventArgs e)
		{
			_component.SelectTissue(e.TabPageIndex);
		}

		void OnSubjectChanged(object sender, EventArgs e)
		{
			_bindingSource.ResetBindings(false);
		}

    }
}
