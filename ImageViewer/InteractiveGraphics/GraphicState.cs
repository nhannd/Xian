#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A base class for graphic states.
	/// </summary>
	public abstract class GraphicState : IMouseButtonHandler
	{
		#region Private fields

		private IStatefulGraphic _statefulGraphic;
		private PointF _lastPoint;
		private MouseButtonHandlerBehaviour _mousebuttonBehaviour = MouseButtonHandlerBehaviour.None;
		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="GraphicState"/>.
		/// </summary>
		/// <param name="statefulGraphic"></param>
		protected GraphicState(IStatefulGraphic statefulGraphic)
		{
			Platform.CheckForNullReference(statefulGraphic, "statefulGraphic");
			_statefulGraphic = statefulGraphic;
		}

		/// <summary>
		/// The <see cref="IStatefulGraphic"/> associated with this
		/// <see cref="GraphicState"/>.
		/// </summary>
		public IStatefulGraphic StatefulGraphic
		{
			get	{ return _statefulGraphic; }
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

		/// <summary>
		/// Called by the framework each time a mouse button is pressed.
		/// </summary>
		/// <remarks>
		/// As a general rule, if the <see cref="IMouseButtonHandler"/> object did anything as a result of this call, it must 
		/// return true.  If false is returned, <see cref="IMouseButtonHandler.Start"/> is called on other <see cref="IMouseButtonHandler"/>s
		/// until one returns true.
		/// </remarks>
		/// <returns>
		/// True if the <see cref="IMouseButtonHandler"/> did something as a result of the call, 
		/// and hence would like to receive capture.  Otherwise, false.
		/// </returns>
		public virtual bool Start(IMouseInformation mouseInformation)
		{
			return false;
		}

		/// <summary>
		/// Called by the framework when the mouse has moved.
		/// </summary>
		/// <remarks>
		/// A button does not necessarily have to be down for this message to be called.  The framework can
		/// call it any time the mouse moves.
		/// </remarks>
		/// <returns>True if the message was handled, otherwise false.</returns>
		public virtual bool Track(IMouseInformation mouseInformation)
		{
			return false;
		}

		/// <summary>
		/// Called by the framework when the mouse button is released.
		/// </summary>
		/// <returns>
		/// True if the framework should <b>not</b> release capture, otherwise false.
		/// </returns>
		public virtual bool Stop(IMouseInformation mouseInformation)
		{
			return false;
		}

		/// <summary>
		/// Called by the framework to let <see cref="IMouseButtonHandler"/> perform any necessary cleanup 
		/// when capture is going to be forcibly released.
		/// </summary>
		/// <remarks>
		/// It is important that this method is implemented correctly and doesn't simply do nothing when it is inappropriate
		/// to do so, otherwise odd behaviour may be experienced.
		/// </remarks>
		public virtual void Cancel()
		{
		}

		/// <summary>
		/// Allows the <see cref="IMouseButtonHandler"/> to override certain default framework behaviour.
		/// </summary>
		public MouseButtonHandlerBehaviour Behaviour
		{
			get { return _mousebuttonBehaviour; }
			protected set { _mousebuttonBehaviour = value; }
		}

		#endregion
	}
}
