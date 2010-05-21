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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable]
	internal class FusionOverlayColorMapSpec : IDisposable
	{
		private StandardColorMaps _colorMap = StandardColorMaps.HotMetal;
		private bool _hideBackground = false;
		private float _opacity = 0.5f;

		[CloneIgnore]
		private IColorMapManager _overlayColorMapManager;

		public FusionOverlayColorMapSpec() {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected FusionOverlayColorMapSpec(FusionOverlayColorMapSpec source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		public void Dispose()
		{
			_overlayColorMapManager = null;
		}

		public StandardColorMaps ColorMap
		{
			get { return _colorMap; }
			set
			{
				if (_colorMap != value)
				{
					_colorMap = value;
					this.OnColorMapChanged(EventArgs.Empty);
				}
			}
		}

		public float Opacity
		{
			get { return _opacity; }
			set
			{
				Platform.CheckTrue(value >= 0f && value <= 1f, "Opacity must be between 0 and 1.");
				if (_opacity != value)
				{
					_opacity = value;
					this.OnOpacityChanged(EventArgs.Empty);
				}
			}
		}

		public bool HideBackground
		{
			get { return _hideBackground; }
			set
			{
				if (_hideBackground != value)
				{
					_hideBackground = value;
					this.OnHideBackgroundChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnColorMapChanged(EventArgs e)
		{
			this.InstallColorMap();
		}

		protected virtual void OnOpacityChanged(EventArgs e)
		{
			this.InstallColorMap();
		}

		protected virtual void OnHideBackgroundChanged(EventArgs e)
		{
			this.InstallColorMap();
		}

		protected void InstallColorMap()
		{
			if (_overlayColorMapManager != null)
			{
				var overlayColorMapReference = AlphaColorMapFactory.GetColorMap(_colorMap.ToString(), (byte) (byte.MaxValue*_opacity), _hideBackground);
				_overlayColorMapManager.InstallColorMap(overlayColorMapReference);
			}
		}

		public void SetOverlayColorMapManager(IColorMapManager overlayColorMapManager)
		{
			_overlayColorMapManager = overlayColorMapManager;
			this.InstallColorMap();
		}
	}
}