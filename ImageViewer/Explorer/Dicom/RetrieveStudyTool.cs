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
            //
            // check pre-conditions
            //
            if (this.Context.SelectedStudy == null)
                return;

            if (this.IsRetrieveActive)
            {
				Platform.ShowMessageBox(SR.MessageCannotStartMultipleRetrievalSessions, MessageBoxActions.Ok);
                return;
            }

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
				Platform.ShowMessageBox(SR.MessageUnableToCreateStorageDirectory);
                return;
            }

            // create an instance of the retrieval component if it doesn't already exist
            if (null == this.RetrieveProgressComponent)
                this.RetrieveProgressComponent = new RetrieveStudyToolProgressComponent(me, myAESettings.DicomStoragePath);

            // open the retrieval component shelf if it's currently closed
            // delegate is used to reset the state of ProgressComponentShelfClosed
            if (this.ProgressComponentShelfClosed)
            {
                #region Launch the retrieval component as a shelf
                ApplicationComponent.LaunchAsShelf(
                    this.Context.DesktopWindow,
                    this.RetrieveProgressComponent,
					SR.TitleRetrieveProgressComponent,
                    ShelfDisplayHint.DockRight,
                    delegate(IApplicationComponent component)
                    {
                        this.ProgressComponentShelfClosed = true;
                    }
                    );
                #endregion

                this.ProgressComponentShelfClosed = false;
            }

            this.IsRetrieveActive = true;
            this.RetrieveProgressComponent.AllRetrievalTasksCompleted += 
                delegate(object source, EventArgs args)
                {
                    this.IsRetrieveActive = false;
                };


            this.RetrieveProgressComponent.Retrieve(this.Context.SelectedStudies);           
		}

		private void CreateStorageDirectory(string path)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}

		private void SetDoubleClickHandler()
		{
			if (!this.Context.SelectedServerGroup.IsLocalDatastore)
				this.Context.DefaultActionHandler = RetrieveStudy;
		}

		protected override void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			// If the results aren't from a remote machine, then we don't
			// even care whether a study has been selected or not
			if (this.Context.SelectedServerGroup.IsLocalDatastore)
				return;

			base.OnSelectedStudyChanged(sender, e);
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			// If no study is selected then we don't even care whether
			// the last searched server has changed.

			if (this.Context.SelectedServerGroup.IsLocalDatastore)
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
        #region Properties
        private RetrieveStudyToolProgressComponent _retrieveProgressComponent;
        private bool _progressComponentShelfClosed = true;
        private bool _isRetrieveActive;

        /// <summary>
        /// Indicates whether or not there is currently an active retrieve operation
        /// taking place.
        /// </summary>
        public bool IsRetrieveActive
        {
            get { return _isRetrieveActive; }
            set { _isRetrieveActive = value; }
        }
	
        /// <summary>
        /// Used to keep track of whether the shelf component that 
        /// displays the retrieval progress has been closed. This 
        /// allows us to reopen the shelf, whenever a new retrieve
        /// operation is initiated.
        /// </summary>
        private bool ProgressComponentShelfClosed
        {
            get { return _progressComponentShelfClosed; }
            set { _progressComponentShelfClosed = value; }
        }
	
        private RetrieveStudyToolProgressComponent RetrieveProgressComponent
        {
            get { return _retrieveProgressComponent; }
            set { _retrieveProgressComponent = value; }
        }
	
        #endregion
    }
}
