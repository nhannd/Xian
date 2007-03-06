using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A collection of <see cref="SeriesCollection"/> objects.
	/// </summary>
	public class SeriesCollection : ObservableDictionary<string, Series, SeriesEventArgs>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SeriesCollection"/>.
		/// </summary>
		public SeriesCollection()
		{

		}
	}
}
