#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System;
using System.Collections.Generic;

namespace ClearCanvas.Common.Utilities
{
	// TODO (Stewart): get rid of this too.

	/// <summary>
	/// A cache of <see cref="IReferenceCountable"/> objects.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Objects are 'added' and 'removed' to/from the cache using string keys.
	/// Repeated <see cref="Add"/> operations simply increment the reference count, and repeated <see cref="Remove"/>
	/// operations decrement the reference count.  When the reference count falls to zero, the object is 
	/// actually removed from the cache.
	/// </para>
	/// <para>
	/// This class is not thread-safe.
	/// </para>
	/// </remarks>
	public class ReferenceCountedObjectCache
	{
		private Dictionary<string, IReferenceCountable> _cache = new Dictionary<string, IReferenceCountable>();

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ReferenceCountedObjectCache()
		{

		}

		/// <summary>
		/// Adds an <see cref="IReferenceCountable"/> object to the cache.
		/// </summary>
		/// <remarks>
		/// When an object already exists for the specified <paramref name="key"/>, its reference count
		/// is simply incremented.  Otherwise, the object is first added, then incremented.
		/// </remarks>
		/// <param name="key">A string key identifying the <see cref="IReferenceCountable"/> object.</param>
		/// <param name="referenceCountedObject">The <see cref="IReferenceCountable"/> object to add to the cache.</param>
		public void Add(string key, IReferenceCountable referenceCountedObject)
		{
			if (_cache.ContainsKey(key))
			{
				_cache[key].IncrementReferenceCount();
			}
			else
			{
				referenceCountedObject.IncrementReferenceCount();
				_cache.Add(key, referenceCountedObject);
			}
		}

		/// <summary>
		/// Removes an object in the cache, given the specified <paramref name="key"/>.
		/// </summary>
		/// <remarks>
		/// When an object exists for the input <paramref name="key"/>, its reference count
		/// is first decremented, and the object is only actually removed if its reference count
		/// is zero.
		/// </remarks>
		/// <param name="key">The key at which to remove the stored object.</param>
		public void Remove(string key)
		{
			if (key == String.Empty)
				return;

			if (_cache.ContainsKey(key))
			{
				IReferenceCountable referenceCountedObject = _cache[key];
				referenceCountedObject.DecrementReferenceCount();

				if (referenceCountedObject.IsReferenceCountZero)
				{
					_cache.Remove(key);

					IDisposable disposable = referenceCountedObject as IDisposable;
					
					if (disposable != null)
						disposable.Dispose();
				}
			}
		}

		/// <summary>
		/// Gets the object stored with the given <paramref name="key"/> if one exists, otherwise null.
		/// </summary>
		public IReferenceCountable this[string key]
		{
			get
			{
				if (_cache.ContainsKey(key))
					return _cache[key];
				else
					return null;
			}
		}

		/// <summary>
		/// Clears the cache.
		/// </summary>
		public void Clear()
		{
			_cache.Clear();
		}
	}
}
