using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Dicom.Services
{
	public static class LocalApplicationEntity
	{
        private static string _aeTitle = null;
        private static string _dicomStoragePath = null;
        private static int _port = -1;
        private static event EventHandler _settingsUpdated;

        public static event EventHandler SettingsUpdated
        {
            add { _settingsUpdated += value; }
            remove { _settingsUpdated -= value; }
        }
        
        public static string AETitle
		{
			get 
            {
                if (_aeTitle == null)
                    _aeTitle = LocalAESettings.Default.AETitle;

                return _aeTitle; 
            }
		}

		public static int Port
		{
			get 
            {
                if (_port < 0)
                    _port = LocalAESettings.Default.Port;

                return _port; 
            }
		}

		public static string DicomStoragePath
		{
			get 
            { 
                if (_dicomStoragePath == null)
                    _dicomStoragePath = LocalAESettings.Default.DicomStoragePath;

                return _dicomStoragePath;                
            }
		}

        public static void UpdateSettings(string aeTitle, int port)
        {
            _aeTitle = aeTitle;
            _port = port;
            EventsHelper.Fire(_settingsUpdated, null, EventArgs.Empty);
        }
	}
}
