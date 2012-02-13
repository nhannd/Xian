#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Common;


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
        	_creationTime = Platform.Time;
			_body = new ConfigurationDocumentBody(this, null, _creationTime);
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