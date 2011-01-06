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
	/// Enum for use with <see cref="UserAuthenticationAuditHelper"/>.
	/// </summary>
	public enum UserAuthenticationEventType
	{
		Login,
		Logout
	}

	/// <summary>
	/// User Authentication Audit Message
	/// </summary>
	/// <remarks>
	/// <para>
	/// This message describes the event of a user has attempting to log on or log off, whether successful or not.
	/// No Participant Objects are needed for this message.
	/// </para>
	/// </remarks>
	public class UserAuthenticationAuditHelper : DicomAuditHelper
	{
		public UserAuthenticationAuditHelper(DicomAuditSource auditSource,
			EventIdentificationTypeEventOutcomeIndicator outcome, UserAuthenticationEventType type)
			: base("UserAuthentication")
		{
			AuditMessage.EventIdentification = new EventIdentificationType();
			AuditMessage.EventIdentification.EventID = CodedValueType.UserAuthentication;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationTypeEventActionCode.E;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddAuditSource(auditSource);

			if (type == UserAuthenticationEventType.Login)
				AuditMessage.EventIdentification.EventTypeCode = new CodedValueType[] { CodedValueType.Login };
			else
				AuditMessage.EventIdentification.EventTypeCode = new CodedValueType[] { CodedValueType.Logout };
		}

		/// <summary>
		/// The identity of the person authenticated if successful. Asserted identity if not successful.
		/// </summary>
		/// <param name="participant">The participant.</param>
		public void AddUserParticipant(AuditPersonActiveParticipant participant)
		{
			participant.UserIsRequestor = true;
			InternalAddActiveParticipant(participant);
		}

		/// <summary>
		/// The identity of the node that is authenticating the user. This is to
		/// identify another node that is performing enterprise wide
		/// authentication, e.g. Kerberos authentication.
		/// </summary>
		/// <param name="participant">The participant.</param>
		public void AddNode(AuditProcessActiveParticipant participant)
		{
			participant.UserIsRequestor = false;
			InternalAddActiveParticipant(participant);
		}
	}
}
