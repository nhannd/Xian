using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A collection of <see cref="ILut"/> objects.
	/// </summary>
	public sealed class LutCollection : ObservableList<ILut, LutEventArgs>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="LutCollection"/>.
		/// </summary>
		public LutCollection()
		{
		}
	}
}
