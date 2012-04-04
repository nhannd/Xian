#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
    internal class ServerTreeLocalServer : IServerTreeLocalServer
    {
        private string _path;
        private string _parentPath;

        private static readonly IDicomServerConfigurationProvider _dicomServerConfigurationProvider;

        static ServerTreeLocalServer()
        {
            _dicomServerConfigurationProvider = DicomServerConfigurationHelper.GetConfigurationProvider();
        }

        internal IDicomServerConfigurationProvider DicomServerConfigurationProvider
        {
            get { return _dicomServerConfigurationProvider; }
        }

        public string AETitle
        {
            get
            {
                RefreshConfiguration();
                return _dicomServerConfigurationProvider.ConfigurationExists ? _dicomServerConfigurationProvider.AETitle : null;
            }
        }

        public string HostName
        {
            get
            {
                RefreshConfiguration();
                return _dicomServerConfigurationProvider.ConfigurationExists ? _dicomServerConfigurationProvider.HostName : null;
            }
        }

        public int? Port
        {
            get
            {
                RefreshConfiguration();
                return _dicomServerConfigurationProvider.ConfigurationExists ? _dicomServerConfigurationProvider.Port : (int?)null;
            }
        }

        public string FileStoreLocation
        {
            get
            {
                RefreshConfiguration();
                return _dicomServerConfigurationProvider.ConfigurationExists ? _dicomServerConfigurationProvider.FileStoreDirectory : null;
            }
        }

        public event EventHandler ConfigurationChanged
        {
            add { _dicomServerConfigurationProvider.Changed += value; }
            remove { _dicomServerConfigurationProvider.Changed -= value; }
        }

        #region IServerTreeNode Members

        public bool IsChecked { get; set; }

        public bool IsLocalDataStore
        {
            get { return true; }
        }

        public bool IsServer
        {
            get { return false; }
        }

        public bool IsServerGroup
        {
            get { return false;	}
        }

        public bool IsRoot
        {
            get { return false; }
        }

        public string ParentPath
        {
            get { return _parentPath; }
        }

        public string Path
        {
            get { return _path; }
        }

        public string Name
        {
            get { return @"My Studies"; }
        }

        public string DisplayName
        {
            get { return SR.MyStudiesTitle; }
        }

        #endregion

        public override string ToString()
        {
            try
            {
                if (_dicomServerConfigurationProvider.NeedsRefresh)
                    _dicomServerConfigurationProvider.RefreshAsync();

                if (_dicomServerConfigurationProvider.ConfigurationExists)
                {
                    var formatString = DicomServer.IsRunning
                                           ? SR.FormatLocalServerDetails
                                           : SR.FormatLocalServerOfflineDetails;

                    return String.Format(formatString,
                                         DisplayName,
                                         _dicomServerConfigurationProvider.AETitle,
                                         _dicomServerConfigurationProvider.HostName,
                                         _dicomServerConfigurationProvider.Port,
                                         _dicomServerConfigurationProvider.FileStoreDirectory);
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
            }

            return String.Format(SR.FormatLocalServerConfigurationUnavailable, DisplayName);
        }
        
        private static void RefreshConfiguration()
        {
            try
            {
                if (_dicomServerConfigurationProvider.NeedsRefresh)
                    _dicomServerConfigurationProvider.RefreshAsync();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e, "Failed to retrieve the dicom server's ae title.");
            }
        }

        internal void ChangeParentPath(string parentPath)
        {
            _parentPath = parentPath ?? "";
            _path = _parentPath + Name;
        }
    }
}