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
	/// <summary>
	/// Represents an outgoing email message.
	/// </summary>
	public class OutgoingMailMessage
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public OutgoingMailMessage()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public OutgoingMailMessage(string sender, string recipient, string subject, string body, bool isHtml)
		{
			Sender = sender;
			Recipient = recipient;
			Subject = subject;
			Body = body;
			IsHtml = isHtml;
		}

		/// <summary>
		/// Gets or sets the sender address.
		/// </summary>
		public string Sender { get; set; }

		/// <summary>
		/// Gets or sets the recipient address.
		/// </summary>
		public string Recipient { get; set; }

		/// <summary>
		/// Gets or sets the subject line.
		/// </summary>
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the content of the message body.
		/// </summary>
		public string Body { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="Body"/> text is HTML.
		/// </summary>
		public bool IsHtml { get; set; }

		/// <summary>
		/// Enqueues this message for transmission.
		/// </summary>
		/// <remarks>
		/// This method is a convenience method and is equivalent to calling
		/// <see cref="IMailQueueService.EnqueueMessage"/> directly.
		/// </remarks>
		public void Enqueue()
		{
			Platform.GetService<IMailQueueService>(service => service.EnqueueMessage(this));
		}
	}
}
