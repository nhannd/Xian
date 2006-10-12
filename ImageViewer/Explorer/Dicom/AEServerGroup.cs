using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	public class AEServerGroup
	{
        private List<DicomServer> _servers;
        private string _name;
		private string _groupID;

		public AEServerGroup()
		{
        }

        public List<DicomServer> Servers
        {
            get {
                if (_servers == null)
                    _servers = new List<DicomServer>();
                return _servers; 
            }
            set { _servers = value; }
        }

        public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	
		public string GroupID
		{
			get { return _groupID; }
			set { _groupID = value; }
		}

		public bool IsLocalDatastore
		{
			get
			{
                if (_servers.Count != 1)
					return false;

                if (_servers[0].DicomAE.Host == "localhost")
					return true;
				else
					return false;
			}
		}
        
    }
}
