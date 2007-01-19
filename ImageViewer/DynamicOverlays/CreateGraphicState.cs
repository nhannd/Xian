using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	/// <summary>
	/// An abstract class that all 'create graphic' states should derive from.  This class
	/// only exists for ease of implementation so that a base class can easily determine if
	/// it is in a create state without forcing the derived class to override a property.
	/// </summary>
	public abstract class CreateGraphicState : GraphicState
	{
		protected CreateGraphicState(StatefulGraphic statefulGraphic)
			: base(statefulGraphic)
		{ 
		}
	}
}
