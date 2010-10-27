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
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Controls
{
    public partial class FPSCounter : UserControl
    {
        public FPSCounter()
        {
            InitializeComponent();

            StatisticsHelper.Update += new EventHandler(Update);
        }

        private void Update(object sender, EventArgs e)
        {
            FPSLabel.Text = String.Format("Frames Drawn: {0} in {2} ms, FPS: {1}",
                StatisticsHelper.FrameCount, StatisticsHelper.FPS, StatisticsHelper.Elapsed);
        }
    }
}
