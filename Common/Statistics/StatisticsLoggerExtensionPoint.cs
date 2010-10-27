#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Defines an extension point for statistics logging. Used by <see cref="StatisticsLogger"/>
    /// to create instances of statistics logging listener.
    /// </summary>
    [ExtensionPoint()]
    public class StatisticsLoggerExtensionPoint : ExtensionPoint<IStatisticsLoggerListener>
    {
    }
}