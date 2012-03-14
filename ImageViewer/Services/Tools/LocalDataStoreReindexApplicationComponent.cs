#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common.LocalDataStore;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[ExtensionPoint]
	public sealed class LocalDataStoreReindexApplicationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(LocalDataStoreReindexApplicationComponentViewExtensionPoint))]
	public class LocalDataStoreReindexApplicationComponent : ApplicationComponent
	{
		internal LocalDataStoreReindexApplicationComponent()
		{
		}

		public ILocalDataStoreReindexer Reindexer { get; private set; }

		public override void Start()
		{
			Reindexer = new LocalDataStoreReindexer();
			Reindexer.Start();
			base.Start();
		}

		public override void Stop()
		{
			base.Stop();
			Reindexer.Dispose();
		}
	}
}
