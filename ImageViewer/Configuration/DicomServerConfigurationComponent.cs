#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.ImageViewer.Services.DicomServer;

namespace ClearCanvas.ImageViewer.Configuration
{
    /// <summary>
    /// Extension point for views onto <see cref="DicomServerConfigurationComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DicomServerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DicomServerConfigurationComponent class
    /// </summary>
    [AssociateView(typeof(DicomServerConfigurationComponentViewExtensionPoint))]
    public class DicomServerConfigurationComponent : ConfigurationApplicationComponent
    {
        private string _aeTitle;
        private string _port;
        private string _storageDir;
		private bool _enabled;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerConfigurationComponent()
        {
		}

        public override void Start()
        {
			Refresh();
			base.Start();

			this.Validation.Add(new AETitleValidatonRule());
			this.Validation.Add(new PortValidationRule());
        }

        public override void Stop()
        {
            base.Stop();
        }

        public void Refresh()
        {
			BlockingOperation.Run(this.ConnectToClientInternal);
			SignalPropertyChanged();
		}

		private void ConnectToClientInternal()
		{
			try
			{
				_enabled = true;
				DicomServerConfigurationHelper.Refresh(true);

				_aeTitle = DicomServerConfigurationHelper.AETitle;
				_port = DicomServerConfigurationHelper.Port.ToString();
				_storageDir = DicomServerConfigurationHelper.InterimStorageDirectory;
			}
			catch
			{
				_aeTitle = "";
				_port = "";
				_storageDir = "";
				_enabled = false;

				this.Host.DesktopWindow.ShowMessageBox(SR.MessageFailedToRetrieveServerSettings, MessageBoxActions.Ok);
			}
		}

        public override void Save()
        {
            try
            {
				// This should never throw an exception because we should have validated the port already.
				int port = Convert.ToInt32(_port);
				DicomServerConfigurationHelper.Update("localhost", _aeTitle, port, _storageDir);
            }
            catch
            {
				this.Host.DesktopWindow.ShowMessageBox(SR.MessageFailedToUpdateServerSettings, MessageBoxActions.Ok);
			}
        }

        private void SignalPropertyChanged()
        {
            NotifyPropertyChanged("AETitle");
            NotifyPropertyChanged("Port");
            NotifyPropertyChanged("Enabled");
        }

        #region Properties

        public string AETitle
        {
            get { return _aeTitle; }
            set 
            {
				if (value != null)
					_aeTitle = value.Trim();
				else
					_aeTitle = String.Empty;

                this.Modified = true;
            }
        }

        public string Port
        {
            get { return _port; }
            set 
            { 
                _port = value;
                this.Modified = true;
            }
        }

        public bool Enabled
        {
			get { return _enabled; }
        }

        #endregion

    }
}
