#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities;
using Path=System.IO.Path;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.CoreTools
{
	[DropDownAction("add", DefaultToolbarActionSite + "/ToolbarAddItems", "DropDownActionModel")]
	[IconSet("add", "Icons.AddItemsToolSmall.png", "Icons.AddItemsToolMedium.png", "Icons.AddItemsToolLarge.png")]
	//
	[MenuAction("addFiles", DropDownMenuActionSite + "/MenuAddFiles", "AddItems")]
	[MenuAction("addFolders", DropDownMenuActionSite + "/MenuAddFolders", "AddItemsByFolder")]
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