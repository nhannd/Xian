using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Explorer.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.TestTools
{
	[MenuAction("activate", "dicomstudybrowser-contextmenu/StreamingAnalysis", "OpenAnalysisTool")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class StreamingAnalysisTool : StudyBrowserTool
	{
		private IShelf _shelf;

		public StreamingAnalysisTool()
		{
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			Enabled = true;
		}

		public override void Initialize()
		{
			base.Initialize();
			Enabled = true;
		}

		public void OpenAnalysisTool()
		{
			if (_shelf != null)
			{
				_shelf.Activate();
			}
			else
			{
				StreamingAnalysisComponent component = new StreamingAnalysisComponent(base.Context);
				_shelf = ApplicationComponent.LaunchAsShelf(base.Context.DesktopWindow, component, 
					"Streaming Analysis", ShelfDisplayHint.DockFloat | ShelfDisplayHint.ShowNearMouse);

				_shelf.Closed += delegate { _shelf = null; };
			}
		}
	}
}
