using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
    /// <summary>
    /// Class created only to allow WCF serialization and usage in service operations
    /// </summary>
    [Serializable]
    public class WcfDataShred : INotifyPropertyChanged
    {
        public WcfDataShred(int id, string name, string description, bool isRunning)
        {
            Platform.CheckForNullReference(name, "name");
            Platform.CheckForNullReference(description, "description");
            Platform.CheckForEmptyString(name, "name");

            _id = id;
            _name = name;
            _description = description;
            _isRunning = isRunning;
        }

        #region Properties
        private string _name;
        private string _description;
        private bool _isRunning;
        private int _id;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }
	

        public bool IsRunning
        {
            get { return _isRunning; }
            set 
            { 
                _isRunning = value;
                NotifyPropertyChanged("IsRunning");
            }
        }
	
        public string Description
        {
            get { return _description; }
            set 
            { 
                _description = value;
                NotifyPropertyChanged("Description");
            }
        }
	
        public string Name
        {
            get { return _name; }
            set 
            { 
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
	
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
