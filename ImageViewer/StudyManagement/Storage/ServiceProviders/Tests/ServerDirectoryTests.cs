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
using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders.Tests
{
    [TestFixture]
    public class ServerDirectoryTests
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            var extensionFactory = new UnitTestExtensionFactory(new Dictionary<Type, Type>
                                                                    {
                                                                        { typeof(ServiceProviderExtensionPoint), typeof(ServerDirectoryServiceProvider) }  
                                                                    });
            Platform.SetExtensionFactory(extensionFactory);
        }

        public void DeleteAllServers()
        {
            var directory = Platform.GetService<IServerDirectory>();
            directory.DeleteAllServers(new DeleteAllServersRequest());
        }

        [Test]
        public void TestAddServer()
        {
            DeleteAllServers();

            var directory = Platform.GetService<IServerDirectory>();

            var server = CreateServer("streaming", true);
            directory.AddServer(new AddServerRequest { Server = server });
            var servers = directory.GetServers(new GetServersRequest()).Servers;
            Assert.AreEqual(1, servers.Count);

            server = CreateServer("normal", false);
            directory.AddServer(new AddServerRequest { Server = server });

            servers = directory.GetServers(new GetServersRequest()).Servers;
            Assert.AreEqual(2, servers.Count);

            servers = directory.GetServers(new GetServersRequest{Name = "normal"}).Servers;
            Assert.AreEqual(1, servers.Count);

            Assert.AreEqual(server, servers[0]);
        }

        [Test]
        public void TestUpdateServer()
        {
            DeleteAllServers();

            var directory = Platform.GetService<IServerDirectory>();

            var server = CreateServer("test", true);
            directory.AddServer(new AddServerRequest { Server = server });

            server.AETitle = "ae2";
            server.ScpParameters = new ScpParameters("host2", 100);
            server.Description = "blah";
            server.Location = "blah";

            directory.UpdateServer(new UpdateServerRequest { Server = server });
            var servers = directory.GetServers(new GetServersRequest()).Servers;
            Assert.AreEqual(1, servers.Count);

            Assert.AreEqual(server, servers[0]);
        }

        [Test]
        public void TestDeleteServer()
        {
            DeleteAllServers();

            var directory = Platform.GetService<IServerDirectory>();

            var server = CreateServer("test", true);
            directory.AddServer(new AddServerRequest { Server = server });
            directory.DeleteServer(new DeleteServerRequest { Server = server });

            var servers = directory.GetServers(new GetServersRequest()).Servers;
            Assert.AreEqual(0, servers.Count);
        }

        [Test]
        [ExpectedException(typeof(FaultException<ServerExistsFault>))]
        public void TestAddServerAlreadyExists()
        {
            DeleteAllServers();

            var directory = Platform.GetService<IServerDirectory>();

            var server = CreateServer("test", true);
            directory.AddServer(new AddServerRequest { Server = server });

            directory.AddServer(new AddServerRequest { Server = server });
        }

        [Test]
        [ExpectedException(typeof(FaultException<ServerNotFoundFault>))]
        public void TestUpdateServerDoesNotExist()
        {
            DeleteAllServers();

            var directory = Platform.GetService<IServerDirectory>();

            var server = CreateServer("test", true);
            directory.UpdateServer(new UpdateServerRequest { Server = server });
        }

        [Test]
        [ExpectedException(typeof(FaultException<ServerNotFoundFault>))]
        public void TestDeleteServerDoesNotExist()
        {
            DeleteAllServers();

            var directory = Platform.GetService<IServerDirectory>();

            var server = CreateServer("test", true);
            directory.DeleteServer(new DeleteServerRequest { Server = server });
        }

        private ApplicationEntity CreateServer(string name, bool streaming)
        {
            var ae = new ApplicationEntity
            {
                Name = name,
                AETitle = "ae",
                ScpParameters = new ScpParameters{HostName = "host", Port = 104},
                Description = "Some server",
                Location = "Room 101"
            };

            if (streaming)
                ae.StreamingParameters = new StreamingParameters(50221, 1000);
            return ae;
        }
    }
}

#endif