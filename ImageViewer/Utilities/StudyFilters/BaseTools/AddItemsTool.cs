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

				StudyFilterComponentLoadHelper.Load(base.Component, base.DesktopWindow, false, paths);
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

				base.Component.BulkOperationsMode = true;
				try
				{
					StudyFilterComponentLoadHelper.Load(base.Component, base.DesktopWindow, false, result.FileName);
					base.Component.Refresh();
				}
				finally
				{
					base.Component.BulkOperationsMode = false;
				}
			}
		}
	}
}