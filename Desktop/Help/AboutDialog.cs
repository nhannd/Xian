#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Help
{
	[ExtensionPoint]
	public sealed class AboutDialogExtensionPoint : ExtensionPoint<IAboutDialog>
	{
		public static IAboutDialog CreateDialog()
		{
			try
			{
				return (IAboutDialog) new AboutDialogExtensionPoint().CreateExtension();
			}
			catch (Exception)
			{
				return new AboutForm();
			}
		}
	}

	public interface IAboutDialog
	{
		DialogResult ShowDialog();
	}
}