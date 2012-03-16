#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Services.Tools;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	[ButtonAction("import", DefaultToolbarActionSite + "/ToolbarImportToLocalDataStore", "Import")]
	[MenuAction("import", DefaultContextMenuActionSite + "/MenuImportToLocalDataStore", "Import")]
	[EnabledStateObserver("import", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[Tooltip("import", "TooltipImportToLocalDataStore")]
	[IconSet("import", "Icons.DataStoreImportToolSmall.png", "Icons.DataStoreImportToolMedium.png", "Icons.DataStoreImportToolLarge.png")]
	[ViewerActionPermission("import", Common.AuthorityTokens.Study.Import)]
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class DataStoreImportTool : LocalExplorerStudyFilterToolProxy<DicomFileImportTool>
	{
		public void Import()
		{
			base.BaseTool.Import();
		}
	}
}