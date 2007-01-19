using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Server.ShredHostClientUI
{
    public class ShredHostClientUI : INotifyPropertyChanged, IDisposable
    {
        public ShredHostClientUI()
        {

            _shredHostProxy = new ShredHostClient();
            _shredCollection = new ShredCollection();
            
            WcfDataShred[] shreds = _shredHostProxy.GetShreds();
            foreach (WcfDataShred shred in shreds)
            {
                _shredCollection.Add(new Shred(shred._id, shred._name, shred._description, shred._isRunning));
            }

        }


        public void Toggle()
        {
            this.IsShredHostRunning = _shredHostProxy.IsShredHostRunning();
            if (this.IsShredHostRunning)
            {
                _shredHostProxy.Stop();
            }
            else
            {
                _shredHostProxy.Start();
            }
        }

        #region Properties

        private ShredCollection _shredCollection;
        private bool _isShredHostRunning;

        public bool IsShredHostRunning
        {
            get { return _isShredHostRunning; }
            set 
            { 
                _isShredHostRunning = value;
                NotifyPropertyChanged("IsShredHostRunning");
            }
        }

        public ShredCollection ShredCollection
        {
            get { return _shredCollection; }
            private set { _shredCollection = value; }
        }


        #endregion

        #region Private fields
        ShredHostClient _shredHostProxy;
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

        #region IDisposable Members

        public void Dispose()
        {
            _shredHostProxy.Close();
        }

        #endregion
    }
}
