#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.ImageViewer.Services.DiskspaceManager;

namespace ClearCanvas.ImageViewer.Services.Configuration
{
    /// <summary>
    /// Extension point for views onto <see cref="DiskspaceManagerConfigurationComponent"/>
    /// </summary>
    [ExtensionPoint]
	public sealed class DiskspaceManagerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DiskspaceManagerConfigurationComponent class
    /// </summary>
    [AssociateView(typeof(DiskspaceManagerConfigurationComponentViewExtensionPoint))]
    public class DiskspaceManagerConfigurationComponent : ConfigurationApplicationComponent
    {
		private static readonly int _minCheckFrequency = 1;
		private static readonly int _maxCheckFrequency = 10;
		private static readonly float _watermarkMinDifference = 5.0F;

		private string _driveName;
		private long _driveSize;
		private string _driveDisplay;

		private float _lowWatermark;
        private float _highWatermark;
		private string _lowWatermarkBytesDisplay;
		private string _highWatermarkBytesDisplay;
		
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

		public override void Stop()
		{
			base.Stop();
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
