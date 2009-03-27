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
		private string _category;
		private DateTime _timestamp;
		private string _hostName;
		private string _application;
		private string _user;
		private string _operation;
		private string _details;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="category"></param>
		/// <param name="timeStamp"></param>
		/// <param name="hostName"></param>
		/// <param name="application"></param>
		/// <param name="user"></param>
		/// <param name="operation"></param>
		/// <param name="details"></param>
		public AuditEntryInfo(string category, DateTime timeStamp, string hostName, string application, string user, string operation, string details)
		{
			TimeStamp = timeStamp;
			HostName = hostName;
			Application = application;
			User = user;
			Category = category;
			Operation = operation;
			Details = details;
		}

		/// <summary>
		/// Gets or sets the time at which this log entry was created.
		/// </summary>
		[DataMember]
		public DateTime TimeStamp
		{
			get { return _timestamp; }
			private set { _timestamp = value; }
		}

		/// <summary>
		/// Gets or sets the hostname of the computer that generated this log entry.
		/// </summary>
		[DataMember]
		public string HostName
		{
			get { return _hostName; }
			private set { _hostName = value; }
		}

		/// <summary>
		/// Gets or sets the the name of the application that created this log entry.
		/// </summary>
		[DataMember]
		public string Application
		{
			get { return _application; }
			private set { _application = value; }
		}

		/// <summary>
		/// Gets or sets the user of the application on whose behalf this log entry was created.
		/// </summary>
		[DataMember]
		public string User
		{
			get { return _user; }
			private set { _user = value; }
		}

		/// <summary>
		/// Gets or sets the name of the operation that caused this log entry to be created.
		/// </summary>
		[DataMember]
		public string Operation
		{
			get { return _operation; }
			private set { _operation = value; }
		}

		/// <summary>
		/// Gets or sets the contents of this log entry, which may be text or XML based.
		/// </summary>
		[DataMember]
		public string Details
		{
			get { return _details; }
			private set { _details = value; }
		}

		/// <summary>
		/// Gets or sets the category of this audit log entry.
		/// </summary>
		[DataMember]
		public string Category
		{
			get { return _category; }
			private set { _category = value; }
		}
	}
}
