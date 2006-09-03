using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("delete", "dicomstudybrowser-toolbar/Delete")]
	[MenuAction("delete", "dicomstudybrowser-contextmenu/Delete")]
	[ClickHandler("delete", "DeleteStudy")]
	[EnabledStateObserver("delete", "Enabled", "EnabledChanged")]
	[Tooltip("delete", "Delete Study")]
	[IconSet("delete", IconScheme.Colour, "Icons.DeleteStudySmall.png", "Icons.DeleteStudySmall.png", "Icons.DeleteStudySmall.png")]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class DeleteStudyTool : StudyBrowserTool
	{
		public DeleteStudyTool()
		{

		}

		public void DeleteStudy()
		{
			this.Context.StudyBrowserComponent.Delete();
		}

	}
}
