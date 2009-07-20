#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Export;
using Path=System.IO.Path;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	[DropDownAction("export", DefaultToolbarActionSite + "/ToolbarExport", "DropDownActionModel")]
	[IconSet("export", IconScheme.Colour, "Icons.SaveSmall.png", "Icons.SaveSmall.png", "Icons.SaveSmall.png")]
	[MenuAction("exportAnonymized", "studyfilters-exportdropdown/MenuExportAnonymized", "ExportAnonymized")]
	[MenuAction("exportAnonymized", DefaultContextMenuActionSite + "/MenuExportAnonymized", "ExportAnonymized")]
	[MenuAction("exportCopy", "studyfilters-exportdropdown/MenuExportCopy", "ExportCopy")]
	[MenuAction("exportCopy", DefaultContextMenuActionSite + "/MenuExportCopy", "ExportCopy")]
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class ExportTool : StudyFilterTool
	{
		public ActionModelNode DropDownActionModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "studyfilters-exportdropdown", this.Actions); }
		}

		public void ExportAnonymized()
		{
			try
			{
				if (base.Selection.Count > 0)
				{
					ExportComponent component = new ExportComponent();

					foreach (StudyItem item in base.Selection)
					{
						FileInfo file = item.File;
						if (file.Exists)
						{
							DicomFile dcf = new DicomFile(file.FullName);
							dcf.Load();
							component.Files.Add(dcf);
						}
					}

					base.DesktopWindow.ShowDialogBox(component, SR.Export);
				}
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, base.DesktopWindow);
			}
		}

		public void ExportCopy()
		{
			try
			{
				if (base.Selection.Count > 0)
				{
					SelectFolderDialogCreationArgs args = new SelectFolderDialogCreationArgs();
					args.Prompt = SR.MessageSelectOutputLocation;
					FileDialogResult result = base.DesktopWindow.ShowSelectFolderDialogBox(args);
					if (result.Action != DialogBoxAction.Ok)
						return;

					string outputDir = result.FileName;
					int count = 0;

					foreach (StudyItem item in base.Selection)
					{
						FileInfo file = item.File;
						if (file.Exists)
						{
							string newpath = Path.Combine(outputDir, file.Name);
							if (!File.Exists(newpath))
							{
								file.CopyTo(newpath);
								count++;
							}
						}
					}

					if (count > 0)
					{
						Process p = Process.Start(outputDir);
					}
				}
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, base.DesktopWindow);
			}
		}
	}
}