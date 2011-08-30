#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers;
using ClearCanvas.Web.Client.Silverlight.Utilities;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Controls
{
    public partial class StatisticsPanel
    {
        public StatisticsPanel()
        {
            InitializeComponent();

            DataContext = PerformanceMonitor.CurrentInstance;

            PerformanceMonitor.CurrentInstance.PropertyChanged += InstancePropertyChanged;
            Speed.Text = PerformanceMonitor.CurrentInstance.SpeedInMbps > 0 ? String.Format(" : {0:0.0} Mpbs", PerformanceMonitor.CurrentInstance.SpeedInMbps) : "Unknown";
            FPS.Text = String.Format(" : {0:0.0}", PerformanceMonitor.CurrentInstance.AverageClientFps);

            double rtt = PerformanceMonitor.CurrentInstance.AverageMouseMoveMsgRTTWithResponse;
            AveRTT.Text = rtt > 1000 ? String.Format("{0:0.0} ms", rtt) : String.Format(" : {0:0.0} ms", PerformanceMonitor.CurrentInstance.AverageMouseMoveMsgRTTWithResponse);

            AveImageSize.Text = String.Format(" : {0:0.0} KB", PerformanceMonitor.CurrentInstance.AverageImageSize / 1024);

            LastImageSize.Text = String.Format(" : {0:0.0} KB", PerformanceMonitor.CurrentInstance.LastImageSize / 1024);
        }

        void InstancePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UIThread.Execute(() =>
            {
                if (e.PropertyName.Equals("AverageClientFps"))
                {
                    var fps = PerformanceMonitor.CurrentInstance.AverageClientFps;
                    FPS.Text = String.Format(" : {0}", double.IsNaN(fps)?"?": string.Format("{0:0}", fps));
                }
                else if (e.PropertyName.Equals("AverageImageSize"))
                    AveImageSize.Text = String.Format(" : {0:0.0} KB", PerformanceMonitor.CurrentInstance.AverageImageSize / 1024);

                else if (e.PropertyName.Equals("AverageMouseMoveMsgRTTWithResponse"))
                {
                    double rtt = PerformanceMonitor.CurrentInstance.AverageMouseMoveMsgRTTWithResponse;
                    AveRTT.Text = rtt > 1000 ? String.Format("{0:0.0} ms", rtt) : String.Format(" : {0:0.0} ms", PerformanceMonitor.CurrentInstance.AverageMouseMoveMsgRTTWithResponse);
                }
                else if (e.PropertyName.Equals("LastImageSize"))
                {
                    LastImageSize.Text = String.Format(" : {0:0.0} KB", PerformanceMonitor.CurrentInstance.LastImageSize / 1024);
                }

                else if (e.PropertyName.Equals("SpeedInMbps"))
                {
                    Speed.Text = PerformanceMonitor.CurrentInstance.SpeedInMbps > 0 ? String.Format(" : {0:0.0} Mpbs", PerformanceMonitor.CurrentInstance.SpeedInMbps) : "Unknown";
                }
            });

        }
    }
}
