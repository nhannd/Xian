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
	/// Provides a Windows Forms user-interface for <see cref="PublishingConfigurationComponent"/>.
	/// </summary>
	public partial class PublishingConfigurationComponentControl : ApplicationComponentUserControl
	{
		private readonly PublishingConfigurationComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public PublishingConfigurationComponentControl(PublishingConfigurationComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			_publishToDefaultServers.DataBindings.Add("Checked", _component, "PublishToDefaultServers", false, DataSourceUpdateMode.OnPropertyChanged);
			_publishLocalToSourceAE.DataBindings.Add("Checked", _component, "PublishLocalToSourceAE", false, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}