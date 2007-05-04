using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public abstract class GraphicState : IMouseButtonHandler
	{
		private IStatefulGraphic _statefulGraphic;
		private bool _supportUndo;
		private UndoableCommand _command;
		private PointF _lastPoint;

		protected GraphicState(IStatefulGraphic statefulGraphic)
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
					throw new InvalidOperationException(SR.ExceptionStatefulGraphicMustBeIMemorable);

				_supportUndo = value; 
			}
		}

		public IStatefulGraphic StatefulGraphic
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

		#region IMouseButtonHandler Members

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
	}
}
