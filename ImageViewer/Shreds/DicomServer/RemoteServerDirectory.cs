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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.ServerTree;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	public static class RemoteServerDirectory
	{
		public static AEInformation Lookup(string aeTitle)
		{
			aeTitle = aeTitle.Trim();

			try
			{
				ServerTree serverTree = new ServerTree();
				List<IServerTreeNode> servers = serverTree.FindChildServers(serverTree.RootNode.ServerGroupNode);

				Common.ServerTree.Server server = servers.Find(delegate(IServerTreeNode node)
												{
													return ((Common.ServerTree.Server)node).AETitle == aeTitle;
												}) as Common.ServerTree.Server;

				if (server != null)
				{
					AEInformation information = new AEInformation();
					information.AETitle = aeTitle;
					information.HostName = server.Host;
					information.Port = server.Port;
					return information;
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}

			return null;
		}
	}
}
