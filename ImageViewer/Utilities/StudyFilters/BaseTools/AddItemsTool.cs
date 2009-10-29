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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;
using Path=System.IO.Path;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.BaseTools
{
	[DropDownAction("add", DefaultToolbarActionSite + "/ToolbarAddItems", "DropDownActionModel")]
	[IconSet("add", IconScheme.Colour, "Icons.AddItemsToolSmall.png", "Icons.AddItemsToolMedium.png", "Icons.AddItemsToolLarge.png")]
	//
	[ButtonAction("addFiles", DropDownMenuActionSite + "/MenuAddFiles", "AddItems")]
	[ButtonAction("addFolders", DropDownMenuActionSite + "/MenuAddFolders", "AddItemsByFolder")]
	//
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class AddItemsTool : StudyFilterTool
	{
		public const string DropDownMenuActionSite = "studyfilters-adddropdown";

		private string _lastFolder = string.Empty;

		public ActionModelNode DropDownActionModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, DropDownMenuActionSite, this.Actions); }
		}

		public void AddItems()
		{
			FileDialogCreationArgs args = new FileDialogCreationArgs(string.Empty);
			args.Filters.Add(new FileExtensionFilter("*.*", SR.LabelAllFiles));
			args.Directory = _lastFolder;

			IEnumerable<string> paths = ExtendedOpenFilesDialog.GetFiles(args);
			if (paths != null)
			{
				foreach (string s in paths)
				{
					_lastFolder = Path.GetDirectoryName(s);
					break;
				}

				base.Load(false, paths);
			}
		}

		public void AddItemsByFolder()
		{
			SelectFolderDialogCreationArgs args = new SelectFolderDialogCreationArgs();
			args.AllowCreateNewFolder = false;
			args.Path = _lastFolder;
			args.Prompt = SR.MessageSelectAddFilesFolder;

			FileDialogResult result = base.Context.DesktopWindow.ShowSelectFolderDialogBox(args);
			if (result.Action == DialogBoxAction.Ok)
			{
				_lastFolder = result.FileName;

				base.Context.BulkOperationsMode = true;
				try
				{
					base.Load(false, result.FileName);
					base.RefreshTable();
				}
				finally
				{
					base.Context.BulkOperationsMode = false;
				}
			}
		}
	}
}