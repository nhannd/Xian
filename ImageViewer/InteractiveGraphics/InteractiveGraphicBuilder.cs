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