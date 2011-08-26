#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core.Mail
{
	public class OutgoingMailMessage
	{
		public OutgoingMailMessage()
		{
			
		}

		public OutgoingMailMessage(string sender, string recipient, string subject, string body, bool isHtml)
		{
			Sender = sender;
			Recipient = recipient;
			Subject = subject;
			Body = body;
			IsHtml = isHtml;
		}

		public string Sender { get; set; }
		public string Recipient { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public bool IsHtml { get; set; }

		public void Enqueue()
		{
			Platform.GetService<IMailQueueService>(service => service.EnqueueMessage(this));
		}
	}
}
