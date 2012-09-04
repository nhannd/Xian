#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Statistics;

namespace ClearCanvas.Dicom.Utilities.Command
{
    /// <summary>
    /// Stores the statistics of a <see cref="CommandBase"/>.
    /// </summary>
    public class CommandStatistics : TimeSpanStatistics
    {
        public CommandStatistics(ICommand cmd)
            : base(cmd.Description)
        {
        }
    }
}
