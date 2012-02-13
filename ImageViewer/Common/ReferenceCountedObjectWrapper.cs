#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common
{
	//TODO: this could be exposed in Common later, but for now we'll just keep it here.

	public class ReferenceCountedObjectWrapper
	{
		private readonly object _item;
		private int _referenceCount;

		public ReferenceCountedObjectWrapper(object item)
		{
			Platform.CheckForNullReference(item, "item");
			_item = item;
		}

		public object Item
		{
			get { return _item; }
		}

		public void IncrementReferenceCount()
		{
			Interlocked.Increment(ref _referenceCount);
		}

		public void DecrementReferenceCount()
		{
			Interlocked.Decrement(ref _referenceCount);
		}

		public bool IsReferenceCountAboveZero()
		{
			return 0 < Thread.VolatileRead(ref _referenceCount);
		}

		public int ReferenceCount
		{
			get { return Thread.VolatileRead(ref _referenceCount); }
		}
	}
	
	public class ReferenceCountedObjectWrapper<T> : ReferenceCountedObjectWrapper
	{
		public ReferenceCountedObjectWrapper(T item)
			: base(item)
		{
		}

		public new T Item
		{
			get { return (T)base.Item; }
		}
	}
}