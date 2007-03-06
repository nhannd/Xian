using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Provides data for <see cref="SopCollection"/> events.
	/// </summary>
	public class SopEventArgs : CollectionEventArgs<Sop>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SopEventArgs"/>.
		/// </summary>
		public SopEventArgs()
		{

		}

		/// <summary>
		/// Initializes a new instance of <see cref="SopEventArgs"/> with
		/// a specified <see cref="Sop"/>.
		/// </summary>
		/// <param name="sop"></param>
		public SopEventArgs(Sop sop)
		{
			base.Item  = sop;
		}

		/// <summary>
		/// Gets the <see cref="Sop"/>.
		/// </summary>
		public Sop Sop { get { return base.Item; } }
	}
}
