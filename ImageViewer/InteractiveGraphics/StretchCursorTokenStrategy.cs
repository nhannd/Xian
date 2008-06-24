using System.Drawing;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A strategy for determining what the <see cref="CursorToken"/> should be
	/// for a given target <see cref="InteractiveGraphic"/> (<see cref="TargetGraphic"/>).
	/// </summary>
	[Cloneable(true)]
	public abstract class StretchCursorTokenStrategy : ICursorTokenProvider
	{
		[CloneIgnore]
		private InteractiveGraphic _targetGraphic;

		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected StretchCursorTokenStrategy()
		{
		}

		/// <summary>
		/// The target <see cref="Graphic"/> for which the <see cref="CursorToken"/>
		/// is to be determined.
		/// </summary>
		protected internal InteractiveGraphic TargetGraphic
		{
			get { return _targetGraphic; }
			internal set { _targetGraphic = value; }
		}

		#region ICursorTokenProvider Members

		/// <summary>
		/// Gets the cursor token to be shown at the current mouse position.
		/// </summary>
		public abstract CursorToken GetCursorToken(Point point);

		#endregion
	}
}
