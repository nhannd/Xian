using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Provides data for the <see cref="LUTCollection"/> events.
	/// </summary>
	public class LutEventArgs : CollectionEventArgs<ILut>
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
