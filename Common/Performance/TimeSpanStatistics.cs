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
    /// Statistics class to store the time span.
    /// </summary>
    /// <remarks>
    /// <see cref="IStatistics.FormattedValue"/> of the <see cref="RateStatistics"/> has unit of "sec", "ms", "min", "hr" depending on the elapsed time
    /// between <see cref="Start"/> and <see cref="End"/> calls. The number of ticks between these calls is stored in <see cref="Statistics{T}.Value"/>.
    /// 
    /// <example>
    /// <code>
    ///     TimeSpanStatistics ts = new TimeSpanStatistics("Time");
    ///     transferSpeed.Begin();
    ///     ....
    ///     transferSpeed.End();
    /// </code>
    /// 
    /// <para>If the time elapsed between Begin() and End() is 90 second, then ts.FormattedValue = "1 min 30 sec"</para>
    /// <para>If the time elapsed is 300 miliseconds, then ts.FormattedValue = "300ms"</para>
    /// 
    /// </example>
    /// 
    /// 
    /// 
    /// </remarks>
    public class TimeSpanStatistics : Statistics<System.TimeSpan>
    {
        private const double TICKSPERMILISECONDS = 10*1000;
        private const double TICKSPERSECONDS = TICKSPERMILISECONDS*1000;
        private const double TICKSPERMINUTE = TICKSPERSECONDS * 60;
        private const double TICKSPERHOUR = TICKSPERMINUTE * 60;


        private long _begin;
        private long _end;

        public TimeSpanStatistics(string name)
            : base(name)
        {

            Value = new System.TimeSpan();

            ValueFormatter = delegate(TimeSpan ts)
                                         {
                                             if (ts.Ticks > TICKSPERHOUR)
                                                return String.Format("{0} hr {1}min", ts.Hours, ts.Minutes);
                                             if (ts.Ticks > TICKSPERMINUTE)
                                                 return String.Format("{0} min {1}sec", ts.Minutes, ts.Seconds);
                                             if (ts.Ticks > TICKSPERSECONDS)
                                                 return String.Format("{0}.{1} sec", ts.Seconds, ts.Milliseconds);
                                             if (ts.Ticks > TICKSPERMILISECONDS)
                                                 return String.Format("{0}ms", ts.Milliseconds);

                                            return String.Format("{0}ms", ts.TotalMilliseconds);
                                         };
                   
            

        }
       

        public override bool SetValue(object value)
        {
            if (value is long)
            {
                Value = new System.TimeSpan( (long)value);
                return true;
            }
            else
                return base.SetValue(value);
            
        }

        
        public void Start()
        {
            _begin = DateTime.Now.Ticks;

        }
        public void End()
        {
            _end = DateTime.Now.Ticks + 1;
            Value = new System.TimeSpan(_end - _begin);
        }

        


    }

   }
