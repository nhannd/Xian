#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Audit
{

	/// <summary>
	/// Security alert event types for use with <see cref="SecurityAlertAuditHelper"/>
	/// </summary>
	public enum SecurityAlertEventTypeCodeEnum
	{
		NodeAuthentication,
		EmergencyOverride,
		NetworkConfiguration,
		SecurityConfiguration,
		HardwareConfiguration,
		SoftwareConfiguration,
		UseOfRestrictedFunction,
		AuditRecordingStopped,
		AuditRecordingStarted,
		ObjectSecurityAttributesChanged,
		SecurityRolesChanged,
		UserSecurityAttributesChanged
	}

	/// <summary>
	/// Security Alert Audit Message Helper
	/// </summary>
	/// <remarks>
	/// <para>
	/// This message describes any event for which a node needs to report a security alert, e.g., a node
	/// authentication failure when establishing a secure communications channel.
	/// </para>
	/// <para>
	/// Note: The Node Authentication event can be used to report both successes and failures. If reporting of
	/// success is done, this could generate a very large number of audit messages, since every authenticated
	/// DICOM association, HL7 transaction, and HTML connection should result in a successful node
	/// authentication. It is expected that in most situations only the node authentication failures will be
	/// reported.
	/// </para>
	/// </remarks>
	public class SecurityAlertAuditHelper : DicomAuditHelper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="auditSource"></param>
		/// <param name="outcome">Success implies an informative alert. The other failure values
		/// imply warning codes that indicate the severity of the alert. A Minor
		/// or Serious failure indicates that mitigation efforts were effective in
		/// maintaining system security. A Major failure indicates that
		/// mitigation efforts may not have been effective, and that the security
		/// system may have been compromised.</param>
		/// <param name="eventTypeCode">The type of Security Alert event</param>
		public SecurityAlertAuditHelper(DicomAuditSource auditSource,
			EventIdentificationTypeEventOutcomeIndicator outcome,
			SecurityAlertEventTypeCodeEnum eventTypeCode)
			: base("SecurityAlert")
		{
			AuditMessage.EventIdentification = new EventIdentificationType();
			AuditMessage.EventIdentification.EventID = CodedValueType.SecurityAlert;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationTypeEventActionCode.E;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;
			AuditMessage.EventIdentification.EventTypeCode = new CodedValueType[] { GetEventTypeCode(eventTypeCode) };

			InternalAddAuditSource(auditSource);
		}

		/// <summary>
		/// The identity of the person or process that detected the activity of
		/// concern. If both are known, then two active participants shall be
		/// included (both the person and the process).
		/// </summary>
		/// <param name="userId">
		/// The identity of the person or process that detected the activity of
		/// concern. If both are known, then two active participants shall be
		/// included (both the person and the process).
		/// </param>
		/// <param name="participant">The reporting participant</param>
		public void AddReportingUser(AuditActiveParticipant participant)
		{
			participant.UserIsRequestor = true;
			InternalAddActiveParticipant(participant);
		}

		/// <summary>
		/// The identity of the person, process, node, or other actor that
		/// performed the activity reported by the alert. If multiple such
		/// participants are known, then all shall be included.
		/// </summary>
		/// <remarks>
		/// Note: In some cases, the user identity is not known precisely. In
		/// such cases, the Active Participant can be left out.
		/// </remarks>
		/// <param name="participant">The participant.</param>
		public void AddActiveParticipant(AuditActiveParticipant participant)
		{
			participant.UserIsRequestor = false;
			InternalAddActiveParticipant(participant);
		}


		/// <summary>
		/// Method for transforming event code enum into a CodedValueType.
		/// </summary>
		/// <param name="eventTypeCode"></param>
		/// <returns></returns>
		private static CodedValueType GetEventTypeCode(SecurityAlertEventTypeCodeEnum eventTypeCode)
		{
			CodedValueType type = null;
			if (eventTypeCode == SecurityAlertEventTypeCodeEnum.NodeAuthentication)
				type = CodedValueType.NodeAuthentication;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.EmergencyOverride)
				type = CodedValueType.EmergencyOverride;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.NetworkConfiguration)
				type = CodedValueType.NetworkConfiguration;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.SecurityConfiguration)
				type = CodedValueType.SecurityConfiguration;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.HardwareConfiguration)
				type = CodedValueType.HardwareConfiguration;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.SoftwareConfiguration)
				type = CodedValueType.SoftwareConfiguration;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.UseOfRestrictedFunction)
				type = CodedValueType.UseOfRestrictedFunction;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.AuditRecordingStopped)
				type = CodedValueType.AuditRecordingStopped;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.AuditRecordingStarted)
				type = CodedValueType.AuditRecordingStarted;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.ObjectSecurityAttributesChanged)
				type = CodedValueType.ObjectSecurityAttributesChanged;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.SecurityRolesChanged)
				type = CodedValueType.SecurityRolesChanged;
			else if (eventTypeCode == SecurityAlertEventTypeCodeEnum.UserSecurityAttributesChanged)
				type = CodedValueType.UserSecurityAttributesChanged;
			return type;
		}
	}
}
