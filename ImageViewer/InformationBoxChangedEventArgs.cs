using System;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="Tile.InformationBoxChanged"/> event.
	/// </summary>
	public class InformationBoxChangedEventArgs : EventArgs
	{
		private InformationBox _informationBox;

		internal InformationBoxChangedEventArgs(InformationBox informationBox)
		{
			_informationBox = informationBox;
		}

		/// <summary>
		/// Gets the <see cref="InformationBox"/> that has changed.
		/// </summary>
		public InformationBox InformationBox 
		{
			get { return _informationBox; }
		}
	}
}
