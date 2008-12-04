// This file is machine generated - changes will be lost.
using System;
using System.Collections.Generic;
using System.Text;

using Iesi.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;


namespace ClearCanvas.Enterprise.Configuration
{


    /// <summary>
    /// ConfigurationDocument entity
    /// </summary>
	
	public partial class ConfigurationDocument : ClearCanvas.Enterprise.Core.Entity
	{
       	#region Private fields
       	
		
	  	private string _documentName;
	  	
	  	private string _documentVersionString;
	  	
	  	private string _user;
	  	
	  	private string _instanceKey;
	  	
	  	private ClearCanvas.Enterprise.Configuration.ConfigurationDocumentBody _body;
	  	
	  	
	  	#endregion
	  	
	  	#region Constructors
	  	
	  	/// <summary>
	  	/// Default no-args constructor required by NHibernate
	  	/// </summary>
	  	public ConfigurationDocument()
	  	{
		 	
		  	
		  	CustomInitialize();
	  	}
                
		
	  	/// <summary>
	  	/// All fields constructor
	  	/// </summary>
	  	public ConfigurationDocument(string documentname1, string documentversionstring1, string user1, string instancekey1, ClearCanvas.Enterprise.Configuration.ConfigurationDocumentBody body1)
			:base()
	  	{
		  	CustomInitialize();

			
		  	_documentName = documentname1;
		  	
		  	_documentVersionString = documentversionstring1;
		  	
		  	_user = user1;
		  	
		  	_instanceKey = instancekey1;
		  	
		  	_body = body1;
		  	
	  	}
		
                
	  	#endregion
	  	
	  	#region Public Properties
	  	
	  	
		
		
		[PersistentProperty]
		[Required]
		[Length(255)]
	  	public virtual string DocumentName
	  	{
			
			get { return _documentName; }
			
			
			 set { _documentName = value; }
			
	  	}
		
	  	
		
		
		[PersistentProperty]
		[Required]
		[Length(30)]
	  	public virtual string DocumentVersionString
	  	{
			
			get { return _documentVersionString; }
			
			
			 set { _documentVersionString = value; }
			
	  	}
		
	  	
		
		
		[PersistentProperty]
		[Length(50)]
	  	public virtual string User
	  	{
			
			get { return _user; }
			
			
			 set { _user = value; }
			
	  	}
		
	  	
		
		
		[PersistentProperty]
		[Length(100)]
	  	public virtual string InstanceKey
	  	{
			
			get { return _instanceKey; }
			
			
			 set { _instanceKey = value; }
			
	  	}
		
	  	
		
		
		[PersistentProperty]
	  	public virtual ClearCanvas.Enterprise.Configuration.ConfigurationDocumentBody Body
	  	{
			
			get { return _body; }
			
			
			 set { _body = value; }
			
	  	}
		
	  	
	  	
	  	#endregion
	}
}
