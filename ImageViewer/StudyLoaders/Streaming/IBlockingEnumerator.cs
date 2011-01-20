#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	//TODO: work this into common, so that all thread pools aren't tied to queues that can't be changed.
	public interface IBlockingEnumerator<T> : IEnumerable<T>
	{
		bool ContinueBlocking { get; set; }
	}
}
