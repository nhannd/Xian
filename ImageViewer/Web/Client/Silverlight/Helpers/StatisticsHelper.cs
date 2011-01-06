#region License

// Copyright (c) 2011, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers
{
    //TODO: Delete this
    public class StatisticsHelper
    {
        public delegate void FPSUpdateMethodDelegate(int fps);
        
        public static event EventHandler Update;
        public static int FPS { get; private set; }

        public static int FrameCount { get; set; }

        private static int _startTick;
        private static int _lastUpdateTick;
        private static int _lastUiUpdate;

        public static void OnFrameDrawn()
        {
            FrameCount++;

            // TODO: REVIEW THIS
            // Per MSDN:
            // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
            // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
            int now = Environment.TickCount;
            if (now - _lastUpdateTick > 2000)
            {
                Reset();
            }

            Elapsed = now - _startTick + 1;
            FPS = FrameCount * 1000 / Elapsed;
            _lastUpdateTick = now;
            
            //if (now-_lastUiUpdate>500)
            {
				if (Update != null)
					Update(null, null);
                _lastUiUpdate = now;
            }
        }

        public static int Elapsed { get; set; }

        private static void Reset()
        {
            FrameCount = 0;
            // TODO: REVIEW THIS
            // Per MSDN:
            // if the system runs continuously, TickCount will increment from zero to Int32.MaxValue for approximately 24.9 days, 
            // then jump to Int32.MinValue, which is a negative number, then increment back to zero during the next 24.9 days.
            _startTick = Environment.TickCount;
        }
    }
}
