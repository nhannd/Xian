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
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Archiving.Hsm
{
	[ExtensionOf(typeof(ImageServerArchiveExtensionPoint))]
	public class HsmArchiveExtension : ImageServerArchiveBase
	{
		private PartitionArchive _partitionArchive;

		public override ArchiveTypeEnum ArchiveType
		{
			get { return ArchiveTypeEnum.HsmArchive; }
		}

		public override void Start(PartitionArchive archive)
		{
			_partitionArchive = archive;

			throw new NotImplementedException();
		}

		public override void Stop()
		{
			throw new NotImplementedException();
		}
	}
}