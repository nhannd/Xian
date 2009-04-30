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

//#define USE_BITMAP

using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Rendering
{
	#region BufferedGraphics Method

	internal sealed class BackBuffer : IDisposable
	{
		private IntPtr _contextID;
		private Rectangle _clientRectangle;

		private BufferedGraphicsContext _graphicsContext;
		private BufferedGraphics _bufferedGraphics;

		public BackBuffer()
		{
		}

		public IntPtr ContextID
		{
			get { return _contextID; }
			set { _contextID = value; }
		}

		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
			set
			{
				if (_clientRectangle == value)
					return;

				_clientRectangle = value;
				DisposeBuffer();
			}
		}

		public System.Drawing.Graphics Graphics
		{
			get
			{
				if (BufferedGraphics != null)
					return BufferedGraphics.Graphics;

				return null;
			}
		}

		private BufferedGraphics BufferedGraphics
		{
			get
			{
				if (_bufferedGraphics == null && !IsClientRectangleEmpty && _contextID != IntPtr.Zero)
				{
					Context.MaximumBuffer = GetMaximumBufferSize();
					_bufferedGraphics = Context.Allocate(_contextID, _clientRectangle);
				}

				return _bufferedGraphics;
			}
		}

		private BufferedGraphicsContext Context
		{
			get
			{
				if (_graphicsContext == null)
					_graphicsContext = new BufferedGraphicsContext();

				return _graphicsContext;
			}
		}

		private bool IsClientRectangleEmpty
		{
			get { return _clientRectangle.Width == 0 || _clientRectangle.Height == 0; }
		}

		public void RenderImage(ImageBuffer imageBuffer)
		{
			RenderImage(imageBuffer.Bitmap);
		}

		public void RenderImage(Image image)
		{
			Graphics.DrawImageUnscaled(image, 0, 0);
		}

		public void RenderToScreen()
		{
			if (_bufferedGraphics != null && !IsClientRectangleEmpty)
				_bufferedGraphics.Render(_contextID);
		}

		private Size GetMaximumBufferSize()
		{
			return new Size(_clientRectangle.Width + 1, _clientRectangle.Height + 1);
		}

		private void DisposeBuffer()
		{
			if (_graphicsContext != null)
				_graphicsContext.Invalidate();

			if (_bufferedGraphics != null)
			{
				_bufferedGraphics.Dispose();
				_bufferedGraphics = null;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			DisposeBuffer();

			if (_graphicsContext != null)
			{
				_graphicsContext.Dispose();
				_graphicsContext = null;
			}
		}

		#endregion
	}

	#endregion
}

