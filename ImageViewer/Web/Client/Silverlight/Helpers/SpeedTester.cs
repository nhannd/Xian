#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion


using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Views;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers
{
    class SpeedTestResult
    {
        public double SpeedInMbps { get; set; }
        public Exception Error { get; set; }
    }

    class SpeedTestCompletedEventArgs : EventArgs
    {
        public SpeedTestResult Result { get; set; }
    }

    internal class ConnectionTester
    {
        const string TinyFile = "Test/speedtest_tiny.bin";
        const string SmallFile = "Test/speedtest_small.bin";
        const string BigFile = "Test/speedtest_big.bin";

        private long _speedTestStartTime;

        public event EventHandler<SpeedTestCompletedEventArgs> SpeedTestCompleted;
        public event EventHandler SpeedTestBegan;

        

        private ConnectionTester()
        {
        }

        public static void StartAsync(Action<SpeedTestResult> actionWhenCompleted)
        {
            ChildWindow msgBox = PopupHelper.PopupMessage("Initializing", "Checking connection speed...");

            ConnectionTester test = new ConnectionTester();
            test.SpeedTestCompleted += (s, ev) =>
            {
                UIThread.Execute(() =>
                {
                    msgBox.Close();
                    var result = ev.Result;
                    actionWhenCompleted(result);
                });

            };

            test.RunSpeedTest();
        }

        public void RunSpeedTest()
        {
            QuickTest();
        }

        private void QuickTest()
        {
            string uri = string.Format("../{0}?rand={1}{2}", TinyFile, Guid.NewGuid().ToString(), Environment.TickCount);

            WebClient client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(QuickTestCompleted);

            if (SpeedTestBegan != null)
            {
                SpeedTestBegan(this, EventArgs.Empty);
            }

            _speedTestStartTime = Environment.TickCount;
            client.OpenReadAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        private void SmallTest()
        {
            string uri = string.Format("../{0}?rand={1}{2}", SmallFile, Guid.NewGuid().ToString(), Environment.TickCount);

            WebClient client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(SmallTestCompleted);

            if (SpeedTestBegan != null)
            {
                SpeedTestBegan(this, EventArgs.Empty);
            }

            _speedTestStartTime = Environment.TickCount;
            client.OpenReadAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        private void BigTest()
        {
            string uri = string.Format("../{0}?rand={1}{2}", BigFile, Guid.NewGuid().ToString(), Environment.TickCount);

            WebClient client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(BigTestCompleted);

            if (SpeedTestBegan != null)
            {
                SpeedTestBegan(this, EventArgs.Empty);
            }

            _speedTestStartTime = Environment.TickCount;
            client.OpenReadAsync(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        private void QuickTestCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                byte[] buffer = new byte[e.Result.Length];
                e.Result.Read(buffer, 0, buffer.Length);
                long elapsed = Environment.TickCount - _speedTestStartTime;
                float sizeMb = buffer.Length / 1024f / 1024f * 8;
                double speedMpbs = sizeMb * 1000f / elapsed;

                // Test bigger file is speed seems ok
                if (speedMpbs > 2)
                {
                    SmallTest();
                }
                else
                {
                    // TODO: Take average?

                    PerformanceMonitor.CurrentInstance.SpeedInMbps = speedMpbs;

                    if (SpeedTestCompleted != null)
                    {
                        SpeedTestCompleted(this, new SpeedTestCompletedEventArgs { Result = new SpeedTestResult { SpeedInMbps = speedMpbs } });
                    }
                }
            }
            else
            {
                if (SpeedTestCompleted != null)
                {
                    SpeedTestCompleted(this, new SpeedTestCompletedEventArgs { Result = new SpeedTestResult { Error = e.Error } });
                }
            }

        }


        private void BigTestCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                byte[] buffer = new byte[e.Result.Length];
                e.Result.Read(buffer, 0, buffer.Length);
                long elapsed = Environment.TickCount - _speedTestStartTime;
                float sizeMb = buffer.Length / 1024f / 1024f * 8;
                double speedMpbs = sizeMb * 1000f / elapsed;

                // TODO: Take average?
                PerformanceMonitor.CurrentInstance.SpeedInMbps = speedMpbs;

                if (SpeedTestCompleted != null)
                {
                    SpeedTestCompleted(this, new SpeedTestCompletedEventArgs { Result = new SpeedTestResult { SpeedInMbps = speedMpbs } });
                }
            }
            else
            {
                if (SpeedTestCompleted != null)
                {
                    SpeedTestCompleted(this, new SpeedTestCompletedEventArgs { Result = new SpeedTestResult { Error = e.Error } });
                }
            }


        }

        private void SmallTestCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                byte[] buffer = new byte[e.Result.Length];
                e.Result.Read(buffer, 0, buffer.Length);
                long elapsed = Environment.TickCount - _speedTestStartTime;
                float sizeMb = buffer.Length / 1024f / 1024f * 8;
                double speedMpbs = sizeMb * 1000f / elapsed;

                // Test bigger file is speed seems ok
                if (speedMpbs > 5)
                {
                    BigTest();
                }
                else
                {
                    // TODO: Take average?

                    PerformanceMonitor.CurrentInstance.SpeedInMbps = speedMpbs;

                    if (SpeedTestCompleted != null)
                    {
                        SpeedTestCompleted(this, new SpeedTestCompletedEventArgs { Result = new SpeedTestResult { SpeedInMbps = speedMpbs } });
                    }
                }
            }
            else
            {
                if (SpeedTestCompleted != null)
                {
                    SpeedTestCompleted(this, new SpeedTestCompletedEventArgs { Result = new SpeedTestResult { Error = e.Error } });
                }
            }

            
        }

    }
}
