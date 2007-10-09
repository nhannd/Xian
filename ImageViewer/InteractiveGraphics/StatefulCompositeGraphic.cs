using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public abstract class StatefulCompositeGraphic 
		: CompositeGraphic, IStatefulGraphic, IMouseButtonHandler, ICursorTokenProvider
	{
		private GraphicStateManager _graphicStateManager = new GraphicStateManager();

		public StatefulCompositeGraphic()
		{
		}

		public GraphicState State
		{
			get { return _graphicStateManager.State; }
			set { _graphicStateManager.State = value; }
		}

		public event EventHandler<GraphicStateChangedEventArgs> StateChanged
		{
			add { _graphicStateManager.StateChanged += value; }
			remove { _graphicStateManager.StateChanged -= value; }
		}

		#region IMouseButtonHandler Members

		public virtual bool Start(IMouseInformation mouseInformation)
		{
			Platform.CheckMemberIsSet(this.State, "State");
			_graphicStateManager.MouseInformation = mouseInformation;
			return this.State.Start(mouseInformation);
		}

		public virtual bool Track(IMouseInformation mouseInformation)
		{
			Platform.CheckMemberIsSet(this.State, "State");
			_graphicStateManager.MouseInformation = mouseInformation;
			return this.State.Track(mouseInformation);
		}

		public virtual bool Stop(IMouseInformation mouseInformation)
		{
			Platform.CheckMemberIsSet(this.State, "State");
			_graphicStateManager.MouseInformation = mouseInformation;
			return this.State.Stop(mouseInformation);
		}

		public virtual void Cancel()
		{
			this.State.Cancel();
		}

		public virtual bool SuppressContextMenu
		{
			get { return false; }
		}

		public virtual bool ConstrainToTile
		{
			get { return false; }
		}

		#endregion

		#region ICursorTokenProvider Members

		public virtual CursorToken GetCursorToken(Point point)
		{
			return null;
		}

		#endregion

		public virtual void InstallDefaultCursors()
		{
		}	
	}
}
