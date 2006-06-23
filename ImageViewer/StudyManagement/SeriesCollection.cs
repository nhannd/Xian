using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class SeriesCollection : ObservableDictionary<string, Series, SeriesEventArgs>
	{
		public SeriesCollection()
		{

		}
	}
}
