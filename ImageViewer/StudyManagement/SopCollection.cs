using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Workstation.Model;

namespace ClearCanvas.Workstation.Model.StudyManagement
{
	public class SopCollection : ObservableDictionary<string, Sop, SopEventArgs>
	{
		public SopCollection()
		{

		}
	}
}
