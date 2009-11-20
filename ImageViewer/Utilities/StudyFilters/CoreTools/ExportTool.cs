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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Export;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.CoreTools
{
	[DropDownAction("export", DefaultToolbarActionSite + "/ToolbarExport", "DropDownActionModel")]
	[IconSet("export", IconScheme.Colour, "Icons.SaveToolSmall.png", "Icons.SaveToolMedium.png", "Icons.SaveToolLarge.png")]
	[EnabledStateObserver("export", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]

	[MenuAction("exportAnonymized", DropDownMenuActionSite + "/MenuExportAnonymized", "ExportAnonymized")]
	[MenuAction("exportAnonymized", DefaultContextMenuActionSite + "/MenuExportAnonymized", "ExportAnonymized")]
	[EnabledStateObserver("exportAnonymized", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	
	[MenuAction("exportCopy", DropDownMenuActionSite + "/MenuExportCopy", "ExportCopy")]
	[MenuAction("exportCopy", DefaultContextMenuActionSite + "/MenuExportCopy", "ExportCopy")]
	[EnabledStateObserver("exportCopy", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class ExportTool : StudyFilterTool
	{
		public const string DropDownMenuActionSite = "studyfilters-exportdropdown";

		private string _lastExportCopyFolder = string.Empty;
		private string _lastExportAnonymizedFolder = string.Empty;

		public ActionModelNode DropDownActionModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, DropDownMenuActionSite, this.Actions); }
		}

		public void ExportAnonymized()
		{
			if (base.SelectedItems.Count == 0)
				return;

			try
			{
				List<FileInfo> files = CollectionUtils.Map(base.SelectedItems, (StudyItem item) => item.File);
				DicomFileExporter exporter = new DicomFileExporter(files)
				                        	{
				                        		OutputPath = _lastExportAnonymizedFolder, 
												Anonymize = true
				                        	};
				
				bool success = exporter.Export();
				_lastExportAnonymizedFolder = exporter.OutputPath;
				
				if (success)
					Process.Start(_lastExportAnonymizedFolder);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, base.DesktopWindow);
			}
		}

		public void ExportCopy()
		{
			if (base.SelectedItems.Count == 0)
				return;

			try
			{
				List<FileInfo> files = CollectionUtils.Map(base.SelectedItems, (StudyItem item) => item.File);
				DicomFileExporter exporter = new DicomFileExporter(files) { OutputPath = _lastExportCopyFolder };

				bool success = exporter.Export();
				_lastExportCopyFolder = exporter.OutputPath;
				if (success)
					Process.Start(_lastExportCopyFolder);
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, base.DesktopWindow);
			}
		}
	}
}