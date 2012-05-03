#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Configuration.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration
{
	public class PriorsServersConfigurationComponent : ServerTreeConfigurationComponent
	{
		public PriorsServersConfigurationComponent()
			:base(SR.DescriptionPriorsServers, GetPriorsServers())
		{
		}

        private static DicomServiceNodeList GetPriorsServers()
        {
            try
            {
                var priorsServers = ServerDirectory.GetPriorsServers(false);
                return new DicomServiceNodeList(priorsServers);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e, "Error initializing priors servers from directory.");
                return new DicomServiceNodeList();
            }
        }

		public override void Save()
		{
		    try
		    {
		        List<ServerDirectoryEntry> allEntries = null;
		        Platform.GetService<IServerDirectory>(s => allEntries = s.GetServers(new GetServersRequest()).ServerEntries);

                var changedEntries = new List<ServerDirectoryEntry>();

                foreach (var existingEntry in allEntries)
                {
                    var isChecked = CheckedServers.Any(s => s.Name == existingEntry.Server.Name);
                    if (existingEntry.IsPriorsServer == isChecked)
                        continue;

                    existingEntry.IsPriorsServer = isChecked;
                    changedEntries.Add(existingEntry);
                }
                
                Platform.GetService(delegate(IServerDirectory service)
		                                {
                                            foreach (var changedEntry in changedEntries)
                                                service.UpdateServer(new UpdateServerRequest{ServerEntry = changedEntry});
		                                });

		    }
		    catch (Exception e)
		    {
		        ExceptionHandler.Report(e, SR.MessageFailedToSavePriorsServers, Host.DesktopWindow);
		    }
		}
	}
}
