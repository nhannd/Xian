#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ClickOncePublisher
{
	class CommonDialogHelper
	{
		public static string GetPathFromFolderDialog(string startPath)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			dlg.SelectedPath = startPath;
			dlg.ShowNewFolderButton = true;
			dlg.ShowDialog();

			return dlg.SelectedPath;
		}

		public static string GetSaveFilename(string suggestedFilename)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.InitialDirectory = Directory.GetCurrentDirectory();
			dlg.FileName = suggestedFilename;
			dlg.Filter = "XML Files|*.xml";
			DialogResult result = dlg.ShowDialog();

			if (result == DialogResult.Cancel)
				return null;

			return dlg.FileName;
		}

		public static string GetOpenFilename()
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.InitialDirectory = Directory.GetCurrentDirectory();
			dlg.Filter = "XML Files|*.xml";

			DialogResult result = dlg.ShowDialog();

			if (result == DialogResult.Cancel)
				return null;

			return dlg.FileName;
		}

		public static string GetExeFilename(string startPath)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.InitialDirectory = startPath;
			dlg.Filter = "exe Files|*.exe";

			DialogResult result = dlg.ShowDialog();

			if (result == DialogResult.Cancel)
				return null;

			return dlg.FileName;
		}
	}
}
