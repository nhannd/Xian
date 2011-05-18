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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.Explorer.Local;

namespace ClearCanvas.ImageViewer.TestTools
{
	[MenuAction("extractEmbeddedOverlays", "explorerlocal-contextmenu/Fix Pixel Data", "Go")]
	[ExtensionOf(typeof(LocalImageExplorerToolExtensionPoint))]
	public class FixPixelDataTool : Tool<ILocalImageExplorerToolContext>
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

					if (dicomFile.TransferSyntax.Encapsulated)
						continue;

					DicomAttribute attribute;
					if (!dicomFile.DataSet.TryGetAttribute(DicomTags.PixelData, out attribute))
						continue;

					new OverlayPlaneModuleIod(dicomFile.DataSet).ExtractEmbeddedOverlays();
					var rawPixelData = (byte[])attribute.Values;

					DicomPixelData pd = new DicomUncompressedPixelData(dicomFile);
					if (DicomUncompressedPixelData.ZeroUnusedBits(rawPixelData, pd.BitsAllocated, pd.BitsStored, pd.HighBit))
					{
						Platform.Log(LogLevel.Info, "Zeroed some unused bits.");
					}
					if (DicomUncompressedPixelData.RightAlign(rawPixelData, pd.BitsAllocated, pd.BitsStored, pd.HighBit))
					{
						var newHighBit = (ushort) (pd.HighBit - pd.LowBit);
						Platform.Log(LogLevel.Info, "Right aligned pixel data (High Bit: {0}->{1}).", pd.HighBit, newHighBit);

						pd.HighBit = newHighBit; //correct high bit after right-aligning.
						dicomFile.DataSet[DicomTags.HighBit].SetUInt16(0, newHighBit);
					}

					string sourceFileName = System.IO.Path.GetFileNameWithoutExtension(file);
					string fileName = System.IO.Path.Combine(result.FileName, sourceFileName);
					fileName += ".fixed-pixeldata.dcm";
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
