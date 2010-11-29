#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.UsageTracking;
using Timer=System.Threading.Timer;

namespace ClearCanvas.Desktop
{
    static internal class PhoneHome
    {
        #region Private Fields

        static private Timer _phoneHomeTimer;
        static private readonly object _sync = new object();
        static private bool _started;
        static private DateTime _startTimestamp;
        #endregion

        #region Public Methods

        /// <summary>
        /// Start up the phone home service.
        /// </summary>
        static internal void Startup()
        {
            lock (_sync)
            {
                try
                {
                    OnStartUp();

                    var msg = CreateUsageMessage(UsageType.Startup);
                    UsageTracking.Register(msg, UsageTrackingThread.Background);

                    _phoneHomeTimer = new Timer(ignore =>
                                                UsageTracking.Register(UsageTracking.GetUsageMessage(), UsageTrackingThread.Background),
                                                null, TimeSpan.FromHours(24), TimeSpan.FromHours(24));
                }
                catch (Exception ex)
                {
                    // Requirement says log must be in debug
                    Platform.Log(LogLevel.Debug, ex, "Error occurred when shutting down phone home service");
                }
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
                    OnShutdown();
                    
                    TimeSpan uptime = DateTime.Now - _startTimestamp;

                    // Message must be sent using the current thread instead of threadpool when the app is being shut down
                    Thread workerThread =
                        new Thread(ignore =>
                        {
                            try
                            {
                                var msg = CreateUsageMessage(UsageType.Shutdown);
                                msg.AppData = new List<UsageApplicationData>();
                                msg.AppData.Add(new UsageApplicationData
                                                    {Key = "PROCESSUPTIME", Value = String.Format(CultureInfo.InvariantCulture, "{0}", uptime.TotalHours)});
                                UsageTracking.Register(msg, UsageTrackingThread.Current);
                            }
                            catch (Exception ex)
                            {
                                // Requirement says log must be in debug
                                Platform.Log(LogLevel.Debug, ex, "Error occurred when shutting down phone home service");
                            }
                        });
                    workerThread.Start();

                    // wait up to 10 seconds, this is a requirement.
                    if (!workerThread.Join(TimeSpan.FromSeconds(10)))
                    {
                        Platform.Log(LogLevel.Debug,
                                     "PhoneHome.ShutDown(): web service does not return within 10 seconds. Continue shutting down.");
                    }
                }
                catch (Exception ex)
                {
                    // Requirement says log must be in debug
                    Platform.Log(LogLevel.Debug, ex, "Error occurred when shutting down phone home service");
                }
            }
        }

        private static void OnShutdown()
        {
            if (_phoneHomeTimer != null)
            {
                _phoneHomeTimer.Dispose();
                _phoneHomeTimer = null;
            }
        }

        #endregion

        #region Helpers

        static UsageMessage CreateUsageMessage(UsageType type)
        {
            var msg = UsageTracking.GetUsageMessage();
            msg.MessageType = type;
            return msg;
        }


        static private void OnStartUp()
        {
            if (!_started)
            {
                _startTimestamp = DateTime.Now;
                _started = true;
            }
        }

        #endregion
    }

}
