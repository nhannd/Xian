using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
    public class Formula
    {
        public static double AreaOfRectangle(double width, double height)
        {
            return Math.Abs(width*height);
        }

        public static double AreaOfEllipse(double width, double height)
        {
            // pi/4 = 0.7853981633
            return Math.Abs(0.7853981633 * width * height);
        }
    }
}
