#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	//TODO: this could be exposed in Common later, but for now we'll just keep it here.

	internal class ReferenceCountedObjectWrapper
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
	
	internal class ReferenceCountedObjectWrapper<T> : ReferenceCountedObjectWrapper
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