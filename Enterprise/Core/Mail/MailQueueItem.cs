#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Enterprise.Core.Mail
{
	public class MailQueueItem : TaskQueueItem
	{
		/// <summary>
		/// Default constructor for NHibernate.
		/// </summary>
		private MailQueueItem()
		{
		}

		public MailQueueItem(string recipientAddress, string subject, string body, bool isHtml)
		{
			this.RecipientAddress = recipientAddress;
			this.Subject = subject;
			this.Body = body;
			this.IsHtml = isHtml;
		}

		#region Overrides of TaskQueueItem

		protected override void Execute()
		{
			throw new NotImplementedException();
		}

		#endregion

		private string RecipientAddress
		{
			get { return this.TaskProperties["RecipientAddress"]; }
			set { this.TaskProperties["RecipientAddress"] = value; }
		}
		private string Subject
		{
			get { return this.TaskProperties["Subject"]; }
			set { this.TaskProperties["Subject"] = value; }
		}
		private string Body
		{
			get { return this.TaskProperties["Body"]; }
			set { this.TaskProperties["Body"] = value; }
		}
		private bool IsHtml
		{
			get { return bool.Parse(this.TaskProperties["IsHtml"]); }
			set { this.TaskProperties["IsHtml"] = value.ToString(); }
		}

	}
}
