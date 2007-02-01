using System;
namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public interface IStandardStatefulGraphic : IStatefulGraphic
	{
		GraphicState CreateCreateState();
		GraphicState CreateFocusSelectedState();
		GraphicState CreateFocusState();
		GraphicState CreateInactiveState();
		GraphicState CreateSelectedState();
	}
}
