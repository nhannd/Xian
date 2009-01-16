using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Reflection;
using System.Threading;
using System.Net;

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
        /// <param name="operation"></param>
        /// <param name="details"></param>
        protected LogEntry(DateTime timestamp, string hostName, string application, string user, string operation, string details)
        {
            _timestamp = timestamp;
        	_hostName = hostName;
            _application = application;
            _user = user;
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

		#endregion
	}
}
