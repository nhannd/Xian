using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Iod
{
	public interface IApplicationEntity
	{
		string AETitle { get; }
		string Description { get; }
		string Location { get; }
	}

	public interface IDicomServerApplicationEntity : IApplicationEntity
	{
		string HostName { get; }
		int Port { get; }
	}

	public interface IStreamingServerApplicationEntity : IDicomServerApplicationEntity
	{
		int HeaderServicePort { get; }

		int WadoServicePort { get; }
	}
}
