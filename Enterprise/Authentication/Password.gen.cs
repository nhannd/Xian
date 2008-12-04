// This file is machine generated - changes will be lost.
using System;
using System.Collections.Generic;
using System.Text;

using Iesi.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;


namespace ClearCanvas.Enterprise.Authentication
{


    /// <summary>
    /// Password component
    /// </summary>
	public sealed partial class Password : ValueObject, IEquatable<Password>, IAuditFormattable
	{
       	#region Private fields
       	
		
	  	private string _salt;
	  	
	  	private string _saltedHash;
	  	
	  	private DateTime? _expiryTime;
	  	
	  	
	  	#endregion
	  	
	  	#region Constructors
	  	
	  	/// <summary>
	  	/// Default no-args constructor required by NHibernate
	  	/// </summary>
	  	public Password()
	  	{
		 	
		  	
		  	CustomInitialize();
	  	}

		
	  	/// <summary>
	  	/// All fields constructor
	  	/// </summary>
	  	public Password(string salt1, string saltedhash1, DateTime? expirytime1)
	  	{
		  	CustomInitialize();

			
		  	_salt = salt1;
		  	
		  	_saltedHash = saltedhash1;
		  	
		  	_expiryTime = expirytime1;
		  	
	  	}
		
                
	  	#endregion
	  	
	  	#region Public Properties
	  	
	  	
		
		
		[PersistentProperty]
		[Required]
	  	public string Salt
	  	{
			
			get { return _salt; }
			
			
	  	}
		
	  	
		
		
		[PersistentProperty]
		[Required]
	  	public string SaltedHash
	  	{
			
			get { return _saltedHash; }
			
			
	  	}
		
	  	
		
		
		[PersistentProperty]
	  	public DateTime? ExpiryTime
	  	{
			
			get { return _expiryTime; }
			
			
			set { _expiryTime = value; }
			
	  	}
		
	  	
	  	
	  	#endregion
	  	
	  	#region IEquatable methods
	  	
	  	public bool Equals(Password that)
		{
			return (that != null) &&
	  	
			((this._salt == default(string)) ? (that._salt == default(string)) : this._salt.Equals(that._salt)) &&
	  	
			((this._saltedHash == default(string)) ? (that._saltedHash == default(string)) : this._saltedHash.Equals(that._saltedHash)) &&
	  	
			((this._expiryTime == default(DateTime?)) ? (that._expiryTime == default(DateTime?)) : this._expiryTime.Equals(that._expiryTime)) &&
	  	
				true;
		}
	  	
	  	#endregion
	  	
	  	#region Object overrides
	  	
	  	public override bool Equals(object that)
		{
			return this.Equals(that as Password);
		}
		
		public override int GetHashCode()
		{
			return
	  	
				(_salt == default(string) ? 0 : _salt.GetHashCode()) ^
	  	
				(_saltedHash == default(string) ? 0 : _saltedHash.GetHashCode()) ^
	  	
				(_expiryTime == default(DateTime?) ? 0 : _expiryTime.GetHashCode()) ^
	  	
				0;
		}
				
	  	#endregion
	  	
	  	/// <summary>
	  	/// Returns a clone of this object.  A deep-copy is performed, so the clone will not share
	  	/// any mutable data with this object.
	  	/// NB. Note that collections are not currently supported.  If this object contains collections
	  	/// they will not be cloned.  It should be relatively easy to add collection support when the need arises.
	  	/// </summary>
        public override object Clone()
        {
			Password clone = new Password();
		
		
	  		clone._salt = this._salt;
		
	  		clone._saltedHash = this._saltedHash;
		
	  		clone._expiryTime = this._expiryTime;
		
			return clone;
        }
		
		#region IAuditFormattable Members

		void IAuditFormattable.Write(IObjectWriter writer)
		{
			
		  	writer.WriteProperty("Salt", _salt);
		  	
		  	writer.WriteProperty("SaltedHash", _saltedHash);
		  	
		  	writer.WriteProperty("ExpiryTime", _expiryTime);
		  	
		}

		#endregion
	}
}
