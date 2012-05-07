#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Services;

namespace ClearCanvas.ImageViewer.Configuration
{
    //TODO (Marmot):Move to IV.StudyManagement?

    /// <summary>
    /// Extension point for views onto <see cref="StorageConfigurationComponent"/>
    /// </summary>
    [ExtensionPoint]
	public sealed class StorageConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(StorageConfigurationComponentViewExtensionPoint))]
    public class StorageConfigurationComponent : ConfigurationApplicationComponent
    {
        private const string _notApplicable = "N/A";

        private DelayedEventPublisher _delaySetFileStoreDirectory;
        private StorageConfiguration _configuration;
        private string _fileStoreDriveName;
        private string _currentFileStoreDirectory;
        private string _originalFileStoreDirectory;

        private IWorkItemActivityMonitor _activityMonitor;

        public override void Start()
		{
            _delaySetFileStoreDirectory = new DelayedEventPublisher(RealSetFileStoreDirectory);
            _activityMonitor = WorkItemActivityMonitor.Create();
            _activityMonitor.IsConnectedChanged += ActivityMonitorOnIsConnectedChanged;

            _configuration = StudyStore.GetConfiguration();
            _currentFileStoreDirectory = _originalFileStoreDirectory = _configuration.FileStoreDirectory;
            
            UpdateFileStoreDriveName();
            MaximumUsedSpaceChanged();
            base.Start();
		}

        public override void Stop()
        {
            base.Stop();

            if (_delaySetFileStoreDirectory != null)
            {
                _delaySetFileStoreDirectory.Cancel();
                _delaySetFileStoreDirectory.Dispose();
                _delaySetFileStoreDirectory = null;
            }

            if (_activityMonitor != null)
            {
                _activityMonitor.IsConnectedChanged -= ActivityMonitorOnIsConnectedChanged;
                _activityMonitor.Dispose();
            }
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

        public override bool HasValidationErrors
        {
            get
            {
                _delaySetFileStoreDirectory.PublishNow();
                return base.HasValidationErrors;
            }
        }

        private bool IsDiskspaceAvailable
        {
            get { return _configuration.FileStoreDriveExists && _configuration.FileStoreDiskSpace.IsAvailable; }
        }

        #region Presentation Model

        public string FileStoreDirectory
        {
            get { return _currentFileStoreDirectory; }
            set
            {
                if (Equals(value, _currentFileStoreDirectory))
                    return;

                Modified = true;
                _currentFileStoreDirectory = value;
                _delaySetFileStoreDirectory.Publish(this, EventArgs.Empty);
            }
        }
        
        public bool HasFileStoreChanged
        {
            get { return _configuration.FileStoreDirectory != _originalFileStoreDirectory; }
        }

        public string FileStoreChangedMessage
        {
            get
            {
                if (!HasFileStoreChanged)
                    return String.Empty;

                return SR.MessageFileStoreChanged;
            }
        }

        public string FileStoreChangedDescription
        {
            get
            {
                if (!HasFileStoreChanged)
                    return String.Empty;

                return SR.DescriptionFileStoreChanged;
            }
        }

        public bool IsLocalServiceRunning
        {
            get { return _activityMonitor.IsConnected; }
        }

        public bool DoesLocalServiceHaveToStop
        {
            get { return IsLocalServiceRunning && HasFileStoreChanged; }
        }

        public string TotalSpaceBytesDisplay
        {
            get
            {
                if (!IsDiskspaceAvailable)
                    return _notApplicable;

                return Diskspace.FormatBytes(_configuration.FileStoreDiskSpace.TotalSpace, "F3");
            }
        }

        public double UsedSpacePercent
		{
            get
            {
                if (!IsDiskspaceAvailable)
                    return 0;

                return _configuration.FileStoreDiskSpace.UsedSpacePercent;
            }
		}

		public string UsedSpacePercentDisplay
		{
            get { return UsedSpacePercent.ToString("F3"); }
		}

		public string UsedSpaceBytesDisplay
		{
            get
            {
                if (!IsDiskspaceAvailable)
                    return _notApplicable;

                return Diskspace.FormatBytes(_configuration.FileStoreDiskSpace.UsedSpace, "F3");
            }
		}

		public double MaximumUsedSpacePercent
		{
			get
			{
                if (!IsDiskspaceAvailable)
                    return 0;

                return _configuration.MaximumUsedSpacePercent;
			}
			set
			{
                if (!IsDiskspaceAvailable)
                    return;

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
            get
            {
                if (!IsDiskspaceAvailable)
                    return _notApplicable;

                return Diskspace.FormatBytes(_configuration.MaximumUsedSpaceBytes, "F3");
            }
        }

        public bool IsMaximumUsedSpaceExceeded
        {
            get
            {
                if (!IsDiskspaceAvailable)
                    return false;

                return _configuration.IsMaximumUsedSpaceExceeded;
            }
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

        public string HelpMessage
        {
            get
            {
                return SR.MessageStorageHelp;
            }
        }

        public void ChangeFileStore()
        {
            var args = new SelectFolderDialogCreationArgs(FileStoreDirectory) { Prompt = SR.TitleSelectFileStore, AllowCreateNewFolder = true };
            var result = base.Host.DesktopWindow.ShowSelectFolderDialogBox(args);
            if (result.Action != DialogBoxAction.Ok)
                return;

            try
            {
                FileStoreDirectory = result.FileName;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, Host.DesktopWindow);
            }
        }

        public void StopLocalService()
        {
            try
            {
                BlockingOperation.Run(LocalServiceProcess.Stop);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Debug, e);
                Host.DesktopWindow.ShowMessageBox(SR.MessageUnableToStopLocalService, MessageBoxActions.Ok);
            }
        }

        public void Help()
        {
            Host.DesktopWindow.ShowMessageBox(SR.DescriptionStorageOptions, MessageBoxActions.Ok);
        }

        #endregion

        [ValidationMethodFor("FileStoreDirectory")]
        private ValidationResult ValidateFileStorePath()
        {
            if (!HasFileStoreChanged)
                return new ValidationResult(true, String.Empty);

            if (!_configuration.IsFileStoreDriveValid)
                return new ValidationResult(false, SR.ValidationDriveInvalid);

            if (!_configuration.FileStoreDriveExists)
                return new ValidationResult(false, String.Format(SR.ValidationDriveDoesNotExist, _configuration.FileStoreRootPath));

            if (!IsLocalServiceRunning)
                return new ValidationResult(true, String.Empty);

            return new ValidationResult(false, SR.ValidationMessageCannotChangeFileStore);
        }

        private void UpdateFileStoreDriveName()
        {
            string value;
            if (!_configuration.FileStoreDriveExists)
                value = null;
            else
                value = _configuration.FileStoreDriveName.ToUpper();

            if (Equals(value, _fileStoreDriveName))
                return;

            _fileStoreDriveName = value;

            UsedSpaceChanged();
            MaximumUsedSpaceChanged();
        }

        private void UsedSpaceChanged()
        {
            NotifyPropertyChanged("UsedSpacePercent");
            NotifyPropertyChanged("UsedSpacePercentDisplay");
            NotifyPropertyChanged("UsedSpaceBytesDisplay");
        }

        private void MaximumUsedSpaceChanged()
        {
            NotifyPropertyChanged("MaximumUsedSpace");
            NotifyPropertyChanged("MaximumUsedSpaceDisplay");
            NotifyPropertyChanged("IsMaximumUsedSpaceExceeded");
            NotifyPropertyChanged("MaximumUsedSpaceExceededMessage");
            NotifyPropertyChanged("MaximumUsedSpaceExceededDescription");
        }

        private void ActivityMonitorOnIsConnectedChanged(object sender, EventArgs eventArgs)
        {
            NotifyPropertyChanged("IsLocalServiceRunning");
            NotifyPropertyChanged("DoesLocalServiceHaveToStop");
        }

        private void RealSetFileStoreDirectory(object sender, EventArgs e)
        {
            if (Equals(_currentFileStoreDirectory, _configuration.FileStoreDirectory))
                return;

            _configuration.FileStoreDirectory = _currentFileStoreDirectory;

            UpdateFileStoreDriveName();

            NotifyPropertyChanged("FileStoreDirectory");
            NotifyPropertyChanged("HasFileStoreChanged");
            NotifyPropertyChanged("FileStoreChangedMessage");
            NotifyPropertyChanged("FileStoreChangedDescription");
            NotifyPropertyChanged("DoesLocalServiceHaveToStop");

            if (!HasValidationErrors)
                ShowValidation(false);
        }
    }
}
