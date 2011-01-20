#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// <see cref="TimeSpan"/> formatter class.
    /// </summary>
    public static class TimeSpanFormatter
    {
        #region Constants

        private const double TICKSPERHOUR = TICKSPERMINUTE*60;
        private const double TICKSPERMICROECONDS = 10;
        private const double TICKSPERMILISECONDS = TICKSPERMICROECONDS*1000;
        private const double TICKSPERMINUTE = TICKSPERSECONDS*60;
        private const double TICKSPERNANOSECONDS = 1/100.0;
        private const double TICKSPERSECONDS = TICKSPERMILISECONDS*1000;

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Formats a <see cref="TimeSpan"/> in appropriate units, with option to round up.
        /// </summary>
        /// <param name="duration">The duration to be formatted</param>
        /// <param name="roundUp">Indicates whether the duration should be rounded up (eg, '3 sec' instead of '3.232 sec')</param>
        /// <returns>A formatted string representation of the duration</returns>
        public static string Format(TimeSpan duration, bool roundUp)
        {
            if (roundUp)
            {
                if (duration == TimeSpan.Zero)
                    return "N/A";
                else if (duration.Ticks > TICKSPERHOUR)
                    return String.Format("{0} hr {1} min", duration.Hours, duration.Minutes);
                if (duration.Ticks > TICKSPERMINUTE)
                    return String.Format("{0:0} min", duration.TotalMinutes);
                if (duration.Ticks > TICKSPERSECONDS)
                    return String.Format("{0:0} sec", duration.TotalSeconds);
                if (duration.Ticks > TICKSPERMILISECONDS)
                    return String.Format("{0:0} ms", duration.TotalMilliseconds);
                if (duration.Ticks > TICKSPERMICROECONDS)
                    return String.Format("{0:0} µs", duration.Ticks / TICKSPERMICROECONDS);
                else
                    return String.Format("{0:0} ns", duration.Ticks / TICKSPERNANOSECONDS);
            }
            else
            {
                if (duration == TimeSpan.Zero)
                    return "N/A";
                else if (duration.Ticks > TICKSPERHOUR)
                    return String.Format("{0} hr {1} min", duration.Hours, duration.Minutes);
                if (duration.Ticks > TICKSPERMINUTE)
                    return String.Format("{0:0.00} min", duration.TotalMinutes);
                if (duration.Ticks > TICKSPERSECONDS)
                    return String.Format("{0:0.00} sec", duration.TotalSeconds);
                if (duration.Ticks > TICKSPERMILISECONDS)
                    return String.Format("{0:0.00} ms", duration.TotalMilliseconds);
                if (duration.Ticks > TICKSPERMICROECONDS)
                    return String.Format("{0:0.00} µs", duration.Ticks / TICKSPERMICROECONDS);
                else
                    return String.Format("{0:0.00} ns", duration.Ticks / TICKSPERNANOSECONDS);
                
            }
        }

        /// <summary>
        /// Formats a <see cref="TimeSpan"/> in appropriate units.
        /// </summary>
        /// <param name="duration">The duration to be formatted</param>
        /// <returns>A formatted string representation of the duration</returns>
        public static string Format(TimeSpan duration)
        {
            return Format(duration, false);
        }

        #endregion
    }
}