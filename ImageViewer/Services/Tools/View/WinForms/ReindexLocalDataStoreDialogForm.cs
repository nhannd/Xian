#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.Common.LocalDataStore;
using Crownwood.DotNetMagic.Forms;
using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
	public partial class ReindexLocalDataStoreDialogForm : DotNetMagicForm
	{
		public ReindexLocalDataStoreDialogForm(ILocalDataStoreReindexer reindexer)
		{
			InitializeComponent();

			this.Text = SR.TitleReindexing;
			var control = new LocalDataStoreReindexApplicationComponentControl(reindexer);
			control.Dock = DockStyle.Fill;
			this._contentPanel.Controls.Add(control);
		}
	}
}