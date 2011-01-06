#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.ImageServer.Services.Archiving.Shreds
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class ArchivingShredExtension : Shred
	{
		#region Private Members

		private readonly string _className;

		#endregion

		#region Constructors

		public ArchivingShredExtension()
		{
			_className = GetType().ToString();
		}

		#endregion

		#region IShred Implementation Shred Override

		public override void Start()
		{
			Platform.Log(LogLevel.Info,"{0}[{1}]: Start invoked", _className, AppDomain.CurrentDomain.FriendlyName);

			ArchivingShredManager.Instance.StartService();
		}

		public override void Stop()
		{
			ArchivingShredManager.Instance.StopService();

			Platform.Log(LogLevel.Info, "{0}[{1}]: Stop invoked", _className, AppDomain.CurrentDomain.FriendlyName);
		}

		public override string GetDisplayName()
		{
			return SR.ArchivingServer;
		}

		public override string GetDescription()
		{
			return SR.ArchivingServerDescription;
		}

		#endregion

	}
}