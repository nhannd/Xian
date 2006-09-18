using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class SopCollection : ObservableDictionary<string, Sop, SopEventArgs>
	{
		public SopCollection()
		{

		}
	}
}
