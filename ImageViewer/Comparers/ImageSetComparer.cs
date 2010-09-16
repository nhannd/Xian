#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Comparers
{
	public interface IImageSetComparer : IComparer<IImageSet>, IEquatable<IImageSetComparer>
	{ }

	/// <summary>
	/// Base class for comparers that compare some aspect of <see cref="IImageSet"/>s.
	/// </summary>
	public abstract class ImageSetComparer : ComparerBase, IImageSetComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ImageSetComparer"/>.
		/// </summary>
		protected ImageSetComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageSetComparer"/>.
		/// </summary>
		protected ImageSetComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IImageSet> Members

		/// <summary>
		/// Compares two <see cref="IImageSet"/>s.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public abstract int Compare(IImageSet x, IImageSet y);

		#endregion

		public bool Equals(IImageSetComparer other)
		{
			return other is ImageSetComparer && Equals((ImageSetComparer)other);
		}

		public virtual bool Equals(ImageSetComparer other)
		{
			return base.Equals(other);
		}
	}
}
