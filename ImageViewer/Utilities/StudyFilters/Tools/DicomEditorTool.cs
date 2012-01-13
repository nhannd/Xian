#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Utilities.DicomEditor;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	[ButtonAction("activate", DefaultToolbarActionSite + "/ToolbarDumpFiles", "Dump")]
	[MenuAction("activate", DefaultContextMenuActionSite + "/MenuDumpFiles", "Dump")]
	[EnabledStateObserver("activate", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[Tooltip("activate", "TooltipDumpFiles")]
	[IconSet("activate", "Icons.DicomEditorToolSmall.png", "Icons.DicomEditorToolMedium.png", "Icons.DicomEditorToolLarge.png")]
	[ViewerActionPermission("activate", ClearCanvas.Utilities.DicomEditor.AuthorityTokens.DicomEditor)]
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class DicomEditorTool : LocalExplorerStudyFilterToolProxy<ShowDicomEditorTool>
	{
		private static MethodInfo _dumpMethod;

		private static MethodInfo DumpMethod
		{
			get
			{
				if (_dumpMethod == null)
					_dumpMethod = typeof (ShowDicomEditorTool).GetMethod("Dump", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				return _dumpMethod;
			}
		}

		public void Dump()
		{
			MethodInfo methodInfo = DumpMethod;
			if (methodInfo != null)
				methodInfo.Invoke(base.BaseTool, null);
		}
	}
}