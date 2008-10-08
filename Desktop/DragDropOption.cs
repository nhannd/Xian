using System;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Enumeration for drag and drop actions.
	/// </summary>
	[Flags]
	public enum DragDropOption : int
	{
		/// <summary>
		/// Indicates that no drag and drop interaction is allowed, or that no action has been taken.
		/// </summary>
		None = 0,

		/// <summary>
		/// Indicates that a drag and drop move interaction is allowed, or that a move action was taken.
		/// </summary>
		Move = 1,

		/// <summary>
		/// Indicates that a drag and drop copy interaction is allowed, or that a copy action was taken.
		/// </summary>
		Copy = 2
	}
}