namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Message that is processed when a tile's focus has changed.
	/// </summary>
	public class FocusChangedMessage
	{
		private readonly bool _lost;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public FocusChangedMessage(bool lost)
		{
			_lost = lost;
		}

		/// <summary>
		/// Gets whether or not focus has been lost.
		/// </summary>
		/// <remarks>
		/// If this property returns false, it means that focus has been <b>gained</b>.
		/// </remarks>
		public bool Lost
		{
			get { return _lost; }
		}
	}
}
