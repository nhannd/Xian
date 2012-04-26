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
        private string _fileStoreDirectory;

		private string _driveName;
		private long _driveSize;
		private string _driveDisplay;

        private float _maximumUsedSpacePercent;
		private string _maximumUsedSpaceDisplay;
		
		private long _usedSpaceBytes;
		private float _usedSpacePercent;
		private string _usedSpacePercentDisplay;
		private string _usedSpaceBytesDisplay;

		private void MaximumUsedSpaceChanged()
		{
			_maximumUsedSpaceDisplay = GetSpaceDescription(_maximumUsedSpacePercent / 100F);

			NotifyPropertyChanged("MaximumUsedSpace");
			NotifyPropertyChanged("MaximumUsedSpaceDisplay");
		}

        public override void Start()
		{
            var configuration = StudyStore.GetConfiguration();
            var drive = configuration.FileStoreDrive;
            
            _fileStoreDirectory = configuration.FileStoreDirectory;
            _driveName = drive.Name;
            _driveSize = drive.TotalSize;
            _driveDisplay = String.Format("{0} ({1})", _driveName, GetSpaceDescription(1F));

            _maximumUsedSpacePercent = configuration.MaximumUsedSpacePercent;
            MaximumUsedSpaceChanged();

            _usedSpaceBytes = drive.TotalSize - drive.AvailableFreeSpace;
            _usedSpacePercent = _usedSpaceBytes / (float)_driveSize * 100F;
            _usedSpacePercentDisplay = _usedSpacePercent.ToString("F3");
            _usedSpaceBytesDisplay = GetSpaceDescription(_usedSpacePercent / 100F);
            
            base.Start();
		}

		public void Refresh()
		{
			NotifyAllPropertiesChanged();
		}

		public override void Save()
        {
            if (!Modified)
                return;

            var configuration = StudyStore.GetConfiguration();
            var drive = configuration.FileStoreDrive;
		    long minUsedDiskSpace = (long)(drive.TotalSize * (100 - _maximumUsedSpacePercent)/100.0);
            StudyStore.UpdateConfiguration(_fileStoreDirectory, minUsedDiskSpace);
        }

		private string GetSpaceDescription(float percentSpace)
		{
			double space = (double)percentSpace * DriveSize;
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

        #region Properties

        public string FileStoreDirectory
        {
            get { return _fileStoreDirectory; }
            set
            {
                if (Equals(value, _fileStoreDirectory))
                    return;

                _fileStoreDirectory = value;
                NotifyPropertyChanged("FileStoreDirectory");
            }
        }
        
        public string DriveName
        {
            get { return _driveName; }
        }

		public long DriveSize
		{
			get { return _driveSize; }
		}

		public string DriveDisplay
		{
			get { return _driveDisplay; }
		}

		public long UsedSpace
		{
			get { return _usedSpaceBytes; }
		}

		public float UsedSpacePercent
		{
            get { return _usedSpacePercent; }
		}

		public string UsedSpacePercentDisplay
		{
			get { return _usedSpacePercentDisplay; }
		}

		public string UsedSpaceBytesDisplay
		{
			get { return _usedSpaceBytesDisplay; }
		}

		public float MaximumUsedSpacePercent
		{
			get { return _maximumUsedSpacePercent; }
			set
			{
                if (Equals(value, _maximumUsedSpacePercent))
                    return;

			    value = Math.Min(value, 100);
			    value = Math.Max(value, 5);

			    _maximumUsedSpacePercent = value;
				this.Modified = true;
                NotifyPropertyChanged("MaximumUsedSpacePercent");
			}
        }

        public string MaximumUsedSpaceDisplay
        {
            get { return _maximumUsedSpaceDisplay; }
        }

        public void ChangeFileStore()
        {
            //TODO (Marmot): say stuff in here.
            var args = new SelectFolderDialogCreationArgs(_fileStoreDirectory) { Prompt = SR.TitleSelectFileStore, AllowCreateNewFolder = true};
            var result = base.Host.DesktopWindow.ShowSelectFolderDialogBox(args);
            if (result.Action != DialogBoxAction.Ok)
                return;

            _fileStoreDirectory = result.FileName;
            NotifyPropertyChanged("FileStoreDirectory");
        }

        #endregion
    }
}
