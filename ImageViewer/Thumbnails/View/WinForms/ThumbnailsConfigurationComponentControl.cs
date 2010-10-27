#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.ImageViewer.Thumbnails.Configuration;

namespace ClearCanvas.ImageViewer.Thumbnails.View.WinForms
{
	public partial class ThumbnailsConfigurationComponentControl : UserControl
	{
		public ThumbnailsConfigurationComponentControl(ThumbnailsConfigurationComponent component)
		{
			InitializeComponent();

			_chkAutoOpenThumbnails.DataBindings.Add(new Binding("Checked", component, "AutoOpenThumbnails", true, DataSourceUpdateMode.OnPropertyChanged));
		}
	}
}