#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Dicom.Iod
{
	public interface IApplicationEntity
	{
		string AETitle { get; }
		string Name { get; }
		string Description { get; }
		string Location { get; }
	}

	public interface IDicomServerApplicationEntity : IApplicationEntity
	{
		string HostName { get; }
		int Port { get; }

        // TODO (CR Mar 2012): Try to get rid of this later.
        bool IsStreaming { get; }
    }

	public interface IStreamingServerApplicationEntity : IDicomServerApplicationEntity
	{
		int HeaderServicePort { get; }

		int WadoServicePort { get; }
	}
}
