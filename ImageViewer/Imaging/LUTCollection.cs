using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A collection of <see cref="IComposableLUT"/> objects.
	/// </summary>
	public class LUTCollection : ObservableList<IComposableLUT, LUTEventArgs>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="LUTCollection"/>.
		/// </summary>
		public LUTCollection()
		{

		}

		//protected override void OnItemAdded(LUTEventArgs e)
		//{
		//    if (e.Index > 0)
		//    {
		//        e.Item.MinInputValue = this[e.Index - 1].MinOutputValue;
		//        e.Item.MaxInputValue = this[e.Index - 1].MaxOutputValue;
		//    }

		//    if (e.Index < this.Count - 1)
		//    {
		//        this[e.Index + 1].MinInputValue = e.Item.MinOutputValue;
		//        this[e.Index + 1].MaxInputValue = e.Item.MaxOutputValue;
		//    }

		//    base.OnItemAdded(e);
		//}
	}
}
