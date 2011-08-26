#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Enterprise.Core.Mail
{
	/// <summary>
	/// Defines an interface to a service that provides and outgoing mail queue.
	/// </summary>
	public interface IMailQueueService
	{
		/// <summary>
		/// Enqueues the specified message for transmission.
		/// </summary>
		/// <param name="message"></param>
		void EnqueueMessage(OutgoingMailMessage message);
	}
}
