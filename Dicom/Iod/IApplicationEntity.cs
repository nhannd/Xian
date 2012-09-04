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
        string Name { get; }
        string AETitle { get; }
		string Description { get; }
		string Location { get; }

	    // TODO (CR Mar 2012): Unsure about this
        IScpParameters ScpParameters { get; }
        IStreamingParameters StreamingParameters { get; }
    }

    public interface IScpParameters
    {
        string HostName { get; }
        int Port { get; }
    }

    public interface IStreamingParameters
    {
        int HeaderServicePort { get; }
        int WadoServicePort { get; }
    }
}
