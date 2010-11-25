#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.UsageTracking;

namespace ClearCanvas.Desktop
{
    static internal class PhoneHome
    {
        #region Private Fields

        static private Timer _phoneHomeTimer;
        static private readonly object _sync = new object();
        static private bool _started;

        #endregion

        #region Public Methods

        /// <summary>
        /// Start up the phone home service.
        /// </summary>
        static internal void Startup()
        {
            lock(_sync)
            {
                OnStartUp();    
            }
            
        }

        /// <summary>
        /// Shut down the phone home service.
        /// </summary>
        static internal void ShutDown()
        {
            lock (_sync)
            {
                try
                {
                    if (_phoneHomeTimer != null)
                    {
                        _phoneHomeTimer.Dispose();
                        _phoneHomeTimer = null;
                    }
                    
                    // Message must be sent using the current thread instead of threadpool when the app is being shut down
                    Thread workerThread =
                        new Thread(ignore => UsageTracking.Register(UsageTracking.GetUsageMessage(), UsageTrackingThread.Current));
                    workerThread.Start();

                    // wait up to 10 seconds, this is a requirement.
                    if (!workerThread.Join(TimeSpan.FromSeconds(10)))
                    {
                        Platform.Log(LogLevel.Debug,
                                     "PhoneHome.ShutDown(): web service does not return within 10 seconds. Continue shutting down.");
                    }
                }catch(Exception ex)
                {
                    // Requirement says log must be in debug
                    Platform.Log(LogLevel.Debug, ex, "Error occurred when shutting down phone home service");
                }
            }
        }

        #endregion

        #region Helpers

        static private void OnStartUp()
        {
            if (!_started)
            {
                try
                {
                    UsageTracking.Register(UsageTracking.GetUsageMessage(), UsageTrackingThread.Background);

                    _phoneHomeTimer = new Timer(ignore =>
                                                UsageTracking.Register(UsageTracking.GetUsageMessage(),UsageTrackingThread.Background),
                                                null, TimeSpan.FromHours(24), TimeSpan.FromHours(24));

                    _started = true;
                }
                catch (Exception ex)
                {
                    // Requirement says log must be in debug
                    Platform.Log(LogLevel.Debug, ex, "Error occurred when starting up phone home service");
                }
            }
        }

        #endregion
    }
}
