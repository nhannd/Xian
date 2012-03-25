#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.ServerDirectory;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	public static class RemoteServerDirectory
	{
        public static IDicomServiceNode Lookup(string aeTitle)
		{
			aeTitle = aeTitle.Trim();

			try
			{
                using (var bridge = new ServerDirectoryBridge())
                    return bridge.GetServersByAETitle(aeTitle).FirstOrDefault();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}

			return null;
		}
	}
}
