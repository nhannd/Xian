using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Services;
using System.IO;
using ClearCanvas.Dicom.OffisWrapper;


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

			using (DicomClient client = new DicomClient(me))
			{
				AEServer server = this.Context.SelectedServer;
				Uid studyUid = new Uid(this.Context.SelectedStudy.StudyInstanceUID);

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
				}

				client.SopInstanceReceived += new EventHandler<SopInstanceReceivedEventArgs>(OnSopInstanceReceived);
				client.Retrieve(server, studyUid, myAESettings.DicomStoragePath);
			}
		}

		void OnSopInstanceReceived(object sender, SopInstanceReceivedEventArgs e)
		{
			InsertSopInstance(e.SopFileName);
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

		private void InsertSopInstance(string fileName)
		{
			DcmFileFormat file = new DcmFileFormat();
			OFCondition condition = file.loadFile(fileName);
			if (!condition.good())
			{
				// there was an error reading the file, possibly it's not a DICOM file
				return;
			}

			DcmMetaInfo metaInfo = file.getMetaInfo();
			DcmDataset dataset = file.getDataset();

			if (ConfirmProcessableFile(metaInfo, dataset))
			{
				IDicomPersistentStore dicomStore = DataAccessLayer.GetIDicomPersistentStore(); 
				dicomStore.InsertSopInstance(metaInfo, dataset, fileName);
				dicomStore.Flush();
			}

			// keep the file object alive until the end of this scope block
			// otherwise, it'll be GC'd and metaInfo and dataset will be gone
			// as well, even though they are needed in the InsertSopInstance
			// and sub methods
			GC.KeepAlive(file);
		}

		private bool ConfirmProcessableFile(DcmMetaInfo metaInfo, DcmDataset dataset)
		{
			StringBuilder stringValue = new StringBuilder(1024);
			OFCondition cond;
			cond = metaInfo.findAndGetOFString(Dcm.MediaStorageSOPClassUID, stringValue);
			if (cond.good())
			{
				// we want to skip Media Storage Directory Storage (DICOMDIR directories)
				if ("1.2.840.10008.1.3.10" == stringValue.ToString())
					return false;
			}

			return true;
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
			if (this.Context.SelectedStudy == null)
				return;

			if (this.Context.SelectedServer.Host == "localhost")
				this.Enabled = false;
			else
				this.Enabled = true;

			SetDoubleClickHandler();
		}
	}
}
