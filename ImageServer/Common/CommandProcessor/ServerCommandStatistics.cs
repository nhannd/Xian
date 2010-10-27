#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Common.CommandProcessor
{
	/// <summary>
	/// Stores the statistics of a <see cref="ServerCommand"/>.
	/// </summary>
	public class ServerCommandStatistics:TimeSpanStatistics
	{
		public ServerCommandStatistics(IServerCommand cmd)
			:base(cmd.Description)
		{
            
		}
	}
}