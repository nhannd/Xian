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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	//TODO: incorporate this kind of thing into the thread pools in common, where the items being 
	//processed can be added and/or removed.
	public class BlockingThreadPool<T> : ThreadPoolBase
	{
		private readonly IBlockingEnumerator<T> _blockingEnumerator;
		private readonly ProcessItemDelegate<T> _processItem;

		public BlockingThreadPool(IBlockingEnumerator<T> blockingEnumerator, ProcessItemDelegate<T> processItem)
		{
			_blockingEnumerator = blockingEnumerator;
			_processItem = processItem;
		}

		protected override bool OnStop(bool completeBeforeStop)
		{
			if (!base.OnStop(completeBeforeStop))
				return false;

			_blockingEnumerator.ContinueBlocking = false;
			return true;
		}

		protected override bool OnStart()
		{
			if (!base.OnStart())
				return false;

			_blockingEnumerator.ContinueBlocking = true;
			return true;
		}

		protected override void RunThread()
		{
			while (base.State != StartStopState.Stopping)
			{
				foreach (T item in _blockingEnumerator)
				{
					try
					{
						_processItem(item);
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e);
					}
				}
			}
		}
	}
}
