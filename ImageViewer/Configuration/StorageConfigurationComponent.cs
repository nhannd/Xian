#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Services;

namespace ClearCanvas.ImageViewer.Configuration
{

    /// <summary>
    /// Extension point for views onto <see cref="StorageConfigurationComponent"/>
    /// </summary>
    [ExtensionPoint]
	public sealed class StorageConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

	//NOTE: this may not be the best place for this, but it doesn't make sense to have any of these tools without
	// the configuration components (or vice versa) anyway.

    /// <summary>
    /// DiskspaceManagerConfigurationComponent class
    /// </summary>
    [AssociateView(typeof(StorageConfigurationComponentViewExtensionPoint))]
    public class StorageConfigurationComponent : ConfigurationApplicationComponent
    {
        private StorageConfiguration _configuration;
        private string _originalFileStoreDirectory;

        private string _maximumUsedSpaceBytesDisplay;
        private string _usedSpacePercentDisplay;
		private string _usedSpaceBytesDisplay;

        public override void Start()
		{
            _configuration = StudyStore.GetConfiguration();
            _originalFileStoreDirectory = _configuration.FileStoreDirectory;

            MaximumUsedSpaceChanged();

            _usedSpacePercentDisplay = UsedSpacePercent.ToString("F3");
            _usedSpaceBytesDisplay = Diskspace.FormatBytes(_configuration.FileStoreDiskSpace.UsedSpace, "F3");
            
            base.Start();
		}

		public override void Save()
        {
		    try
		    {
                StudyStore.UpdateConfiguration(_configuration);
		    }
		    catch (Exception e)
		    {
                ExceptionHandler.Report(e, Host.DesktopWindow);
		    }
        }

        #region Presentation Model

        public string FileStoreDirectory
        {
            get { return _configuration.FileStoreDirectory; }
            private set
            {
                if (Equals(value, _configuration.FileStoreDirectory))
                    return;

                _configuration.FileStoreDirectory = value;

                NotifyPropertyChanged("FileStoreDirectory");
                NotifyPropertyChanged("HasFileStoreChanged");
            }
        }

        public bool HasFileStoreChanged
        {
            get { return _configuration.FileStoreDirectory != _originalFileStoreDirectory; }
        }

        public string TotalSpaceBytesDisplay
        {
            get { return Diskspace.FormatBytes(_configuration.FileStoreDiskSpace.TotalSpace, "F3"); }
        }

        public double UsedSpacePercent
		{
            get { return _configuration.FileStoreDiskSpace.UsedSpacePercent; }
		}

		public string UsedSpacePercentDisplay
		{
			get { return _usedSpacePercentDisplay; }
		}

		public string UsedSpaceBytesDisplay
		{
			get { return _usedSpaceBytesDisplay; }
		}

		public double MaximumUsedSpacePercent
		{
			get { return _configuration.MaximumUsedSpacePercent; }
			set
			{
                if (Equals(value, _configuration.MaximumUsedSpacePercent))
                    return;

			    value = Math.Min(value, 100);
			    value = Math.Max(value, 10);

			    _configuration.MaximumUsedSpacePercent = value;
				this.Modified = true;
                MaximumUsedSpaceChanged();
            }
        }

        public string MaximumUsedSpaceDisplay
        {
            get { return _maximumUsedSpaceBytesDisplay; }
        }

        public bool IsMaximumUsedSpaceExceeded
        {
            get { return _configuration.IsMaximumUsedSpaceExceeded; }
        }

        public string MaximumUsedSpaceExceededMessage
        {
            get
            {
                if (!IsMaximumUsedSpaceExceeded)
                    return String.Empty;

                return StudyManagement.SR.MessageMaximumDiskUsageExceeded;
            }
        }

        public string MaximumUsedSpaceExceededDescription
        {
            get
            {
                if (!IsMaximumUsedSpaceExceeded)
                    return String.Empty;

                return StudyManagement.SR.DescriptionMaximumDiskUsageExceeded;
            }
        }

        public void ChangeFileStore()
        {
            if (WorkItemActivityMonitor.IsRunning)
            {
                if (DialogBoxAction.No == Host.DesktopWindow.ShowMessageBox(SR.QuestionCannotChangeFileStore, MessageBoxActions.YesNo))
                    return;

                try
                {
                    LocalServiceProcess.Stop();
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.MessageUnableToStopLocalService, Host.DesktopWindow);
                    return;
                }
            }

            var args = new SelectFolderDialogCreationArgs(FileStoreDirectory) { Prompt = SR.TitleSelectFileStore, AllowCreateNewFolder = true };
            var result = base.Host.DesktopWindow.ShowSelectFolderDialogBox(args);
            if (result.Action != DialogBoxAction.Ok)
                return;

            try
            {
                FileStoreDirectory = result.FileName;
                Host.DesktopWindow.ShowMessageBox(SR.MessageMoveFileStore, MessageBoxActions.Ok);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, Host.DesktopWindow);
            }
        }

        #endregion

        private void MaximumUsedSpaceChanged()
        {
            _maximumUsedSpaceBytesDisplay = Diskspace.FormatBytes(_configuration.MaximumUsedSpaceBytes, "F3");

            NotifyPropertyChanged("MaximumUsedSpace");
            NotifyPropertyChanged("MaximumUsedSpaceDisplay");
            NotifyPropertyChanged("IsMaximumUsedSpaceExceeded");
            NotifyPropertyChanged("MaximumUsedSpaceExceededLabel");
            NotifyPropertyChanged("MaximumUsedSpaceExceededMessage");
        }
    }
}
