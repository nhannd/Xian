using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Provides data for the <see cref="LutCollection"/> events.
	/// </summary>
	public sealed class LutEventArgs : CollectionEventArgs<ILut>
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public LutEventArgs()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="lut">The <see cref="ILut"/> that is the subject of the raised event.</param>
		public LutEventArgs(ILut lut)
			: base()
		{
			Platform.CheckForNullReference(lut, "lut");

			base.Item = lut;
		}

		/// <summary>
		/// Gets or sets the <see cref="ILut"/> that is the subject of the raised event.
		/// </summary>
		public ILut Lut 
		{
			get { return base.Item; }
			set { base.Item = value; }
		}
	}
}
