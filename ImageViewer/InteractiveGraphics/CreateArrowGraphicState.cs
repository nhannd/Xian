using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Graphics
{
	internal class CreateArrowGraphicState : CreateGraphicState
	{
		private int _currentControlPointIndex = -1;

		public CreateArrowGraphicState(ArrowInteractiveGraphic interactiveGraphic) : base(interactiveGraphic) {}

		private ArrowInteractiveGraphic InteractiveGraphic
		{
			get { return (ArrowInteractiveGraphic) this.StandardStatefulGraphic; }
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			_currentControlPointIndex++;
			if (_currentControlPointIndex == 0)
			{
				this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;
				this.InteractiveGraphic.ControlPoints[0] = mouseInformation.Location;
				this.InteractiveGraphic.ControlPoints[1] = mouseInformation.Location;
				this.InteractiveGraphic.ResetCoordinateSystem();
			}
			else if (_currentControlPointIndex == 1)
			{
				this.StandardStatefulGraphic.State = this.StandardStatefulGraphic.CreateFocussedSelectedState();
			}
			else
			{
				return false;
			}

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_currentControlPointIndex == 0)
			{
				this.InteractiveGraphic.CoordinateSystem = CoordinateSystem.Destination;
				this.InteractiveGraphic.ControlPoints[_currentControlPointIndex + 1] = mouseInformation.Location;
				this.InteractiveGraphic.ResetCoordinateSystem();
				this.InteractiveGraphic.Draw();
			}

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			return true;
		}

		public override string ToString()
		{
			return "CreateArrowGraphicState";
		}
	}
}