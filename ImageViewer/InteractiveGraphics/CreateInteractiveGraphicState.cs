using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	//TODO: consider how we can do something more like this:
	// StandardStatefulInteractiveGraphic graphic = new StandardStatefulInteractiveGraphic(CreateInteractiveGraphicState state)
	// and make the Create states be the responsible entity for creating the interactive graphic.
	// There is a chicken and egg issue with the create states, and I believe this is the key.

	public abstract class CreateInteractiveGraphicState : CreateGraphicState
	{
		protected CreateInteractiveGraphicState(IStandardStatefulInteractiveGraphic standardStatefulInteractiveGraphic)
			: base(standardStatefulInteractiveGraphic)
		{
		}

		protected InteractiveGraphic InteractiveGraphic
		{
			get { return ((IStandardStatefulInteractiveGraphic)base.StatefulGraphic).InteractiveGraphic; }
		}
	}
}
