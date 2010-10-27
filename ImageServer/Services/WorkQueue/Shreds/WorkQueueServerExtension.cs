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

namespace ClearCanvas.ImageServer.Services.WorkQueue.Shreds
{
	/// <summary>
	/// Plugin to handle WorkQueue processing for the ImageServer.
	/// </summary>
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class WorkQueueServerExtension : Shred
	{
		#region Private Members

		private readonly string _className;

		#endregion

		#region Constructors

		public WorkQueueServerExtension()
		{
			_className = GetType().ToString();
		}

		#endregion

		#region IShred Implementation Shred Override

		public override void Start()
		{
			Platform.Log(LogLevel.Info,"{0}[{1}]: Start invoked", _className, AppDomain.CurrentDomain.FriendlyName);

			WorkQueueServerManager.PrimaryInstance.StartService();
		}

		public override void Stop()
		{        
			WorkQueueServerManager.PrimaryInstance.StopService();

			Platform.Log(LogLevel.Info, "{0}[{1}]: Stop invoked", _className, AppDomain.CurrentDomain.FriendlyName);
		}

		public override string GetDisplayName()
		{
			return SR.WorkQueueServer;
		}

		public override string GetDescription()
		{
			return SR.WorkQueueServerDescription;
		}

		#endregion
	}
}