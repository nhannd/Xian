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
using System.Linq;
using ClearCanvas.Dicom.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.ServerDirectory.Tests
{
    public class ServerDirectoryTestServiceProvider : IServiceProvider
    {
        public static void Reset()
        {
            TestServerDirectory.Reset();
        }

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (typeof(IServerDirectory) == serviceType)
                return new TestServerDirectory();

            return null;
        }

        #endregion
    }

    public class TestServerDirectory : IServerDirectory
    {
        public static List<ServerDirectoryEntry> Entries;

        static TestServerDirectory()
        {
            Reset();
        }

        public static void Reset()
        {
            Entries = new List<ServerDirectoryEntry>
                           {
                               new ServerDirectoryEntry
                                   {
                                       Server = new ApplicationEntity{Name = "Name1", AETitle = "AE1", ScpParameters = new ScpParameters("localhost", 104)},
                                       IsPriorsServer = true,
                                       Data = new Dictionary<string, object>{{"test1", "value1"}}
                                   },
                               new ServerDirectoryEntry
                                   {
                                       Server = new ApplicationEntity{Name = "Name2", AETitle = "AE2", ScpParameters = null},
                                       IsPriorsServer = false,
                                       Data = new Dictionary<string, object>{{"test2", "value2"}}
                                   },
                           };
        }

        #region IServerDirectory Members

        public GetServersResult GetServers(GetServersRequest request)
        {
            IEnumerable<ServerDirectoryEntry> entries = Entries;
            if (!String.IsNullOrEmpty(request.Name))
                entries = entries.Where(e => e.Server.Name == request.Name);
            else if (!String.IsNullOrEmpty(request.AETitle))
                entries = entries.Where(e => e.Server.AETitle == request.AETitle);

            return new GetServersResult { ServerEntries = entries.ToList() };
        }

        public AddServerResult AddServer(AddServerRequest request)
        {
            Entries.Add(request.ServerEntry);
            return new AddServerResult { ServerEntry = request.ServerEntry };
        }

        public UpdateServerResult UpdateServer(UpdateServerRequest request)
        {
            var index = Entries.FindIndex(e => e.Server.Name == request.ServerEntry.Server.Name);
            Entries[index] = request.ServerEntry;
            return new UpdateServerResult { ServerEntry = request.ServerEntry };
        }

        public DeleteServerResult DeleteServer(DeleteServerRequest request)
        {
            var index = Entries.FindIndex(e => e.Server.Name == request.ServerEntry.Server.Name);
            Entries.RemoveAt(index);
            return new DeleteServerResult();
        }

        #endregion
    }
}

#endif