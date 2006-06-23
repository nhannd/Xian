using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Workstation.Model;
using ClearCanvas.ImageViewer.Tools;
using ClearCanvas.ImageViewer.Actions;
using ClearCanvas.ImageViewer.StudyManagement;

using Gtk;

namespace ClearCanvas.ImageViewer.View.GTK
{
	[MenuAction("activate", "MenuFile/MenuFileSearch")]
	[ButtonAction("activate", "ToolbarStandard/ToolbarToolsStandardStudyCentre")]
	[ClickHandler("activate", "Activate")]
	[ClearCanvas.Workstation.Model.Actions.IconSet("activate", IconScheme.Colour, "", "Icons.DashboardMedium.png", "Icons.DashboardLarge.png")]
	
	[ExtensionOf(typeof(ClearCanvas.Workstation.Model.WorkstationToolExtensionPoint))]
	public class StudyCentreTool : StockTool
	{
		
		public StudyCentreTool()
		{
		}

		public void Activate()
		{
			object[] buttonResponses = new object[] {"Accept", ResponseType.Accept, "Cancel", ResponseType.Cancel};
			FileChooserDialog fileDialog = new FileChooserDialog("Local Studies", (Window)_mainView.GuiElement, FileChooserAction.SelectFolder, buttonResponses);
			
			int result = fileDialog.Run();
			string folder = fileDialog.Filename;
			fileDialog.Destroy();	// must manually destroy the dialog
			
			if(result == (int)ResponseType.Accept)
			{
				LocalImageLoader loader = new LocalImageLoader();
				string studyUID = loader.Load(folder);
				if(studyUID == "" || WorkstationModel.StudyManager.StudyTree.GetStudy(studyUID) == null)
				{
					Platform.ShowMessageBox(ClearCanvas.Workstation.Model.SR.ErrorUnableToLoadStudy);
				}
				else
				{
					ImageWorkspace ws = new ImageWorkspace(studyUID);
					WorkstationModel.WorkspaceManager.Workspaces.Add(ws);
				}
			}
		}
	}
}
