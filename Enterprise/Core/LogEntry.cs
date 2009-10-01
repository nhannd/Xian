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
using ClearCanvas.Common;
using System.Threading;
using System.Net;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Abstract base class representing an entry in a log.
    /// </summary>
    public abstract class LogEntry : Entity
    {
        private DateTime _timestamp;
    	private string _hostName;
        private string _application;
        private string _user;
    	private string _userSessionId;
        private string _operation;
        private string _details;

         /// <summary>
        /// No-args constructor (required to support NHibernate).
        /// </summary>
        protected LogEntry()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="hostName"></param>
        /// <param name="application"></param>
        /// <param name="user"></param>
        /// <param name="userSessionId"></param>
        /// <param name="operation"></param>
        /// <param name="details"></param>
        protected LogEntry(DateTime timestamp, string hostName, string application, string user, string userSessionId, string operation, string details)
        {
            _timestamp = timestamp;
        	_hostName = hostName;
            _application = application;
            _user = user;
        	_userSessionId = userSessionId;
            _operation = operation;
            _details = details;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="details"></param>
        protected LogEntry(string operation, string details)
            :this(
				SafeGetTime(),
				SafeGetHostName(),
                null, // TODO: replace this with something more meaningful????
                Thread.CurrentPrincipal.Identity.Name,
				SafeGetUserSessionId(),
                operation,
                details
                )
        {
        }

    	/// <summary>
        /// Gets or sets the time at which this log entry was created.
        /// </summary>
        public DateTime TimeStamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

		/// <summary>
		/// Gets or sets the hostname of the computer that generated this log entry.
		/// </summary>
		public string HostName
		{
			get { return _hostName; }
			set { _hostName = value; }
		}

		/// <summary>
        /// Gets or sets the the name of the application that created this log entry.
        /// </summary>
        public string Application
        {
            get { return _application; }
            set { _application = value; }
        }

        /// <summary>
        /// Gets or sets the user of the application on whose behalf this log entry was created.
        /// </summary>
        public string User
        {
            get { return _user; }
            set { _user = value; }
        }

		/// <summary>
		/// Gets or sets the user session ID on whose behalf this log entry was created.
		/// </summary>
    	public string UserSessionId
    	{
			get { return _userSessionId; }
			set { _userSessionId = value; }
    	}

        /// <summary>
        /// Gets or sets the name of the operation that caused this log entry to be created.
        /// </summary>
        public string Operation
        {
            get { return _operation; }
            set { _operation = value; }
        }

        /// <summary>
        /// Gets or sets the contents of this log entry, which may be text or XML based.
        /// </summary>
        public string Details
        {
            get { return _details; }
            set { _details = value; }
		}

		#region Helpers

		/// <summary>
		/// Gets the hostname of the local computer, or null if an error occurs.
		/// </summary>
		/// <returns></returns>
		private static string SafeGetHostName()
		{
			try
			{
				return Dns.GetHostName();
			}
			catch(Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Returns <see cref="Platform.Time"/>, or the local computer time if an error occurs.
		/// </summary>
		/// <returns></returns>
		private static DateTime SafeGetTime()
		{
			try
			{
				return Platform.Time;
			}
			catch(Exception)
			{
				return DateTime.Now;
			}
		}

		/// <summary>
		/// Gets the user session ID of the current thread, if supported.
		/// </summary>
		/// <returns></returns>
		private static string SafeGetUserSessionId()
		{
			return (Thread.CurrentPrincipal is IUserCredentialsProvider) ? ((IUserCredentialsProvider)Thread.CurrentPrincipal).SessionTokenId : null;
		}

		#endregion
	}
}
