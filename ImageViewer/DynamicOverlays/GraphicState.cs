using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	/// <summary>
	/// Summary description for GraphicState.
	/// </summary>
	public abstract class GraphicState : IUIEventHandler
	{
		private StatefulGraphic _statefulGraphic;
		private bool _supportUndo;
		private UndoableCommand _command;
		private PointF _lastPoint;

		protected GraphicState(StatefulGraphic statefulGraphic)
		{
			Platform.CheckForNullReference(statefulGraphic, "statefulGraphic");
			_statefulGraphic = statefulGraphic;

			if (_statefulGraphic is IMemorable)
				_supportUndo = true;
			else
				_supportUndo = false;
		}

		// Allow clients to turn off undo support if necessary
		public bool SupportUndo
		{
			get { return _supportUndo; }
			set 
			{
				// If we're going to support undo, StatefulGraphic
				// must implement IMemorable
				if (value && !(this.StatefulGraphic is IMemorable))
					throw new InvalidOperationException("For undo support, your StatefulGraphic subclass must implement IMemorable");

				_supportUndo = value; 
			}
		}

		protected StatefulGraphic StatefulGraphic
		{
			get	{ return _statefulGraphic; }
		}

		protected UndoableCommand Command
		{
			get { return _command; }
			set	
			{
				Platform.CheckForNullReference(value, "Command");
				_command = value;
			}
		}

		protected PointF LastPoint
		{
			get { return _lastPoint; }
			set { _lastPoint = value; }
		}

		#region IUIEventHandler Members

		public virtual bool OnMouseDown(XMouseEventArgs e)
		{
			return false;
		}

		public virtual bool OnMouseMove(XMouseEventArgs e)
		{
			return false;
		}

		public virtual bool OnMouseUp(XMouseEventArgs e)
		{
			return false;
		}

		public virtual bool OnMouseWheel(XMouseEventArgs e)
		{
			return false;
		}

		public virtual bool OnKeyDown(XKeyEventArgs e)
		{
			return false;
		}

		public virtual bool OnKeyUp(XKeyEventArgs e)
		{
			return false;
		}

		#endregion

		public virtual void OnEnterState(XMouseEventArgs e)
		{
		}

		public virtual void OnExitState(XMouseEventArgs e)
		{
		}
	}
}
