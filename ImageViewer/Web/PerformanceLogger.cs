#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Diagnostics;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Web.Common;
using ClearCanvas.Web.Services;

namespace ClearCanvas.ImageViewer.Web
{
	// TODO: Rewrite this class because
	// When hosted in IIS, the service must have special permissions to access the system performance log
    [ExtensionOf(typeof(PerformanceMonitorExtensionPoint), Enabled=false)]
    public class PerformanceLogger : IPerformanceLogger
    {
        private static Timer _performanceLoggerTimer;

        private const string CLIENT_STACKING_SPEED = "CLIENT_STACKING_SPEED";

        private PerformanceCounter _memoryUsageCounter;
        private PerformanceCounter _speedCounter;

        public void Initialize()
        {

            InitializeCounterCategories();

            // Initialize the counters for the categories
            _memoryUsageCounter = new PerformanceCounter();
            _memoryUsageCounter.CategoryName = "ImageServer";
            _memoryUsageCounter.CounterName = "MemoryUsage";
            _memoryUsageCounter.MachineName = ".";
            _memoryUsageCounter.ReadOnly = false;

            _speedCounter = new PerformanceCounter();
            _speedCounter.CategoryName = "ImageServerWebViewers";
            _speedCounter.CounterName = "Stacking FPS";
            _speedCounter.MachineName = ".";
            _speedCounter.InstanceName = "localhost";
            _speedCounter.ReadOnly = false;

            _performanceLoggerTimer = new Timer(LogMemoryUsage, null, 1000, 1000);

        }

        private static void InitializeCounterCategories()
        {
            if (PerformanceCounterCategory.Exists("ImageServer"))
            {
                PerformanceCounterCategory.Delete("ImageServer");
            }

            CounterCreationDataCollection counters = new CounterCreationDataCollection();
            CounterCreationData memoryUsage = new CounterCreationData();
            memoryUsage.CounterName = "MemoryUsage";
            memoryUsage.CounterHelp = "Total Memory Usage used by ImageServer (MB)";
            memoryUsage.CounterType = PerformanceCounterType.NumberOfItems64;
            counters.Add(memoryUsage);

            PerformanceCounterCategory.Create("ImageServer", "ImageServer performance counters", PerformanceCounterCategoryType.SingleInstance, counters);

            if (PerformanceCounterCategory.Exists("ImageServerWebViewers"))
            {
                PerformanceCounterCategory.Delete("ImageServerWebViewers");
            }

            counters = new CounterCreationDataCollection();
            CounterCreationData fps = new CounterCreationData();
            fps.CounterName = "Stacking FPS";
            fps.CounterHelp = "Client Stacking Speed";
            fps.CounterType = PerformanceCounterType.NumberOfItems64;
            counters.Add(fps);

            PerformanceCounterCategory.Create("ImageServerWebViewers", "ImageServer web viewer client performance counters", PerformanceCounterCategoryType.MultiInstance, counters);
        }

        private void LogMemoryUsage(object state)
        {
            Process currentProc = Process.GetCurrentProcess();
            long physicalMemoryUsed = currentProc.WorkingSet64;
            _memoryUsageCounter.RawValue = physicalMemoryUsed / 1024 / 1024;
        }

        public void Report(string clientIp, PerformanceData data)
        {
            try
            {
                if (data.Name == CLIENT_STACKING_SPEED)
                {
                    _speedCounter.InstanceName = clientIp;
                    _speedCounter.RawValue = (int)data.Value;
                }
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex);
            }
        }
    }
}