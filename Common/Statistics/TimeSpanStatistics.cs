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

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Used to store the elapsed time between two "events"
    /// </summary>
    /// <remarks>
    /// <para>
    /// Users call <seealso cref="OnBegin()"/> and <seealso cref="OnEnd()"/> to signal
    /// the beginning and at of the events. The elapsed time can be determinted using <seealso cref="ElapsedTimeInMs"/>
    /// </para>
    /// </remarks>
    public class TimeSpanStatistics : BaseStatistics
    {
        #region Private Variables
        private long _elapsedTime;
        #endregion

        #region Public Properties
        /// <summary>
        /// Elapsed time between two "events" (In 100 nanoseconds)
        /// </summary>
        /// <remarks>
        /// "Events" are marked by calling <seealso cref="OnBegin()"/> and <seealso cref="OnEnd()"/>
        /// </remarks>
        public long ElapsedTimeInMs
        {
            get
            {
                return _elapsedTime;
            }
            set
            {
                _elapsedTime = value;
                _statsValuesCollection["@ElapsedTimeInMs"] = value;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <seealso cref="TimeSpanStatistics"/>.
        /// </summary>
        /// <param name="desc"></param>
        public TimeSpanStatistics(string desc)
            : base(desc)
        {
        }

        #endregion

        #region Protected Overridden Methods
        
        protected override void OnBegin()
        {
            // NOOP
        }

        protected override void OnEnd()
        {
            ElapsedTimeInMs = (_endTick - _beginTick) / 10000; // convert 100 ns to ms
        }

        #endregion
    }
}
