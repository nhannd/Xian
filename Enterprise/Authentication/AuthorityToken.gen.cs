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
    /// AuthorityToken entity
    /// </summary>
	
	public partial class AuthorityToken : ClearCanvas.Enterprise.Core.Entity
	{
       	#region Private fields
       	
		
	  	private string _name;
	  	
	  	private string _description;
	  	
	  	
	  	#endregion
	  	
	  	#region Constructors
	  	
	  	/// <summary>
	  	/// Default no-args constructor required by NHibernate
	  	/// </summary>
	  	public AuthorityToken()
	  	{
		 	
		  	
		  	CustomInitialize();
	  	}
                
		
	  	/// <summary>
	  	/// All fields constructor
	  	/// </summary>
	  	public AuthorityToken(string name1, string description1)
			:base()
	  	{
		  	CustomInitialize();

			
		  	_name = name1;
		  	
		  	_description = description1;
		  	
	  	}
		
                
	  	#endregion
	  	
	  	#region Public Properties
	  	
	  	
		
		
		[PersistentProperty]
		[Required]
		[Length(400)]
		[Unique]
	  	public virtual string Name
	  	{
			
			get { return _name; }
			
			
			 set { _name = value; }
			
	  	}
		
	  	
		
		
		[PersistentProperty]
		[Length(1024)]
	  	public virtual string Description
	  	{
			
			get { return _description; }
			
			
			 set { _description = value; }
			
	  	}
		
	  	
	  	
	  	#endregion
	}
}
