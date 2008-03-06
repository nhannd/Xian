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
using System.ComponentModel;
using System.Drawing;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Thumbnails
{
	/// <summary>
	/// Extension point for views onto <see cref="ThumbnailComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ThumbnailComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ThumbnailComponent class.
	/// </summary>
	[AssociateView(typeof(ThumbnailComponentViewExtensionPoint))]
	public class ThumbnailComponent : ApplicationComponent
	{
		private readonly IImageViewer _imageViewer;
		private readonly BindingList<DisplaySetItem> _displaySetItems = new BindingList<DisplaySetItem>();

		/// <summary>
		/// Constructor.
		/// </summary>
		public ThumbnailComponent(IImageViewer imageViewer)
		{
			_imageViewer = imageViewer;
		}

		public IEnumerable<DisplaySetItem> DisplaySetItems
		{
			get { return _displaySetItems; }
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			CreateDisplaySetThumbnails();
			base.Start();
		}

		private void CreateDisplaySetThumbnails()
		{
			ILogicalWorkspace logicalWorkspace = _imageViewer.LogicalWorkspace;

			foreach (IImageSet imageSet in logicalWorkspace.ImageSets)
			{
				foreach (IDisplaySet displaySet in imageSet.DisplaySets)
				{
					IPresentationImage image = GetMiddlePresentationImage(displaySet);
					Bitmap bitmap = image.DrawToBitmap(100, 100);
					DisplaySetItem displaySetItem = new DisplaySetItem(displaySet, bitmap);
					_displaySetItems.Add(displaySetItem);
				}
			}
		}

		private static IPresentationImage GetMiddlePresentationImage(IDisplaySet displaySet)
		{
			int middleImage = displaySet.PresentationImages.Count/2;

			return displaySet.PresentationImages[middleImage].CreateFreshCopy();
		}
	}
}
