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

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	// TODO (later): see if there's a way to cleanly get rid of this and/or 
	// put the delegates into one of the superclasses so it can be used on something other than just images.
	// For now, though, just leave it, since it's code that's already been released.

	/// <summary>
	/// A simple way to implement an ImageOperation, using delegates.
	/// </summary>
	public class BasicImageOperation : ImageOperation
	{
		/// <summary>
		/// Defines a delegate used to get the originator for a given <see cref="IPresentationImage"/>.
		/// </summary>
		public delegate IMemorable GetOriginatorDelegate(IPresentationImage image);

		/// <summary>
		/// Defines a delegate used to apply an undoable operation to an <see cref="IPresentationImage"/>.
		/// </summary>
		public delegate void ApplyDelegate(IPresentationImage image);

		private readonly GetOriginatorDelegate _getOriginatorDelegate;
		private readonly ApplyDelegate _applyDelegate;

		/// <summary>
		/// Mandatory constructor.
		/// </summary>
		public BasicImageOperation(GetOriginatorDelegate getOriginatorDelegate, ApplyDelegate applyDelegate)
		{
			Platform.CheckForNullReference(getOriginatorDelegate, "getOriginatorDelegate");
			Platform.CheckForNullReference(applyDelegate, "applyDelegate");

			_getOriginatorDelegate = getOriginatorDelegate;
			_applyDelegate = applyDelegate;
		}

		/// <summary>
		/// Gets the originator for the input <see cref="IPresentationImage"/>, which must be <see cref="IMemorable"/>.
		/// </summary>
		public override IMemorable GetOriginator(IPresentationImage image)
		{
			return _getOriginatorDelegate(image);
		}

		/// <summary>
		/// Applies the operation to the input <see cref="IPresentationImage"/>.
		/// </summary>
		public sealed override void Apply(IPresentationImage image)
		{
			_applyDelegate(image);
		}
	}
}
