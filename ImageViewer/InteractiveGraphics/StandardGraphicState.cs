#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Base class for standard graphic states.
	/// </summary>
	public abstract class StandardGraphicState : GraphicState
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StandardGraphicState"/>.
		/// </summary>
		/// <param name="standardStatefulGraphic"></param>
		protected StandardGraphicState(IStandardStatefulGraphic standardStatefulGraphic)
			: base(standardStatefulGraphic)
		{

		}

		/// <summary>
		/// Gets the <see cref="IStandardStatefulGraphic"/> associated with
		/// this state.
		/// </summary>
		protected new IStandardStatefulGraphic StatefulGraphic
		{
			get { return (IStandardStatefulGraphic)base.StatefulGraphic; }
		}
	}
}
