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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Explorer.Local;
using ClearCanvas.Desktop.Actions;
using Path=ClearCanvas.Desktop.Path;

namespace ClearCanvas.ImageViewer.TestTools
{
	[MenuAction("extractEmbeddedOverlays", "explorerlocal-contextmenu/Extract Embedded Overlays", "Go")]
	[ExtensionOf(typeof(LocalImageExplorerToolExtensionPoint))]
	public class ExtractEmbeddedOverlaysTool : Tool<ILocalImageExplorerToolContext>
	{
		public void Go()
		{
			string[] files = BuildFileList();
			var args = new SelectFolderDialogCreationArgs
			{
				Path = GetDirectoryOfFirstPath(),
				AllowCreateNewFolder = true,
				Prompt = "Select output folder"
			};

			var result = base.Context.DesktopWindow.ShowSelectFolderDialogBox(args);
			if (result.Action != DialogBoxAction.Ok)
				return;

			try
			{
				foreach (string file in files)
				{
					DicomFile dicomFile = new DicomFile(file);
					dicomFile.Load();
					if (!new OverlayPlaneModuleIod(dicomFile.DataSet).ExtractEmbeddedOverlays())
						continue;

					string sourceFileName = System.IO.Path.GetFileNameWithoutExtension(file);
					string fileName = System.IO.Path.Combine(result.FileName, sourceFileName);
					fileName += ".overlays-extracted.dcm";
					dicomFile.Save(fileName);
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, Context.DesktopWindow);
			}
		}

		private string GetDirectoryOfFirstPath()
		{
			foreach (string path in Context.SelectedPaths)
				return System.IO.Path.GetDirectoryName(path);

			return null;
		}

		private string[] BuildFileList()
		{
			List<string> fileList = new List<string>();

			foreach (string path in this.Context.SelectedPaths)
			{
				if (File.Exists(path))
					fileList.Add(path);
				else if (Directory.Exists(path))
					fileList.AddRange(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories));
			}

			return fileList.ToArray();
		}
	}
}
