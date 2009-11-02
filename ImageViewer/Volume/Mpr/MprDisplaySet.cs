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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	//TODO (cr Oct 2009): if the slice sets were display set descriptors, 
	//you could just attach them to the display set and this class could be removed.
	[Cloneable]
	public class MprDisplaySet : DisplaySet, IDisplaySet
	{
		[CloneCopyReference]
		private IMprSliceSet _sliceSet;

		public MprDisplaySet(string name, IMprSliceSet sliceSet)
			: base(name, sliceSet.Uid)
		{
			_sliceSet = sliceSet;
			_sliceSet.SliceSopsChanged += sliceSet_SliceSopsChanged;

			FillPresentationImages();
		}

		protected MprDisplaySet(MprDisplaySet source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_sliceSet.SliceSopsChanged += sliceSet_SliceSopsChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_sliceSet.SliceSopsChanged -= sliceSet_SliceSopsChanged;
				_sliceSet = null;
			}
			base.Dispose(disposing);
		}

		public IMprSliceSet SliceSet
		{
			get { return _sliceSet; }
		}

		IDisplaySet IDisplaySet.CreateFreshCopy()
		{
			return (IDisplaySet) CloneBuilder.Clone(this);
		}

		IDisplaySet IDisplaySet.Clone()
		{
			return (IDisplaySet) CloneBuilder.Clone(this);
		}

		private void sliceSet_SliceSopsChanged(object sender, EventArgs e)
		{
			// clear old presentation images
			List<IPresentationImage> images = new List<IPresentationImage>(this.PresentationImages);
			this.PresentationImages.Clear();
			foreach (IPresentationImage image in this.PresentationImages)
				image.Dispose();

			// repopulate with new slices
			this.FillPresentationImages();
		}

		private void FillPresentationImages()
		{
			foreach (MprSliceSop sop in _sliceSet.SliceSops)
			{
				foreach (IPresentationImage image in PresentationImageFactory.Create(sop))
					base.PresentationImages.Add(image);
			}
		}
	}
}