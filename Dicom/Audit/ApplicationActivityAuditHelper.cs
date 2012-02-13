#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Audit
{
	/// <summary>
	/// Enum for use with <see cref="ApplicationActivityAuditHelper"/>.
	/// </summary>
	public enum ApplicationActivityType
	{
		ApplicationStarted,
		ApplicationStopped
	}

	/// <summary>
	/// Helper for Application Activity Audit Log
	/// </summary>
	/// <remarks>
	/// This audit message describes the event of an Application Entity starting or stoping.
	/// </remarks>
	public class ApplicationActivityAuditHelper : DicomAuditHelper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="auditSource"></param>
		/// <param name="outcome"></param>
		/// <param name="type"></param>
		/// <param name="idOfApplicationStarted">Add the ID of the Application Started, should be called once.</param>
		public ApplicationActivityAuditHelper(DicomAuditSource auditSource,
			EventIdentificationContentsEventOutcomeIndicator outcome, 
			ApplicationActivityType type,
			AuditProcessActiveParticipant idOfApplicationStarted) : base("ApplicationActivity")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents();
			AuditMessage.EventIdentification.EventID = EventID.ApplicationActivity;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationContentsEventActionCode.E;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddAuditSource(auditSource);

			if (type == ApplicationActivityType.ApplicationStarted)
                AuditMessage.EventIdentification.EventTypeCode = new EventTypeCode[] { EventTypeCode.ApplicationStart };
			else
                AuditMessage.EventIdentification.EventTypeCode = new EventTypeCode[] { EventTypeCode.ApplicationStop };

			idOfApplicationStarted.UserIsRequestor = false;
			idOfApplicationStarted.RoleIdCode = RoleIDCode.Application;

			InternalAddActiveParticipant(idOfApplicationStarted);

		}

		/// <summary>
		/// Add the ID of person or process that started or stopped the Application.  Can be called multiple times.
		/// </summary>
		/// <param name="participant">The participant.</param>
		public void AddUserParticipant(AuditActiveParticipant participant)
		{
            participant.RoleIdCode = RoleIDCode.ApplicationLauncher;
			participant.UserIsRequestor = true;

			InternalAddActiveParticipant(participant);
		}
	}
}
