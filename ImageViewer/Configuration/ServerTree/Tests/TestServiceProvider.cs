#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using System;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.ServerDirectory;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree.Tests
{
    internal class TestDicomServerConfiguration : IDicomServerConfiguration
    {
        #region IDicomServerConfiguration Members

        public GetDicomServerConfigurationResult GetConfiguration(GetDicomServerConfigurationRequest request)
        {
            return new GetDicomServerConfigurationResult
                       {
                           Configuration = new DicomServerConfiguration
                                               {
                                                   AETitle = "Test",
                                                   HostName = "localhost",
                                                   Port = 103,
                                                   FileStoreDirectory = @"C:\filestore"
                                               }
                       };
        }

        public UpdateDicomServerConfigurationResult UpdateConfiguration(UpdateDicomServerConfigurationRequest request)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal class TestServerDirectory : IServerDirectory
    {
        #region IServerDirectory Members

        public GetServersResult GetServers(GetServersRequest request)
        {
            return new GetServersResult();
        }

        public AddServerResult AddServer(AddServerRequest request)
        {
            return new AddServerResult();
        }

        public UpdateServerResult UpdateServer(UpdateServerRequest request)
        {
            return new UpdateServerResult();
        }

        public DeleteServerResult DeleteServer(DeleteServerRequest request)
        {
            return new DeleteServerResult();
        }

        public DeleteAllServersResult DeleteAllServers(DeleteAllServersRequest request)
        {
            return new DeleteAllServersResult();
        }

        #endregion
    }

    internal class TestServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDicomServerConfiguration))
                return new TestDicomServerConfiguration();

            if (serviceType == typeof(IServerDirectory))
                return new TestServerDirectory();
            return null;
        }

        #endregion
    }
}

#endif