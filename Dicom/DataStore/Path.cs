using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
	public class Path : DicomTagPath
	{
		#region NHibernate-specific members
		/// <summary>
		/// Mandatory constructor for NHibernate.
		/// </summary>
		public Path()
			: base()
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
