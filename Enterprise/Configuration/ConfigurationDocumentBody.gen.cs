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
    /// ConfigurationDocumentBody entity
    /// </summary>
	
	public partial class ConfigurationDocumentBody : ClearCanvas.Enterprise.Core.Entity
	{
       	#region Private fields
       	
		
	  	private ClearCanvas.Enterprise.Configuration.ConfigurationDocument _document;
	  	
	  	private string _documentText;
	  	
	  	
	  	#endregion
	  	
	  	#region Constructors
	  	
	  	/// <summary>
	  	/// Default no-args constructor required by NHibernate
	  	/// </summary>
	  	public ConfigurationDocumentBody()
	  	{
		 	
		  	
		  	CustomInitialize();
	  	}
                
		
	  	/// <summary>
	  	/// All fields constructor
	  	/// </summary>
	  	public ConfigurationDocumentBody(ClearCanvas.Enterprise.Configuration.ConfigurationDocument document1, string documenttext1)
			:base()
	  	{
		  	CustomInitialize();

			
		  	_document = document1;
		  	
		  	_documentText = documenttext1;
		  	
	  	}
		
                
	  	#endregion
	  	
	  	#region Public Properties
	  	
	  	
		
		
		[PersistentProperty]
	  	public virtual ClearCanvas.Enterprise.Configuration.ConfigurationDocument Document
	  	{
			
			get { return _document; }
			
			
			 set { _document = value; }
			
	  	}
		
	  	
		
		
		[PersistentProperty]
		[Length(65000)]
	  	public virtual string DocumentText
	  	{
			
			get { return _documentText; }
			
			
			 set { _documentText = value; }
			
	  	}
		
	  	
	  	
	  	#endregion
	}
}
