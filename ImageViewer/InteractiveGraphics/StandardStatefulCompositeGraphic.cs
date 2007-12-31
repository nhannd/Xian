using System;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A <see cref="StatefulCompositeGraphic"/> that has factory methods
	/// that create standard graphic states.
	/// </summary>
	/// <remarks>
	/// Factory methods can be overridden so that customized graphic states
	/// can be created.
	/// </remarks>
	public abstract class StandardStatefulCompositeGraphic 
		: StatefulCompositeGraphic, IStandardStatefulGraphic
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StandardStatefulCompositeGraphic"/>.
		/// </summary>
		protected StandardStatefulCompositeGraphic()
		{
			
		}

		#region IStandardStatefulGraphic Members

		/// <summary>
		/// Creates a new instance of <see cref="InactiveGraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateInactiveState()
		{
			return new InactiveGraphicState(this);
		}

		/// <summary>
		/// Creates a new instance of <see cref="FocussedGraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateFocussedState()
		{
			return new FocussedGraphicState(this);
		}

		/// <summary>
		/// Creates a new instance of <see cref="SelectedGraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateSelectedState()
		{
			return new SelectedGraphicState(this);
		}

		/// <summary>
		/// Creates a new instance of <see cref="FocussedSelectedGraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateFocussedSelectedState()
		{
			return new FocussedSelectedGraphicState(this);
		}

		#endregion
	}
}
