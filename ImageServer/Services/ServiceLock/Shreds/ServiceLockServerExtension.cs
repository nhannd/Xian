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

namespace ClearCanvas.ImageServer.Services.ServiceLock.Shreds
{
	/// <summary>
	/// Plugin to handle ServiceLock processing for the ImageServer.
	/// </summary>
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class ServiceLockServerExtension : Shred
	{
		private readonly string _className;
      
		public ServiceLockServerExtension()
		{
			_className = GetType().ToString();
		}
		public override void Start()
		{
			Platform.Log(LogLevel.Info, "{0}[{1}]: Start invoked", _className, AppDomain.CurrentDomain.FriendlyName);

			ServiceLockServerManager.Instance.StartService();
		}

		public override void Stop()
		{
			ServiceLockServerManager.Instance.StopService();

			Platform.Log(LogLevel.Info, "{0}[{1}]: Stop invoked", _className, AppDomain.CurrentDomain.FriendlyName);
		}

		public override string GetDisplayName()
		{
			return SR.ServiceLockServer;
		}

		public override string GetDescription()
		{
			return SR.ServiceLockServerDescription;
		}
	}
}