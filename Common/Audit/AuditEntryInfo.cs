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
using System.Runtime.Serialization;
using System.Text;

namespace ClearCanvas.Common.Audit
{
	/// <summary>
	/// Contains all information about an audit log entry.
	/// </summary>
	[DataContract]
	public class AuditEntryInfo
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="category"></param>
		/// <param name="timeStamp"></param>
		/// <param name="hostName"></param>
		/// <param name="application"></param>
		/// <param name="user"></param>
		/// <param name="userSessionId"></param>
		/// <param name="operation"></param>
		/// <param name="details"></param>
		public AuditEntryInfo(string category, DateTime timeStamp, string hostName, string application, string user, string userSessionId, string operation, string details)
		{
			TimeStamp = timeStamp;
			HostName = hostName;
			Application = application;
			User = user;
			UserSessionId = userSessionId;
			Category = category;
			Operation = operation;
			Details = details;
		}

		/// <summary>
		/// Gets or sets the time at which this log entry was created.
		/// </summary>
		[DataMember]
		public DateTime TimeStamp { get; private set; }

		/// <summary>
		/// Gets or sets the hostname of the computer that generated this log entry.
		/// </summary>
		[DataMember]
		public string HostName { get; private set; }

		/// <summary>
		/// Gets or sets the the name of the application that created this log entry.
		/// </summary>
		[DataMember]
		public string Application { get; private set; }

		/// <summary>
		/// Gets or sets the user of the application on whose behalf this log entry was created.
		/// </summary>
		[DataMember]
		public string User { get; private set; }

		/// <summary>
		/// Gets or sets the user session ID on whose behalf this log entry was created.
		/// </summary>
		[DataMember(IsRequired = false)]
		public string UserSessionId { get; private set; }

		/// <summary>
		/// Gets or sets the name of the operation that caused this log entry to be created.
		/// </summary>
		[DataMember]
		public string Operation { get; private set; }

		/// <summary>
		/// Gets or sets the contents of this log entry, which may be text or XML based.
		/// </summary>
		[DataMember]
		public string Details { get; private set; }

		/// <summary>
		/// Gets or sets the category of this audit log entry.
		/// </summary>
		[DataMember]
		public string Category { get; private set; }
	}
}
