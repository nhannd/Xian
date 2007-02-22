using System;
namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public interface IStandardStatefulGraphic : IStatefulGraphic
	{
		GraphicState CreateCreateState();
		GraphicState CreateFocussedSelectedState();
		GraphicState CreateFocussedState();
		GraphicState CreateInactiveState();
		GraphicState CreateSelectedState();
	}
}
