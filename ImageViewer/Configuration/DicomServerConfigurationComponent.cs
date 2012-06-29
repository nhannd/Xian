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

namespace ClearCanvas.ImageViewer.Configuration
{
	/// <summary>
    /// Extension point for views onto <see cref="DicomServerConfigurationComponent"/>
    /// </summary>
    [ExtensionPoint]
	public sealed class DicomServerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    //TODO (Marmot):Move to ImageViewer?

    /// <summary>
    /// DicomServerConfigurationComponent class
    /// </summary>
    [AssociateView(typeof(DicomServerConfigurationComponentViewExtensionPoint))]
    public class DicomServerConfigurationComponent : ConfigurationApplicationComponent
    {
        private string _hostName;
        private string _aeTitle;
        private int _port;

    	public override void Start()
        {
			base.Start();

    	    _hostName = DicomServer.HostName;
            _aeTitle = DicomServer.AETitle;
            _port = DicomServer.Port;
		}

    	public override void Save()
    	{
    		try
    		{
    		    DicomServer.UpdateConfiguration(new DicomServerConfiguration
    		                                        {
    		                                            HostName = _hostName,
    		                                            AETitle = AETitle,
    		                                            Port = Port
    		                                        });
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
				return base.HasValidationErrors;
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

		[ValidateGreaterThan(0, Inclusive = false, Message = "ValidationPortOutOfRange")]
		[ValidateLessThan(65536, Inclusive = false, Message = "ValidationPortOutOfRange")]
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
		
		#endregion
    }
}
