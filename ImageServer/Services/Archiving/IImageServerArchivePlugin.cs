#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Archiving
{
	public interface IImageServerArchivePlugin
	{
		ArchiveTypeEnum ArchiveType { get;}
		void Start(PartitionArchive archive);
		void Stop();
	}
}
