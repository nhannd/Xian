using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Services;
using System.IO;
using ClearCanvas.ImageViewer.StudyManagement;


namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ButtonAction("activate", "dicomstudybrowser-toolbar/Retrieve")]
	[MenuAction("activate", "dicomstudybrowser-contextmenu/Retrieve")]
	[ClickHandler("activate", "RetrieveStudy")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "Retrieve Study")]
	[IconSet("activate", IconScheme.Colour, "Icons.SendStudySmall.png", "Icons.SendStudySmall.png", "Icons.SendStudySmall.png")]
	[ExtensionOf(typeof(StudyBrowserToolExtensionPoint))]
	public class RetrieveStudyTool : StudyBrowserTool
	{
		public RetrieveStudyTool()
		{

		}

		public override void Initialize()
		{
			SetDoubleClickHandler();

			base.Initialize();
		}

		private void RetrieveStudy()
		{
            if (this.Context.SelectedStudy == null)
                return;

            LocalAESettings myAESettings = new LocalAESettings();
            ApplicationEntity me = new ApplicationEntity(new HostName("localhost"), new AETitle(myAESettings.AETitle), new ListeningPort(myAESettings.Port));

            // Try to create the storage directory if it doesn't already exist.
            // Ideally, this code should eventually be removed when the
            // directory is handled properly by the dicom.services layer.
            try
            {
                CreateStorageDirectory(myAESettings.DicomStoragePath);
            }
            catch
            {
                Platform.ShowMessageBox("Unable to create storage directory; cannot retrieve study");
                return;
            }


            foreach (StudyItem item in this.Context.SelectedStudies)
            {
                RetrieveStudyToolProgressComponent progressComponent = new RetrieveStudyToolProgressComponent(me,
                    this.Context.SelectedServer,
                    new Uid(item.StudyInstanceUID),
                    myAESettings.DicomStoragePath);

                ApplicationComponent.LaunchAsWorkspace(
                    this.Context.DesktopWindow,
                    progressComponent,
                    "Retrieve Progress",
                    delegate(IApplicationComponent component)
                    { Console.WriteLine("Done!"); }
                    );
            }
		}

		private void CreateStorageDirectory(string path)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}

		private void SetDoubleClickHandler()
		{
			if (this.Context.SelectedServer.Host != "localhost")
				this.Context.DefaultActionHandler = RetrieveStudy;
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			// If the results aren't from a remote machine, then we don't
			// even care whether a study has been selected or not
			if (this.Context.SelectedServer.Host == "localhost")
				return;

			base.OnSelectedStudyChanged(sender, e);
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			// If no study is selected then we don't even care whether
			// the last searched server has changed.

			if (this.Context.SelectedServer.Host == "localhost")
			{
				this.Enabled = false;
				return;
			}
			else
			{
				if (this.Context.SelectedStudy != null)
					this.Enabled = true;
				else
					this.Enabled = false;

				SetDoubleClickHandler();
			}
		}
	}
}
