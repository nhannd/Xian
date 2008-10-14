#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Services.Configuration
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
				exceptionHandlingContext.ShowMessageBox(SR.ExceptionDicomServerConfigurationRefreshFailed);
			}
			else if (e is DicomServerConfigurationHelper.UpdateException)
			{
				exceptionHandlingContext.ShowMessageBox(SR.ExceptionDicomServerConfigurationUpdateFailed);
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

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerConfigurationComponent()
        {
		}

    	private void ConnectToClientInternal()
    	{
    		try
    		{
				DicomServerConfigurationHelper.Refresh(true);

    			_aeTitle = DicomServerConfigurationHelper.AETitle;
    			_port = DicomServerConfigurationHelper.Port;
				_storageDirectory = DicomServerConfigurationHelper.InterimStorageDirectory;

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

		public void Refresh()
		{
			BlockingOperation.Run(this.ConnectToClientInternal);
		}

		#endregion
    }
}
