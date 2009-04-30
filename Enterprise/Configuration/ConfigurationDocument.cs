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
using System.Collections;
using System.Collections.Generic;

using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;


namespace ClearCanvas.Enterprise.Configuration {


    /// <summary>
    /// Stores a set of settings keys and values for a given settings group.  Used internally by the framework.
    /// </summary>
    [UniqueKey("DocumentKey", new string[]{"DocumentName", "DocumentVersionString", "User", "InstanceKey"})]
	public partial class ConfigurationDocument : Entity
	{
        /// <summary>
        /// Constructs a new configuration document with an empty body.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="versionString"></param>
        /// <param name="user"></param>
        /// <param name="instanceKey"></param>
        public ConfigurationDocument(string name, string versionString, string user, string instanceKey)
        {
            _documentName = name;
            _documentVersionString = versionString;
            _user = user;
            _instanceKey = instanceKey;
            _body = new ConfigurationDocumentBody(this, null);
        }


		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		#region Object overrides
		
		public override bool Equals(object obj)
		{
            ConfigurationDocument that = obj as ConfigurationDocument;
            if (that == null)
                return false;

            return this._documentName == that._documentName && this._documentVersionString == that._documentVersionString
                && this._instanceKey == that._instanceKey && this._user == that._user;
		}
		
		public override int GetHashCode()
		{
            int hash = _documentName.GetHashCode();

            if (_documentVersionString != null)
                hash ^= _documentVersionString.GetHashCode();
            if (_user != null)
                hash ^= _user.GetHashCode();
            if (_instanceKey != null)
                hash ^= _instanceKey.GetHashCode();
            return hash;
		}
		
		#endregion
    }
}