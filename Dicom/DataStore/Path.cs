using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
	public class Path : DicomTagPath
	{
		#region NHibernate-specific members
		/// <summary>
		/// Constructor for NHibernate.
		/// </summary>
		private Path()
		{
		}


		/// <summary>
		/// Property for NHibernate.
		/// </summary>
		protected virtual string InternalPath
		{
			get { return base.Path; }
			set { base.Path = value; }
		}

		#endregion
	}
}
