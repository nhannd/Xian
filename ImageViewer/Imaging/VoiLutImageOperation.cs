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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A specialization of the <see cref="BasicImageOperation"/> where the
	/// originator is an <see cref="IVoiLutManager"/>.
	/// </summary>
	public class VoiLutImageOperation : BasicImageOperation
	{
		/// <summary>
		/// Mandatory constructor.
		/// </summary>
		public VoiLutImageOperation(ApplyDelegate applyDelegate)
			: base(GetVoiLutManager, applyDelegate)
		{
		}

		/// <summary>
		/// Returns the <see cref="IVoiLutManager"/> associated with the 
		/// <see cref="IPresentationImage"/>, or null.
		/// </summary>
		/// <remarks>
		/// When used in conjunction with an <see cref="ImageOperationApplicator"/>,
		/// it is always safe to cast the return value directly to <see cref="IVoiLutManager"/>
		/// without checking for null from within the <see cref="BasicImageOperation.ApplyDelegate"/> 
		/// specified in the constructor.
		/// </remarks>
		public override IMemorable GetOriginator(IPresentationImage image)
		{
			return base.GetOriginator(image) as IVoiLutManager;
		}

		private static IMemorable GetVoiLutManager(IPresentationImage image)
		{
			if (image is IVoiLutProvider)
				return ((IVoiLutProvider)image).VoiLutManager;

			return null;
		}
	}
}
