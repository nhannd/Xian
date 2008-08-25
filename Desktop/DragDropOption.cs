using System;

namespace ClearCanvas.Desktop
{
	[Flags]
	public enum DragDropOption : int
	{
		None = 0,
		Move = 1,
		Copy = 2
	}
}