using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.DataStore
{
	[Serializable]
	public sealed class DataStoreException : Exception
	{
		internal DataStoreException(string message)
			: base(message)
		{
		}

		internal DataStoreException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
