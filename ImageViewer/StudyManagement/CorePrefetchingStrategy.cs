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

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public interface ICorePrefetchingStrategy
	{
		bool CanRetrieveFrame(Frame frame);

		void RetrieveFrame(Frame frame);

		bool CanDecompressFrame(Frame frame);

		void DecompressFrame(Frame frame);
	}

	public class SimpleCorePrefetchingStrategy : ICorePrefetchingStrategy
	{
		private readonly Predicate<Frame> _canRetrieve;

		public SimpleCorePrefetchingStrategy()
			: this(ignore => true)
		{
		}

		public SimpleCorePrefetchingStrategy(Predicate<Frame> canRetrieve)
		{
			Platform.CheckForNullReference(canRetrieve, "canRetrieve");
			_canRetrieve = canRetrieve;
		}

		public bool CanRetrieveFrame(Frame frame)
		{
			return _canRetrieve(frame);
		}

		public void RetrieveFrame(Frame frame)
		{
			frame.GetNormalizedPixelData();
		}

		public bool CanDecompressFrame(Frame frame)
		{
			return false;
		}

		public void DecompressFrame(Frame frame) { }
	}

}