#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

#pragma warning disable 1591

using System;

namespace ClearCanvas.Common.Performance
{
    /// <summary>
    /// The type of information used in <see cref="RateStatistics"/>.
    /// </summary>
    public enum RateType
    {
        BYTES,
        MESSAGES
    } ;

    /// <summary>
    /// Statistics class to store the rate of changes of the specified information.
    /// </summary>
    /// <remarks>
    /// The information being supported include: Byte rates or Message rate. The number of bytes or messages is set by calling <see cref="SetData"/>.
    /// The rate will be calculated based on the value set by calling <see cref="SetData"/> in between <see cref="Start"/> and <see cref="End"/> calls.
    /// 
    /// <see cref="IStatistics.FormattedValue"/> of the <see cref="RateStatistics"/> has unit of "GB/s", "MB/s", "KB/s" or "msg/s"
    /// depending on type of value specified in the constructor.
    /// 
    /// 
    /// <example>
    /// <code>
    ///     RateStatistics transferSpeed = new RateStatistics("Speed", RateType.BYTES);
    ///     transferSpeed.Begin();
    ///     transferSpeed.SetData(2408);
    ///     transferSpeed.End();    
    /// </code>
    /// 
    /// <para>If the time elapsed between Begin() and End() is one second, then transferSpeed.FormattedValue = "2 KB/s"</para>
    /// <para>If the time elapsed is 5 seconds, then transferSpeed.FormattedValue = "0.4 KB/s"</para>
    /// 
    /// </example>
    /// 
    /// 
    /// 
    /// </remarks>
    public class RateStatistics : Statistics<double>
    {
        private const double TICKSPERSECOND= 10*1000*1000;
        
        private const double KILOBYTES = 1024;
        private const double MEGABYTES = 1024 * KILOBYTES;
        private const double GIGABYTES = 1024 * MEGABYTES;
        

        private long _begin;
        private long _end;
        private double _prevdata = 0;
        private double _delta = 0;

        private RateType _type;

        public RateStatistics(string name, RateType rateType)
            : base(name)
        {
            _type = rateType;

            switch (_type)
            {
                case RateType.BYTES:
                    ValueFormatter = delegate(double rate)
                                         {
                                             if (rate > GIGABYTES)
                                                 return String.Format("{0:0.00} GB/s", rate / GIGABYTES);
                                             if (rate > MEGABYTES)
                                                return String.Format("{0:0.00} MB/s", rate/ MEGABYTES);
                                             if (rate > KILOBYTES)
                                                return String.Format("{0:0.00} KB/s", rate / KILOBYTES);

                                             return String.Format("{0:0} B/s", rate);
                                         };
                    break;

                case RateType.MESSAGES:
                    ValueFormatter = delegate(double rate)
                                         {
                                             return String.Format("{0:0.00} msg/s", rate);
                                         };
                    break;
            }
        }

        public void SetData(double value)
        {
            _delta = value - _prevdata;
            _prevdata = value;
        }

        public void Start()
        {
            _begin = DateTime.Now.Ticks;

        }
        public void End()
        {
            _end = DateTime.Now.Ticks + 1;
            Value = _delta / ((_end - _begin) / TICKSPERSECOND);
        }

        public override bool SetValue(object value)
        {
            if (value is double)
            {
                Value = (double) value;
                return true;
            }
            else
                return base.SetValue(value);

        }

       
    }

   }
