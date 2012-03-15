using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.Common.ServerTree
{
    [Serializable]
    public class LocalDataStore : IServerTreeNode
    {
        public const string DefaultOfflineAE = "AETITLE";

        #region Private Fields

        private string _offlineAE;

        private string _path;
        private string _parentPath;
        private static readonly IDicomServerConfigurationProvider _dicomServerConfigurationProvider =
            DicomServerConfigurationHelper.GetDicomServerConfigurationProvider();
		
        #endregion

        internal LocalDataStore()
        {
        }

        public IDicomServerConfigurationProvider DicomServerConfigurationProvider
        {
            get { return _dicomServerConfigurationProvider; }
        }

        public override string ToString()
        {
            try
            {
                if (_dicomServerConfigurationProvider.NeedsRefresh)
                    _dicomServerConfigurationProvider.RefreshAsync();

                if (_dicomServerConfigurationProvider.ConfigurationExists)
                    return String.Format(SR.FormatLocalDataStoreDetails, DisplayName, _dicomServerConfigurationProvider.AETitle, _dicomServerConfigurationProvider.Host, _dicomServerConfigurationProvider.Port, _dicomServerConfigurationProvider.InterimStorageDirectory);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
            }

            return String.Format(SR.FormatLocalDataStoreConfigurationUnavailable, OfflineAE);
        }

        // NOTE: this is not good, but it's the only way to allow certain things to work without altering some of the services.
        // When the server tree is refactored, this must be dealt with.
        public string GetClientAETitle()
        {
            string ae = null;
            try
            {
                if (_dicomServerConfigurationProvider.NeedsRefresh)
                    _dicomServerConfigurationProvider.RefreshAsync();

                if (_dicomServerConfigurationProvider.ConfigurationExists)
                    ae = _dicomServerConfigurationProvider.AETitle;
            }
            catch(Exception e)
            {
                Platform.Log(LogLevel.Warn, e, "Failed to retrieve the dicom server's ae title.");
            }

            return ae ?? OfflineAE;
        }

        public string OfflineAE
        {
            get { return _offlineAE ?? DefaultOfflineAE; }
            set { _offlineAE = value ?? DefaultOfflineAE; }
        }

        #region IServerTreeNode Members

        public bool IsChecked
        {
            get { return false; }	
        }

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
            get { return SR.MyDataStoreTitle; }
        }

        #endregion

        internal void ChangeParentPath(string parentPath)
        {
            _parentPath = parentPath ?? "";
            _path = _parentPath + Name;
        }
    }
}