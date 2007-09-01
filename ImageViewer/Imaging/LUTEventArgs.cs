using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Provides data for the <see cref="LUTCollection"/> events.
	/// </summary>
	public sealed class LutEventArgs : CollectionEventArgs<ILut>
	{
		public LutEventArgs()
		{

		}

		public LutEventArgs(ILut lut)
		{
			Platform.CheckForNullReference(lut, "lut");

			base.Item = lut;
		}

		public ILut Lut { get { return base.Item; } }
	}
}
