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
            TestDicomServerConfiguration.Reset();
            TestDicomServer.Reset();
        }

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (typeof(IDicomServerConfiguration) == serviceType)
                return new TestDicomServerConfiguration();

            if (typeof(IDicomServer) == serviceType)
                return new TestDicomServer();

            return null;
        }

        #endregion
    }

    public class TestDicomServerConfiguration : IDicomServerConfiguration
    {
        public static DicomServerConfiguration Configuration;

        static TestDicomServerConfiguration()
        {
            Reset();
        }

        public static void Reset()
        {
            Configuration = new DicomServerConfiguration { AETitle = "Local", HostName = "localhost", Port = 104 };
        }

        #region IDicomServerConfiguration Members

        public GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request)
        {
            return new GetDicomServerConfigurationResult
                       {
                           Configuration = Configuration
                       };
        }

        public UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request)
        {
            Configuration = request.Configuration;
            return new UpdateDicomServerConfigurationResult();
        }

        public GetDicomServerExtendedConfigurationResult GetExtendedConfiguration(GetDicomServerExtendedConfigurationRequest request)
        {
            throw new NotImplementedException();
        }

        public UpdateDicomServerExtendedConfigurationResult UpdateExtendedConfiguration(UpdateDicomServerExtendedConfigurationRequest request)
        {
            throw new NotImplementedException();
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