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
			this.CreateVolumeButton.Click += new EventHandler(OnCreateVolumeButtonClick);
			_tabControl.Selected += new TabControlEventHandler(OnTabSelected);
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
    }
}
