#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.ComponentModel;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.Server.ShredHost
{
    public partial class WcfDataShred
    {
        public WcfDataShred(int id, string name, string description, bool isRunning)
        {
            this._idField = id;
            this._nameField = name;
            this._descriptionField = description;
            this._isRunningField = isRunning;
        }
    }
}

namespace ClearCanvas.Server.ShredHostClientUI
{
    public class Shred : INotifyPropertyChanged
    {
        public Shred() : this(0, "defaultName", "defaultDescription", false)
        {

        }

        public Shred(int id, string name, string description, bool isRunning)
        {
            _id = id;
            _name = name;
            _description = description;
            _isRunning = isRunning;
        }

        public WcfDataShred GetWcfDataShred()
        {
            return new WcfDataShred(this.Id, this.Name, this.Description, this.IsRunning);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        public override bool Equals(object obj)
        {
            Shred otherShred = obj as Shred;
            if (null == otherShred)
                return false;
            else if (this == otherShred)
                return true;
            else if (this.Id == otherShred.Id)
                return true;
            else
                return false;
        }

        #region Properties
        private string _name;
        private bool _isRunning;
        private string _description;
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
	
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                NotifyPropertyChanged("Description");
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
