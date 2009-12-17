namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Message object created by the view layer to allow a controlling object <see cref="TileController"/>
	/// to be notified that the <see cref="Tile"/> has lost input focus.
	/// </summary>
	public sealed class LostFocusMessage
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public LostFocusMessage()
		{
		}
	}
}