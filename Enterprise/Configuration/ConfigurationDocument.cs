using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;
using System.Xml;
using System.IO;


namespace ClearCanvas.Enterprise.Configuration {


    /// <summary>
    /// Stores a set of settings keys and values for a given settings group.  Used internally by the framework.
    /// </summary>
	public partial class ConfigurationDocument : Entity
	{
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        /// <summary>
        /// Clears all stored values
        /// </summary>
        public void Clear()
        {
            _documentText = null;
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