#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using System.ServiceModel;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders.Tests
{
    [TestFixture]
    public class ServerDirectoryTests
    {
        public void DeleteAllServers()
        {
            var directory = new ServerDirectory();
            directory.DeleteAllServers(new DeleteAllServersRequest());
        }

        [Test]
        public void TestAddServer()
        {
            DeleteAllServers();

            var directory = new ServerDirectory();

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

            var directory = new ServerDirectory();

            var server = CreateServer("test", true);
            directory.AddServer(new AddServerRequest { Server = server });

            server.AETitle = "ae2";
            server.HostName = "host2";
            server.Port = 100;
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

            var directory = new ServerDirectory();

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

            var directory = new ServerDirectory();

            var server = CreateServer("test", true);
            directory.AddServer(new AddServerRequest { Server = server });

            directory.AddServer(new AddServerRequest { Server = server });
        }

        [Test]
        [ExpectedException(typeof(FaultException<ServerNotFoundFault>))]
        public void TestUpdateServerDoesNotExist()
        {
            DeleteAllServers();

            var directory = new ServerDirectory();

            var server = CreateServer("test", true);
            directory.UpdateServer(new UpdateServerRequest { Server = server });
        }

        [Test]
        [ExpectedException(typeof(FaultException<ServerNotFoundFault>))]
        public void TestDeleteServerDoesNotExist()
        {
            DeleteAllServers();

            var directory = new ServerDirectory();

            var server = CreateServer("test", true);
            directory.DeleteServer(new DeleteServerRequest { Server = server });
        }

        private DicomServerApplicationEntity CreateServer(string name, bool streaming)
        {
            if (streaming)
            return new StreamingServerApplicationEntity
            {
                Name = name,
                AETitle = "ae",
                HostName = "host",
                Port = 104,
                HeaderServicePort = 100,
                WadoServicePort = 100,
                Description = "Some server",
                Location = "Room 101"
            };

            return new DicomServerApplicationEntity
            {
                Name = name,
                AETitle = "ae",
                HostName = "host",
                Port = 104,
                Description = "Some server",
                Location = "Room 101"
            };
        }
    }
}

#endif