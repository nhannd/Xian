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

using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	[ButtonAction("export", DefaultToolbarActionSite + "/ToolbarAnonymizeExport", "Export")]
	[ButtonAction("saveto", DefaultToolbarActionSite + "/ToolbarSaveTo", "SaveTo")]
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class ExportTool : StudyFilterTool
	{
		public void Export()
		{
			// still no folder picker ><
			if (base.Selection.Count > 0)
			{
				string path = null;

				foreach (StudyItem item in base.Selection)
				{
					path = path ?? item.File.Directory.FullName;
				}
			}
		}

		public void SaveTo()
		{
			// no folder picker ><
			if (base.Selection.Count > 0)
			{
				string outputDir = FolderPickerExtensionPoint.GetFolder();
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
							count ++;
						}
					}
				}

				if (count > 0)
				{
					Process p = Process.Start(outputDir);
				}
			}
		}
	}
}