#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="BiographyDemographicComponent"/>
	/// </summary>
	public partial class BiographyDemographicComponentControl : ApplicationComponentUserControl
	{
		private readonly BiographyDemographicComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public BiographyDemographicComponentControl(BiographyDemographicComponent component)
			:base(component)
		{
			InitializeComponent();
			_component = component;

			_selectedProfile.DataSource = _component.ProfileChoices;
			_selectedProfile.DataBindings.Add("Value", _component, "SelectedProfile", true, DataSourceUpdateMode.OnPropertyChanged);
			_selectedProfile.Format += delegate(object sender, ListControlConvertEventArgs e) { e.Value = _component.FormatPatientProfile(e.ListItem); };

			var profileViewer = (Control)_component.ProfileViewComponentHost.ComponentView.GuiElement;
			profileViewer.Dock = DockStyle.Fill;
			this._demoHostPanel.Controls.Add(profileViewer);

			_component.AllPropertiesChanged += AllPropertiesChangedEventHandler;
		}

		private void AllPropertiesChangedEventHandler(object sender, EventArgs e)
		{
			_selectedProfile.DataSource = _component.ProfileChoices;
		}
	}
}
