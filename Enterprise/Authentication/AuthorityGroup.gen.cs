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
    /// AuthorityGroup entity
    /// </summary>
	
	public partial class AuthorityGroup : ClearCanvas.Enterprise.Core.Entity
	{
       	#region Private fields
       	
		
	  	private string _name;
	  	
	  	private ISet<ClearCanvas.Enterprise.Authentication.AuthorityToken> _authorityTokens;
	  	
	  	private ISet<ClearCanvas.Enterprise.Authentication.User> _users;
	  	
	  	
	  	#endregion
	  	
	  	#region Constructors
	  	
	  	/// <summary>
	  	/// Default no-args constructor required by NHibernate
	  	/// </summary>
	  	public AuthorityGroup()
	  	{
		 	
		  	_authorityTokens = new HashedSet<ClearCanvas.Enterprise.Authentication.AuthorityToken>();
		  	
		  	_users = new HashedSet<ClearCanvas.Enterprise.Authentication.User>();
		  	
		  	
		  	CustomInitialize();
	  	}
                
		
	  	/// <summary>
	  	/// All fields constructor
	  	/// </summary>
	  	public AuthorityGroup(string name1, ISet<ClearCanvas.Enterprise.Authentication.AuthorityToken> authoritytokens1, ISet<ClearCanvas.Enterprise.Authentication.User> users1)
			:base()
	  	{
		  	CustomInitialize();

			
		  	_name = name1;
		  	
		  	_authorityTokens = authoritytokens1;
		  	
		  	_users = users1;
		  	
	  	}
		
                
	  	#endregion
	  	
	  	#region Public Properties
	  	
	  	
		
		
		[PersistentProperty]
		[Required]
		[Length(100)]
		[Unique]
	  	public virtual string Name
	  	{
			
			get { return _name; }
			
			
			 set { _name = value; }
			
	  	}
		
	  	
		
		
		[PersistentProperty]
	  	public virtual ISet<ClearCanvas.Enterprise.Authentication.AuthorityToken> AuthorityTokens
	  	{
			
			get { return _authorityTokens; }
			
			
	  	}
		
	  	
		
		
		[PersistentProperty]
	  	public virtual ISet<ClearCanvas.Enterprise.Authentication.User> Users
	  	{
			
			get { return _users; }
			
			
	  	}
		
	  	
	  	
	  	#endregion
	}
}
