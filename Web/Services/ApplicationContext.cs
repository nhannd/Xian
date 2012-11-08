#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Security.Principal;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Web.Common;

namespace ClearCanvas.Web.Services
{
    public interface IApplicationContext
    {
        EntityHandlerStore EntityHandlers { get; }
        IPrincipal Principal { get; }
        Guid ApplicationId { get; }
        bool BlobsSupported { get; }
        bool IsStatisticsLoggingEnabled { get; }

        void LogStatistics(StatisticsSet statistics);
        void LogStatistics(string logName, StatisticsSet statistics);

        void FireEvent(Event e);
        void FatalError(Exception e);
    }

    public abstract class ApplicationContext : IApplicationContext
    {
        public static IApplicationContext Current
        {
            get
            {
                Application application = Application.Current;
                return application != null ? application.Context : null;
            }
        }

        public abstract EntityHandlerStore EntityHandlers { get; }
        public abstract IPrincipal Principal { get; }
        public abstract Guid ApplicationId { get; }
        public abstract bool BlobsSupported { get; }
        public abstract bool IsStatisticsLoggingEnabled { get; }
        
        public abstract void LogStatistics(StatisticsSet statistics);
        public abstract void LogStatistics(string logName, StatisticsSet statistics);
        
        public abstract void FireEvent(Event e);
        public abstract void FatalError(Exception e);
    }
}