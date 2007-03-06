using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A collection of <see cref="Study"/> objects.
	/// </summary>
	public class StudyCollection : ObservableDictionary<string, Study, StudyEventArgs>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StudyCollection"/>.
		/// </summary>
		public StudyCollection()
		{

		}
	}
}
