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
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public sealed class GraphicEventArgs : EventArgs
	{
		public readonly IGraphic Graphic;

		public GraphicEventArgs(IGraphic graphic)
		{
			this.Graphic = graphic;
		}
	}

	public abstract class InteractiveGraphicBuilder : IMouseButtonHandler, ICursorTokenProvider
	{
		private event EventHandler<GraphicEventArgs> _graphicComplete;
		private event EventHandler<GraphicEventArgs> _graphicCancelled;
		private static readonly CursorToken _crossCursorToken = new CursorToken(CursorToken.SystemCursors.Cross);
		private readonly IGraphic _graphic;

		protected InteractiveGraphicBuilder(IGraphic graphic)
		{
			_graphic = graphic;
		}

		public IGraphic Graphic
		{
			get { return _graphic; }
		}

		public event EventHandler<GraphicEventArgs> GraphicComplete
		{
			add { _graphicComplete += value; }
			remove { _graphicComplete -= value; }
		}

		public event EventHandler<GraphicEventArgs> GraphicCancelled
		{
			add { _graphicCancelled += value; }
			remove { _graphicCancelled -= value; }
		}

		protected virtual void NotifyGraphicComplete()
		{
			this.OnGraphicComplete();
			EventsHelper.Fire(_graphicComplete, this, new GraphicEventArgs(_graphic));
		}

		protected virtual void NotifyGraphicCancelled()
		{
			this.OnGraphicCancelled();
			EventsHelper.Fire(_graphicCancelled, this, new GraphicEventArgs(_graphic));
		}

		protected virtual void OnGraphicComplete() {}
		protected virtual void OnGraphicCancelled() {}

		public virtual bool Start(IMouseInformation mouseInformation)
		{
			return false;
		}

		public virtual bool Track(IMouseInformation mouseInformation)
		{
			return false;
		}

		public virtual bool Stop(IMouseInformation mouseInformation)
		{
			return false;
		}

		public virtual void Cancel()
		{
			this.NotifyGraphicCancelled();
		}

		public virtual void Reset() {}

		public virtual MouseButtonHandlerBehaviour Behaviour
		{
			get { return MouseButtonHandlerBehaviour.None; }
		}

		public virtual CursorToken GetCursorToken(Point point)
		{
			return _crossCursorToken;
		}
	}
}