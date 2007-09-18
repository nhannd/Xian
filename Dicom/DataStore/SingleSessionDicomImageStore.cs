using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.DataStore
{
    /// <summary>
    /// Assumes that the objects have non-null and valid UIDs.
    /// </summary>
    public class SingleSessionDicomImageStore : DicomImageStoreBase
    {
		protected override IDataStoreReader GetIDataStoreReader()
		{
			return SingleSessionDataAccessLayer.GetIDataStoreReader();
		}

		protected override IDataStoreWriter GetIDataStoreWriter()
		{
			return SingleSessionDataAccessLayer.GetIDataStoreWriter();
		}
		
		protected  override void Flush()
        {
			try
			{
				base.Flush();
			}
			finally
			{
				// TODO:
				// This is a hack. For some reason, when we do a whole succession of Stores,
				// which involves SaveOrUpdate(), Commit(), Flush(), 
				// the next time we try to do a query (on the next image rebuild), the Session
				// seems closed, even though its state declares that it is not closed.
				// The hack is to close the current session after flushing occurs, and then
				// the SingleSessionDataAccessLayer will detect closed sessions, and open a new one
				// when that occurs.
				// Addendum:
				// The hack employed worked, except that subsequent queries on objects that
				// did not exist in the database, on UIDs, would take very very very long.
				// A workaround to this, is to clear the session of domain objects explicitly
				// before closing the session.
				// Addendum 08-Feb-2007:
				// This hack seems necessary for SQL Server CE as well.

				SingleSessionDataAccessLayer.SqliteWorkaround();
			}
        }
    }
}
