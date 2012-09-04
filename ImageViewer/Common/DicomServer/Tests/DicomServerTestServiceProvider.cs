#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

#if UNIT_TESTS

namespace ClearCanvas.ImageViewer.Common.DicomServer.Tests
{
    public class DicomServerTestServiceProvider : IServiceProvider
    {
        public static void Reset()
        {
            TestDicomServer.Reset();
        }

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (typeof(IDicomServerConfiguration) == serviceType)
                return new DicomServerConfigurationProxy();

            if (typeof(IDicomServer) == serviceType)
                return new TestDicomServer();

            return null;
        }

        #endregion
    }

    public class TestDicomServer : IDicomServer
    {
        public static ServiceStateEnum ServiceState;

        static TestDicomServer()
        {
            Reset();
        }

        public static void Reset()
        {
            ServiceState = ServiceStateEnum.Stopped;
        }

        #region IDicomServer Members

        public GetListenerStateResult GetListenerState(GetListenerStateRequest request)
        {
            return new GetListenerStateResult {State = ServiceState};
        }

        public RestartListenerResult RestartListener(RestartListenerRequest request)
        {
            ServiceState = ServiceStateEnum.Started;
            return new RestartListenerResult();
        }

        #endregion
    }
}

#endif