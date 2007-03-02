using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="DisplaySetCollection"/> events.
	/// </summary>
	public class DisplaySetEventArgs : CollectionEventArgs<IDisplaySet>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DisplaySetEventArgs"/>.
		/// </summary>
		public DisplaySetEventArgs()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DisplaySetEventArgs"/> with
		/// a specified <see cref="IDisplaySet"/>.
		/// </summary>
		/// <param name="displaySet"></param>
		public DisplaySetEventArgs(IDisplaySet displaySet)
		{
			base.Item  = displaySet;
		}

		/// <summary>
		/// Gets the <see cref="IDisplaySet"/>
		/// </summary>
		public IDisplaySet DisplaySet { get { return base.Item; } }
	}
}
