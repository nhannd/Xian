using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	// TODO: Support for undo shouldn't be coupled with the state

	/// <summary>
	/// A base class for graphic states.
	/// </summary>
	public abstract class GraphicState : IMouseButtonHandler
	{
		#region Private fields

		private IStatefulGraphic _statefulGraphic;
		private bool _supportUndo;
		private UndoableCommand _command;
		private PointF _lastPoint;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="GraphicState"/>.
		/// </summary>
		/// <param name="statefulGraphic"></param>
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

		/// <summary>
		/// The <see cref="IStatefulGraphic"/> associated with this
		/// <see cref="GraphicState"/>.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the last location of the mouse.
		/// </summary>
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
