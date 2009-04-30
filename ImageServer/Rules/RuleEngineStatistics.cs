#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using ClearCanvas.Common.Statistics;

namespace ClearCanvas.ImageServer.Rules
{
    /// <summary>
    /// Stores the engine statistics of a rule engine.
    /// </summary>
    public class RuleEngineStatistics : StatisticsSet
    {
        #region Private members

        #endregion Private members

        public void Reset()
        {
            LoadTime.Reset();
            ExecutionTime.Reset();
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the execution time of the rule engine in miliseconds.
        /// </summary>
        public TimeSpanStatistics ExecutionTime
        {
            get
            {
                if (this["ExecutionTime"] == null)
                {
                    this["ExecutionTime"] = new TimeSpanStatistics("ExecutionTime");
                }

                return (this["ExecutionTime"] as TimeSpanStatistics);
            }
        }

        /// <summary>
        /// Gets or sets the load time of the rule engine in miliseconds.
        /// </summary>
        public TimeSpanStatistics LoadTime
        {
            get
            {
                if (this["LoadTime"] == null)
                    this["LoadTime"] = new TimeSpanStatistics("LoadTime");
                return (this["LoadTime"] as TimeSpanStatistics);
            }
        }

        #endregion Public Properties

        #region Constructors

        public RuleEngineStatistics()
            : base()
        {
        }

        public RuleEngineStatistics(string name, string description)
            : base(name, description)
        {
            Context = new StatisticsContext(name);
        }

        #endregion
    }
}