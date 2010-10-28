#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.Dicom.ServiceModel.Streaming;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageServer.Services.Streaming.Shreds
{
	/// <summary>
	/// Plugin to handle streaming request for the ImageServer.
	/// </summary>
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class HeaderStreamingServer : WcfShred
	{

		#region Private Members

		private readonly string _className;

		#endregion

		#region Constructors

		public HeaderStreamingServer()
		{
			_className = GetType().ToString();
		}

		#endregion

		#region IShred Implementation Shred Override

		public override void Start()
		{
			Platform.Log(LogLevel.Debug, "{0}[{1}]: Start invoked", _className, AppDomain.CurrentDomain.FriendlyName);

			try
			{
				Platform.Log(LogLevel.Info, "Starting {0} using basic Http binding", GetDisplayName());
                StartBasicHttpHost<HeaderStreamingService, IHeaderStreamingService>("HeaderStreaming", GetDescription());
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Fatal, e, "Unexpected exception starting Streaming Server Shred");

			    ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, SR.HeaderStreamingServerDisplayName,
                                     AlertTypeCodes.UnableToStart, null, TimeSpan.Zero,
			                         SR.AlertUnableToStart, e.Message);
			}
		}

		public override void Stop()
		{
			Platform.Log(LogLevel.Info, "{0}[{1}]: Stop invoked", _className, AppDomain.CurrentDomain.FriendlyName);
            StopHost("HeaderStreaming");
		}

		public override string GetDisplayName()
		{
			return SR.HeaderStreamingServerDisplayName;
		}

		public override string GetDescription()
		{
			return SR.HeaderStreamingServerDescription;
		}

		#endregion
	}
}