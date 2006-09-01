using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("open", "dicomstudybrowser-toolbar/Open")]
	[ClickHandler("open", "OpenStudy")]
	//[EnabledStateObserver("edit", "Enabled", "EnabledChanged")]
	[Tooltip("open", "Open Study")]
	[IconSet("open", IconScheme.Colour, "Icons.OpenStudySmall.png", "Icons.OpenStudySmall.png", "Icons.OpenStudySmall.png")]
	[ClearCanvas.Common.ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class StudyBrowserTool : Tool
	{
		public StudyBrowserTool()
		{

		}

		public override void Initialize()
		{
			base.Initialize();
			this.Context.DefaultActionHandler = OpenStudy;
		}

		public void OpenStudy()
		{
			this.Context.StudyBrowserComponent.Open();
		}

		protected IStudyBrowserToolContext Context
		{
			get { return this.ContextBase as IStudyBrowserToolContext; }
		}

	}
}
