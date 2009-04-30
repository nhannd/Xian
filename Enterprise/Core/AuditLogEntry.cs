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

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Represents an entry in an audit log.
    /// </summary>
    public class AuditLogEntry : LogEntry
    {
        private string _category;

        /// <summary>
        /// Private no-args constructor (required to support NHibernate).
        /// </summary>
        protected AuditLogEntry()
        {

        }

		/// <summary>
		/// All args constructor.
		/// </summary>
		/// <param name="category"></param>
		/// <param name="timestamp"></param>
		/// <param name="hostName"></param>
		/// <param name="application"></param>
		/// <param name="user"></param>
		/// <param name="operation"></param>
		/// <param name="details"></param>
		protected internal AuditLogEntry(string category, DateTime timestamp, string hostName, string application, string user, string operation, string details)
			:base(timestamp, hostName, application, user, operation, details)
		{
			_category = category;
		}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="operation"></param>
        /// <param name="details"></param>
        public AuditLogEntry(string category, string operation, string details)
            :base(operation, details)
        {
            _category = category;
        }

        /// <summary>
        /// Gets or sets the category of this audit log entry.
        /// </summary>
        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }
    }
}
