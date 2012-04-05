#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	/// <summary>
	/// Exception handling policy for <see cref="DicomServerConfigurationHelper.UpdateException"/>s and <see cref="DicomServerConfigurationHelper.RefreshException"/>s.
	/// </summary>
	[ExceptionPolicyFor(typeof(DicomServerConfigurationHelper.UpdateException))]
	[ExceptionPolicyFor(typeof(DicomServerConfigurationHelper.RefreshException))]

	[ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
	public sealed class DicomServerConfigurationHelperExceptionPolicy : IExceptionPolicy
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public DicomServerConfigurationHelperExceptionPolicy()
		{
		}

		#region IExceptionPolicy Members

		///<summary>
		/// Handles the specified exception.
		///</summary>
		public void Handle(Exception e, IExceptionHandlingContext exceptionHandlingContext)
		{
			if (!(e.InnerException is EndpointNotFoundException))
				exceptionHandlingContext.Log(LogLevel.Error, e);

			if (e is DicomServerConfigurationHelper.RefreshException)
			{
				exceptionHandlingContext.ShowMessageBox(SR.MessageFailedToRetrieveDicomServerConfiguration);
			}
			else if (e is DicomServerConfigurationHelper.UpdateException)
			{
				exceptionHandlingContext.ShowMessageBox(SR.MessageFailedToUpdateDicomServerConfiguration);
			}
		}

		#endregion
	}
	
	/// <summary>
    /// Extension point for views onto <see cref="DicomServerConfigurationComponent"/>
    /// </summary>
    [ExtensionPoint]
	public sealed class DicomServerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    //TODO (Marmot):Move.

	//NOTE: this may not be the best place for this, but it doesn't make sense to have any of these tools without
	// the configuration components (or vice versa) anyway.

    /// <summary>
    /// DicomServerConfigurationComponent class
    /// </summary>
    [AssociateView(typeof(DicomServerConfigurationComponentViewExtensionPoint))]
    public class DicomServerConfigurationComponent : ConfigurationApplicationComponent
    {
        private string _aeTitle;
        private int _port;
        private string _storageDirectory;
		private bool _enabled;

    	private void Refresh()
    	{
    		try
    		{
				DicomServerConfigurationHelper.Refresh(true);

    			_aeTitle = DicomServerConfigurationHelper.AETitle;
    			_port = DicomServerConfigurationHelper.Port;
				_storageDirectory = DicomServerConfigurationHelper.FileStoreDirectory;

    			Enabled = true;
    		}
    		catch(Exception e)
    		{
    			Enabled = false;
				
				_aeTitle = "";
    			_port	= 0;
				_storageDirectory = "";

    			ExceptionHandler.Report(e, this.Host.DesktopWindow);
    		}

			NotifyPropertyChanged("AETitle");
			NotifyPropertyChanged("Port");
			NotifyPropertyChanged("StorageDirectory");
    	}

    	public override void Start()
        {
			base.Start();
			Refresh();
		}

    	public override void Save()
    	{
    		try
    		{
    			DicomServerConfigurationHelper.Update("localhost", _aeTitle, _port, _storageDirectory);
    		}
    		catch (Exception e)
    		{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
    		}
    	}

    	public override bool HasValidationErrors
		{
			get
			{
				AETitle = AETitle.Trim();

				return Enabled && base.HasValidationErrors;
			}
		}

        #region Presentation Model

		[ValidateLength(1, 16, Message = "ValidationAETitleLengthIncorrect")]
		[ValidateRegex(@"[\r\n\e\f\\]+", SuccessOnMatch = false, Message = "ValidationAETitleInvalidCharacters")]
		public string AETitle
        {
            get { return _aeTitle; }
            set 
            {
				if (_aeTitle == value)
					return;

				_aeTitle = value ?? "";
                base.Modified = true;
				NotifyPropertyChanged("AETitle");
            }
        }

		[ValidateGreaterThanAttribute(0, Inclusive = false, Message = "ValidationPortOutOfRange")]
		[ValidateLessThanAttribute(65536, Inclusive = false, Message = "ValidationPortOutOfRange")]
		public int Port
        {
            get { return _port; }
            set 
            {
				if (_port == value)
					return;

                _port = value;
				base.Modified = true;
				NotifyPropertyChanged("Port");
			}
        }

		//private for now.
		private string StorageDirectory
		{
			get { return _storageDirectory; }
			set
			{
				if (_storageDirectory == value)
					return;

				_storageDirectory = value;
				base.Modified = true;
				NotifyPropertyChanged("StorageDirectory");
			}
		}
		
		public bool Enabled
        {
			get { return _enabled; }
			private set
			{
				if (_enabled == value)
					return;

				_enabled = value;
				NotifyPropertyChanged("Enabled");
			}
        }

		#endregion
    }
}
