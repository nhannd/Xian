using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
	public interface IDataStoreCleaner
	{
		/// <summary>
		/// This method will clear all studies (and related series and sop instances) from the datastore
		/// *without* deleting the corresponding files from the filestore.
		/// </summary>
		void ClearAllStudies();
	}
}
