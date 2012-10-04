#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
		/// <param name="userSessionId"></param>
		/// <param name="operation"></param>
		/// <param name="details"></param>
		protected internal AuditLogEntry(string category, DateTime timestamp, string hostName, string application, string user, string userSessionId, string operation, string details)
			:base(timestamp, hostName, application, user, userSessionId, operation, details)
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
