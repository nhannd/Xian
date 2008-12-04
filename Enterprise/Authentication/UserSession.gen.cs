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
    /// UserSession entity
    /// </summary>
	
	public partial class UserSession : ClearCanvas.Enterprise.Core.Entity
	{
       	#region Private fields
       	
		
	  	private string _sessionId;
	  	
	  	private DateTime _expiryTime;
	  	
	  	
	  	#endregion
	  	
	  	#region Constructors
	  	
	  	/// <summary>
	  	/// Default no-args constructor required by NHibernate
	  	/// </summary>
	  	public UserSession()
	  	{
		 	
		  	_expiryTime = Platform.Time;
		  	
		  	
		  	CustomInitialize();
	  	}
                
		
	  	/// <summary>
	  	/// All fields constructor
	  	/// </summary>
	  	public UserSession(string sessionid1, DateTime expirytime1)
			:base()
	  	{
		  	CustomInitialize();

			
		  	_sessionId = sessionid1;
		  	
		  	_expiryTime = expirytime1;
		  	
	  	}
		
                
	  	#endregion
	  	
	  	#region Public Properties
	  	
	  	
		
		
		[PersistentProperty]
		[Required]
	  	public virtual string SessionId
	  	{
			
			get { return _sessionId; }
			
			
			 set { _sessionId = value; }
			
	  	}
		
	  	
		
		
		[PersistentProperty]
		[Required]
	  	public virtual DateTime ExpiryTime
	  	{
			
			get { return _expiryTime; }
			
			
			 set { _expiryTime = value; }
			
	  	}
		
	  	
	  	
	  	#endregion
	}
}
