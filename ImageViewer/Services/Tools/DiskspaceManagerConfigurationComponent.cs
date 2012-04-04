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
using ClearCanvas.ImageViewer.Common.DiskspaceManager;

namespace ClearCanvas.ImageViewer.Services.Tools
{
    /// <summary>
    /// Extension point for views onto <see cref="DiskspaceManagerConfigurationComponent"/>
    /// </summary>
    [ExtensionPoint]
	public sealed class DiskspaceManagerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

	//NOTE: this may not be the best place for this, but it doesn't make sense to have any of these tools without
	// the configuration components (or vice versa) anyway.

    /// <summary>
    /// DiskspaceManagerConfigurationComponent class
    /// </summary>
    [AssociateView(typeof(DiskspaceManagerConfigurationComponentViewExtensionPoint))]
    public class DiskspaceManagerConfigurationComponent : ConfigurationApplicationComponent
    {
		private static readonly int _minCheckFrequency = 1;
		private static readonly int _maxCheckFrequency = 10;
    	private static readonly long _watermarkMinDifferenceBytes = 5*1024*1024;

		private string _driveName;
		private long _driveSize;
		private string _driveDisplay;

		private float _lowWatermark;
        private float _highWatermark;
		private string _lowWatermarkBytesDisplay;
		private string _highWatermarkBytesDisplay;
		private float _watermarkMinDifference;
		
		private long _spaceUsed;
		private float _spaceUsedPercent;
		private string _spaceUsedPercentDisplay;
		private string _spaceUsedBytesDisplay;
        
		private int _checkFrequency;

    	private string _studyCountText;
		private bool _enforceStudyLimit;
    	private int _studyLimit;
		private int _minStudyLimit;
		private int _maxStudyLimit;

		private bool _enabled;

        /// <summary>
        /// Constructor
        /// </summary>
        public DiskspaceManagerConfigurationComponent()
        {
        }

		private void LowWatermarkChanged()
		{
			_lowWatermarkBytesDisplay = GetSpaceDescription(_lowWatermark / 100F);
			
			NotifyPropertyChanged("LowWatermark");
			NotifyPropertyChanged("LowatermarkBytesDisplay");
		}

		private void HighWatermarkChanged()
		{
			_highWatermarkBytesDisplay = GetSpaceDescription(_highWatermark / 100F);

			NotifyPropertyChanged("HighWatermark");
			NotifyPropertyChanged("HighatermarkBytesDisplay");
		}

		private void ConnectToClientInternal()
		{
			DiskspaceManagerServiceClient serviceClient = new DiskspaceManagerServiceClient();

			try
			{
				serviceClient.Open();
				DiskspaceManagerServiceInformation serviceInformation = serviceClient.GetServiceInformation();
				serviceClient.Close();

				_driveName = serviceInformation.DriveName;
				_driveSize = serviceInformation.DriveSize;
				_driveDisplay = String.Format("{0} ({1})", _driveName, GetSpaceDescription(1F));

				_watermarkMinDifference = (float)_watermarkMinDifferenceBytes/_driveSize * 100F;

				_lowWatermark = serviceInformation.LowWatermark;
				LowWatermarkChanged();
				
				_highWatermark = serviceInformation.HighWatermark;
				HighWatermarkChanged();

				_spaceUsed = serviceInformation.UsedSpace;
				_spaceUsedPercent = _spaceUsed / (float)_driveSize * 100F;
				_spaceUsedPercentDisplay = _spaceUsedPercent.ToString("F3");
				_spaceUsedBytesDisplay = GetSpaceDescription(_spaceUsedPercent / 100F);
				
				_checkFrequency = serviceInformation.CheckFrequency;

				_studyCountText = serviceInformation.StudyCount.ToString();

				_enforceStudyLimit = serviceInformation.EnforceStudyLimit;
				_studyLimit = serviceInformation.StudyLimit;
				_minStudyLimit = serviceInformation.MinStudyLimit;
				_maxStudyLimit = serviceInformation.MaxStudyLimit;
				this.Enabled = true;
			}
			catch
			{
				serviceClient.Abort();

				_driveName = "";
				_driveSize = 0;
				_driveDisplay = "";
				
				_lowWatermark = 0.0F;
				_lowWatermarkBytesDisplay = "";
				
				_highWatermark = 0.0F;
				_highWatermarkBytesDisplay = "";
				
				_spaceUsed = 0;
				_spaceUsedPercent = 0F;
				_spaceUsedPercentDisplay = "";
				_spaceUsedBytesDisplay = "";

				_studyCountText = SR.MessageStudyCountUnavailable;
				_enforceStudyLimit = false;
				_studyLimit = 0;
				_minStudyLimit = 0;
				_maxStudyLimit = 0;
				_checkFrequency = 10;

				this.Enabled = false; 
				
				this.Host.DesktopWindow.ShowMessageBox(SR.MessageFailedToRetrieveDiskspaceManagementSettings, MessageBoxActions.Ok);
			}
		}

		public override void Start()
		{
			Refresh();
			base.Start();
		}

		public void Refresh()
		{
			BlockingOperation.Run(this.ConnectToClientInternal);
			NotifyAllPropertiesChanged();
		}
		
		public override void Save()
        {
			DiskspaceManagerServiceClient serviceClient = new DiskspaceManagerServiceClient();

			try
			{
				serviceClient.Open();
				DiskspaceManagerServiceConfiguration newConfiguration = new DiskspaceManagerServiceConfiguration();
                newConfiguration.LowWatermark = _lowWatermark;
                newConfiguration.HighWatermark = _highWatermark;
                newConfiguration.CheckFrequency = _checkFrequency;
				newConfiguration.EnforceStudyLimit = _enforceStudyLimit;
				newConfiguration.StudyLimit = _studyLimit;
				serviceClient.UpdateServiceConfiguration(newConfiguration);
                serviceClient.Close();
            }
            catch
            {
				serviceClient.Abort();
				this.Host.DesktopWindow.ShowMessageBox(SR.MessageFailedToUpdateDiskspaceManagementSettings, MessageBoxActions.Ok);
            }
        }

		private string GetSpaceDescription(float percentSpace)
		{
			double space = (double)percentSpace * this.DriveSize;
			if (space <= 0)
				return "";

			int i = 0;
			while (space > 1024)
			{
				space /= 1024;
				if (++i == 4)
					break;
			}

			StringBuilder builder = new StringBuilder(space.ToString("F3"));
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

		public bool Enabled
		{
			get { return _enabled; }
			private set
			{
				_enabled = value;
				NotifyPropertyChanged("Enabled");
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

		public long SpaceUsed
		{
			get { return _spaceUsed; }
		}

		public float SpaceUsedPercent
		{
			get { return _spaceUsedPercent; }
		}

		public string SpaceUsedPercentDisplay
		{
			get { return _spaceUsedPercentDisplay; }
		}

		public string SpaceUsedBytesDisplay
		{
			get { return _spaceUsedBytesDisplay; }
		}

		public int MinimumCheckFrequency
		{
			get { return _minCheckFrequency; }
		}

		public int MaximumCheckFrequency
		{
			get { return _maxCheckFrequency; }
		}
		
		public int CheckFrequency
		{
			get { return _checkFrequency; }
			set
			{
				int checkFrequency = Math.Max(value, _minCheckFrequency);
				checkFrequency = Math.Min(value, _maxCheckFrequency);

				if (_checkFrequency != checkFrequency)
				{
					_checkFrequency = checkFrequency;
					this.Modified = true;

					NotifyPropertyChanged("CheckFrequency");
				}
			}
		}

		public float WatermarkMinDifference
		{
			get { return _watermarkMinDifference; }
		}

		public string LowWaterMarkBytesDisplay
		{
			get { return _lowWatermarkBytesDisplay; }
		}
		
		public string HighWaterMarkBytesDisplay
		{
			get { return _highWatermarkBytesDisplay; }
		}

		public float LowWatermark
		{
			get { return _lowWatermark; }
			set
			{
				if (value >= (100.0F - _watermarkMinDifference))
				{
					_lowWatermark = 100.0F - _watermarkMinDifference;
				}
				else if (value <= 0.0F)
				{
					_lowWatermark = 0.0F;
				}
				else
					_lowWatermark = value;

				LowWatermarkChanged();

				if (_highWatermark <= (_lowWatermark + _watermarkMinDifference))
				{
					_highWatermark = _lowWatermark + _watermarkMinDifference;
					HighWatermarkChanged();
				}

				this.Modified = true;
			}
		}

		public float HighWatermark
		{
			get { return _highWatermark; }
			set
			{
				if (value >= 100.0F)
				{
					_highWatermark = 100.0F;
				}
				else if (value <= _watermarkMinDifference)
				{
					_highWatermark = _watermarkMinDifference;
				}
				else
					_highWatermark = value;

				HighWatermarkChanged();

				if (_highWatermark <= (_lowWatermark + _watermarkMinDifference))
				{
					_lowWatermark = _highWatermark - _watermarkMinDifference;
					LowWatermarkChanged();
				}

				this.Modified = true;
			}
		}

		public string StudyCountText
		{
			get { return _studyCountText; }
		}

		public int MinStudyLimit
		{
			get { return _minStudyLimit; }
		}

		public int MaxStudyLimit
		{
			get { return _maxStudyLimit; }
		}
		
		public bool EnforceStudyLimit
    	{
			get { return _enforceStudyLimit; }
			set
			{
				if (_enforceStudyLimit != value)
				{
					_enforceStudyLimit = value;
					NotifyPropertyChanged("EnforceStudyLimit");
					this.Modified = true;
				}
			}
    	}

		public int StudyLimit
		{
			get { return _studyLimit; }
			set
			{
				if (_studyLimit != value)
				{
					_studyLimit = value;
					NotifyPropertyChanged("StudyLimit");
					this.Modified = true;
				}
			}
		}

		#endregion
    }
}
