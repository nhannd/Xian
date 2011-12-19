#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using Crownwood.DotNetMagic.Forms;

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
	public partial class ReindexStartupDialogForm : DotNetMagicForm
	{
		public ReindexStartupDialogForm()
		{
			InitializeComponent();
		}

		public string Message
		{
			get { return _message.Text; }
			set { _message.Text = value ?? ""; }
		}
	}
}
