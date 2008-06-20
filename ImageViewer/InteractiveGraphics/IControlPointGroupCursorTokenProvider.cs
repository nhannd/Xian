using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	//TODO: add CursorTokenStrategy and get rid of this

	/// <summary>
	/// An <see cref="ICursorTokenProvider"/> specifically for manipulating
	/// <see cref="ControlPoint"/>s in a <see cref="ControlPointGroup"/>.
	/// </summary>
	public interface IControlPointGroupCursorTokenProvider : ICursorTokenProvider
	{
		/// <summary>
		/// For internal framework use only.
		/// </summary>
		void SetControlPoints(ControlPointGroup controlPoints);
	}
}
