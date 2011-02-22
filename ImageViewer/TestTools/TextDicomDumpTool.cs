#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Explorer.Local;
using ClearCanvas.Desktop.Actions;
using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.TestTools
{
	[MenuAction("go", "explorerlocal-contextmenu/Dump Files (Text)", "Go")]
	[ExtensionOf(typeof(LocalImageExplorerToolExtensionPoint))]
	public class TextDicomDumpTool : Tool<ILocalImageExplorerToolContext>
	{
		public void Go()
		{
			var result = base.Context.DesktopWindow.ShowSaveFileDialogBox(new FileDialogCreationArgs("dump.txt"));
			if (result.Action != DialogBoxAction.Ok)
				return;

			FileInfo info = new FileInfo(result.FileName);

			using (var writeStream = info.OpenWrite())
			{
				using (var writer = new StreamWriter(writeStream))
				{
					foreach(var path in base.Context.SelectedPaths)
					{
						FileProcessor.Process(path, "*.*",
							delegate(string file)
								{
									try
									{
										if (Directory.Exists(file))
											return;

										var dicomFile = new DicomFile(file);
										dicomFile.Load(DicomReadOptions.Default |
										               DicomReadOptions.DoNotStorePixelDataInDataSet);
										writer.WriteLine(dicomFile.Dump(String.Empty, DicomDumpOptions.None));
										writer.WriteLine();
									}
									catch (Exception e)
									{
										writer.WriteLine("Failed: {0}\n{1}", file, e);
									}
								}, true);
					}
				}
			}
		}
	}
}
