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
using System.Globalization;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.UsageTracking;
using ClearCanvas.Utilities.Manifest;
using Timer=System.Threading.Timer;

namespace ClearCanvas.Desktop
{
    static internal class PhoneHome
    {
        #region Private Fields

        private static readonly TimeSpan _repeat24Hours = TimeSpan.FromHours(24);
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
                    UsageUtilities.Register(msg, UsageTrackingThread.Background);

                    _phoneHomeTimer = new Timer(ignore =>
                                                    {
                                                        msg = CreateUsageMessage(UsageType.Other);
                                                        UsageUtilities.Register(msg, UsageTrackingThread.Background);
                                                    },
                                                null, _repeat24Hours, _repeat24Hours);
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
            // Guard against incorrect use of this class when Startup() is not called.
            if (!_started)
                return;
            
            lock (_sync)
            {
                try
                {
                    OnShutdown();
                    
                    // Note: use a thread to send the message because we don't want to block the app
                    Thread workerThread = new Thread(SendShutdownMessage);
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

        #endregion

        #region Helpers


        private static void SendShutdownMessage()
        {
            const string keyProcessUptime = "PROCESSUPTIME";
            try
            {
                TimeSpan uptime = DateTime.Now - _startTimestamp;

                var msg = CreateUsageMessage(UsageType.Shutdown);
                msg.AppData = new List<UsageApplicationData>();
                msg.AppData.Add(new UsageApplicationData { Key = keyProcessUptime, Value = String.Format(CultureInfo.InvariantCulture, "{0}", uptime.TotalHours) });

                // Message must be sent using the current thread instead of threadpool when the app is being shut down
                UsageUtilities.Register(msg, UsageTrackingThread.Current);
            }
            catch (Exception ex)
            {
                // Requirement says log must be in debug
                Platform.Log(LogLevel.Debug, ex, "Error occurred when shutting down phone home service");
            }
        }

        static UsageMessage CreateUsageMessage(UsageType type)
        {
            var msg = UsageUtilities.GetUsageMessage();
            msg.Certified = ManifestVerification.Valid;
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


        static private void OnShutdown()
        {
            if (_phoneHomeTimer != null)
            {
                _phoneHomeTimer.Dispose();
                _phoneHomeTimer = null;
                _started = false;
            }
        }
        #endregion
    }

}
