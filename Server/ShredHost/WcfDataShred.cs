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
using System.Runtime.Serialization;
using System.ComponentModel;
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
    /// <summary>
    /// Class created only to allow WCF serialization and usage in service operations
    /// </summary>
    [Serializable]
    public partial class WcfDataShred
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
            }
        }
	

        public bool IsRunning
        {
            get { return _isRunning; }
            set 
            { 
                _isRunning = value;
            }
        }
	
        public string Description
        {
            get { return _description; }
            set 
            { 
                _description = value;
            }
        }
	
        public string Name
        {
            get { return _name; }
            set 
            { 
                _name = value;
            }
        }
	
        #endregion
    }
}
