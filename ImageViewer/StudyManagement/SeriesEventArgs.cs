using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Provides data for <see cref="SeriesCollection"/> events.
	/// </summary>
	public class SeriesEventArgs : CollectionEventArgs<Series>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SeriesEventArgs"/>.
		/// </summary>
		public SeriesEventArgs()
		{

		}

		/// <summary>
		/// Initializes a new instance of <see cref="SeriesEventArgs"/> with
		/// a specified <see cref="Series"/>.
		/// </summary>
		/// <param name="series"></param>
		public SeriesEventArgs(Series Series)
		{
			base.Item  = Series;
		}

		/// <summary>
		/// Gets the <see cref="Series"/>.
		/// </summary>
		public Series Series { get { return base.Item; } }

	}
}
