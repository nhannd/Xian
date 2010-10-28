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

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="BiographyOrderHistoryComponentControl"/>
	/// </summary>
	public partial class BiographyOrderHistoryComponentControl : ApplicationComponentUserControl
	{
		private readonly BiographyOrderHistoryComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public BiographyOrderHistoryComponentControl(BiographyOrderHistoryComponent component)
		{
			InitializeComponent();
			_component = component;

			_orderList.Table = _component.Orders;
			_orderList.DataBindings.Add("Selection", _component, "SelectedOrder", true, DataSourceUpdateMode.OnPropertyChanged);

			// Load initial value
			_banner.Text = _component.BannerText;
			_banner.DataBindings.Add("Text", _component, "BannerText", true, DataSourceUpdateMode.OnPropertyChanged);

			Control content = (Control)_component.RightHandComponentContainerHost.ComponentView.GuiElement;
			content.Dock = DockStyle.Fill;
			_tabHostPanel.Controls.Add(content);
		}
	}
}
