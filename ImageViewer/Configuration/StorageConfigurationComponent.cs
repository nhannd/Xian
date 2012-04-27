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
		private string _maximumUsedSpaceDisplay;
        private string _usedSpacePercentDisplay;
		private string _usedSpaceBytesDisplay;

        public override void Start()
		{
            _configuration = StudyStore.GetConfiguration();
            MaximumUsedSpaceChanged();

            _usedSpacePercentDisplay = UsedSpacePercent.ToString("F3");
            _usedSpaceBytesDisplay = GetSpaceDescription(UsedSpacePercent / 100F);
            
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
            }
        }
        
		public long UsedSpace
		{
			get { return _configuration.FileStoreDrive.TotalUsedSpace; }
		}

		public double UsedSpacePercent
		{
            get { return _configuration.FileStoreDrive.TotalUsedSpacePercent; }
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
            get { return _maximumUsedSpaceDisplay; }
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

                return SR.MaximumUsedSpaceExceededMessage;
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
            _maximumUsedSpaceDisplay = GetSpaceDescription(MaximumUsedSpacePercent / 100F);

            NotifyPropertyChanged("MaximumUsedSpace");
            NotifyPropertyChanged("MaximumUsedSpaceDisplay");
            NotifyPropertyChanged("IsMaximumUsedSpaceExceeded");
            NotifyPropertyChanged("MaximumUsedSpaceExceededMessage");
        }
        
        private string GetSpaceDescription(double percentSpace)
        {
            double space = percentSpace * _configuration.FileStoreDrive.TotalSize;
            if (space <= 0)
                return "";

            int i = 0;
            while (space > 1024)
            {
                space /= 1024;
                if (++i == 4)
                    break;
            }

            var builder = new StringBuilder(space.ToString("F3"));
            switch (i)
            { 
                case 4:
                    builder.AppendFormat(" {0}", SR.LabelTerabytes);
                    break;
                case 3:
                    builder.AppendFormat(" {0}", SR.LabelGigabytes);
                    break;
                case 2:
                    builder.AppendFormat(" {0}", SR.LabelMegabytes);
                    break;
                case 1:
                    builder.AppendFormat(" {0}", SR.LabelKilobytes);
                    break;
                default: //0
                    builder.AppendFormat(" {0}", SR.LabelBytes);
                    break;
            }

            return builder.ToString();
        }
    }
}
