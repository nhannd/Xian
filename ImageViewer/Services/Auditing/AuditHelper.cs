#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Audit;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.ImageViewer.Services.DicomServer;

namespace ClearCanvas.ImageViewer.Services.Auditing
{
	/// <summary>
	/// Provides static helper methods to records events in the audit log.
	/// </summary>
	public static class AuditHelper
	{
		private static readonly string _messageAuditFailed = "Event Audit Failed.";
		private static bool _auditingEnabled = true;
		private static AuditLog _log;

		/// <summary>
		/// Gets or sets a value indicating if auditing through this helper class is enabled or disabled.
		/// </summary>
		/// <remarks>
		/// A "Security Alert" event is generated in the audit log, according to DICOM Supplement 95,
		/// indicating whether audit recording was started or stopped.
		/// </remarks>
		public static bool Enabled
		{
			get { return _auditingEnabled; }
			set
			{
				if (_auditingEnabled != value)
				{
					_auditingEnabled = value;

					SecurityAlertEventTypeCodeEnum type = _auditingEnabled ? SecurityAlertEventTypeCodeEnum.AuditRecordingStarted : SecurityAlertEventTypeCodeEnum.AuditRecordingStopped;
					Log(_auditingEnabled ? "Auditing ON" : "Auditing OFF", new SecurityAlertAuditHelper(EventSource.CurrentUser, EventResult.Success, type));
				}
			}
		}

		/// <summary>
		/// Logs an event to the audit log using the format as described in DICOM Supplement 95.
		/// </summary>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="message">The audit message to log.</param>
		public static void Log(string operation, DicomAuditHelper message)
		{
			if (_auditingEnabled && _log == null)
			{
				AuditSinkExtensionPoint xp = new AuditSinkExtensionPoint();
				_auditingEnabled = xp.ListExtensions().Length > 0;
				if (_auditingEnabled)
					_log = new AuditLog("ImageViewer", string.Empty);
				else 
					Platform.Log(LogLevel.Warn, "No audit sink extensions found - Auditing will be disabled for the remainder of the session.");
			}

			if (_auditingEnabled)
				_log.WriteEntry(operation ?? string.Empty, message.Serialize(false));
		}

		/// <summary>
		/// Generates a "User Authentication" login event in the audit log, according to DICOM Supplement 95,
		/// and a "Security Alert" event if the operation failed.
		/// </summary>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="username">The username or asserted username of the account that was logged in.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogLogin(string operation, string username, EventResult eventResult)
		{
			LogLogin(operation, username, null, eventResult);
		}

		/// <summary>
		/// Generates a "User Authentication" login event in the audit log, according to DICOM Supplement 95,
		/// and a "Security Alert" event if the operation failed.
		/// </summary>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="username">The username or asserted username of the account that was logged in.</param>
		/// <param name="authenticationServer">The authentication server against which the operation was performed.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogLogin(string operation, string username, EventSource authenticationServer, EventResult eventResult)
		{
			if (!_auditingEnabled)
				return;

			try
			{
				UserAuthenticationAuditHelper auditHelper = new UserAuthenticationAuditHelper(EventSource.CurrentProcess, eventResult, UserAuthenticationEventType.Login);
				auditHelper.AddUserParticipant(new AuditPersonActiveParticipant(username, string.Empty, username));
				if (authenticationServer != null)
					auditHelper.AddNode(authenticationServer);

				Log(operation, auditHelper);

				if (eventResult != EventResult.Success)
				{
					SecurityAlertAuditHelper alertAuditHelper = new SecurityAlertAuditHelper(EventSource.CurrentProcess, eventResult, SecurityAlertEventTypeCodeEnum.NodeAuthentication);
					alertAuditHelper.AddReportingUser(EventSource.CurrentProcess);
					alertAuditHelper.AddActiveParticipant(new AuditPersonActiveParticipant(username, string.Empty, username));
					Log(operation, alertAuditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "User Authentication" logout event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="username">The username or asserted username of the account that was logged out.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogLogout(string operation, string username, EventResult eventResult)
		{
			LogLogout(operation, username, null, eventResult);
		}

		/// <summary>
		/// Generates a "User Authentication" logout event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="username">The username or asserted username of the account that was logged out.</param>
		/// <param name="authenticationServer">The authentication server against which the operation was performed.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogLogout(string operation, string username, EventSource authenticationServer, EventResult eventResult)
		{
			if (!_auditingEnabled)
				return;

			try
			{
				UserAuthenticationAuditHelper auditHelper = new UserAuthenticationAuditHelper(EventSource.CurrentProcess, eventResult, UserAuthenticationEventType.Logout);
				auditHelper.AddUserParticipant(new AuditPersonActiveParticipant(username, string.Empty, username));
				if (authenticationServer != null)
					auditHelper.AddNode(authenticationServer);

				Log(operation, auditHelper);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Query" event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="aeTitle">The application entity on which the query is taking place.</param>
		/// <param name="hostname">The hostname of the application entity on which the query is taking place.</param>
		/// <param name="instances">The studies that were opened.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogQueryStudies(string operation, string aeTitle, string hostname, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!_auditingEnabled)
				return;

			try
			{
				QueryAuditHelper auditHelper = new QueryAuditHelper(eventSource, eventResult,
					LocalAETitle, LocalHostname, aeTitle ?? LocalAETitle, hostname ?? LocalHostname);
				if (eventSource != EventSource.CurrentProcess)
					auditHelper.AddOtherParticipant(EventSource.CurrentProcess);
				foreach (AuditPatientParticipantObject patient in instances.EnumeratePatients())
					auditHelper.AddPatientParticipantObject(patient);
				foreach (AuditStudyParticipantObject study in instances.EnumerateStudies())
					auditHelper.AddStudyParticipantObject(study);
				Log(operation, auditHelper);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Dicom Instances Accessed" read event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="aeTitles">The application entities from which the instances were accessed.</param>
		/// <param name="instances">The studies that were opened.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogOpenStudies(string operation, IEnumerable<string> aeTitles, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!_auditingEnabled)
				return;

			try
			{
				string[] aeTitlesArray = ToArray(aeTitles);
				foreach (AuditPatientParticipantObject patient in instances.EnumeratePatients())
				{
					DicomInstancesAccessedAuditHelper auditHelper = new DicomInstancesAccessedAuditHelper(eventSource, eventResult, EventIdentificationTypeEventActionCode.R);
					auditHelper.AddUser(eventSource);
					if (aeTitlesArray.Length > 0)
						auditHelper.AddUser(new AuditProcessActiveParticipant(aeTitlesArray));
					auditHelper.AddPatientParticipantObject(patient);
					foreach (AuditStudyParticipantObject study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(operation, auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Dicom Instances Accessed" create event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="aeTitles">The application entities from which the instances were accessed.</param>
		/// <param name="instances">The studies that were opened.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogCreateInstances(string operation, IEnumerable<string> aeTitles, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!_auditingEnabled)
				return;

			try
			{
				string[] aeTitlesArray = ToArray(aeTitles);
				foreach (AuditPatientParticipantObject patient in instances.EnumeratePatients())
				{
					DicomInstancesAccessedAuditHelper auditHelper = new DicomInstancesAccessedAuditHelper(eventSource, eventResult, EventIdentificationTypeEventActionCode.C);
					auditHelper.AddUser(eventSource);
					if (aeTitlesArray.Length > 0)
						auditHelper.AddUser(new AuditProcessActiveParticipant(aeTitlesArray));
					auditHelper.AddPatientParticipantObject(patient);
					foreach (AuditStudyParticipantObject study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(operation, auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Dicom Instances Accessed" update event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="aeTitles">The application entities from which the instances were accessed.</param>
		/// <param name="instances">The studies that were opened.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogUpdateInstances(string operation, IEnumerable<string> aeTitles, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!_auditingEnabled)
				return;

			try
			{
				string[] aeTitlesArray = ToArray(aeTitles);
				foreach (AuditPatientParticipantObject patient in instances.EnumeratePatients())
				{
					DicomInstancesAccessedAuditHelper auditHelper = new DicomInstancesAccessedAuditHelper(eventSource, eventResult, EventIdentificationTypeEventActionCode.U);
					auditHelper.AddUser(eventSource);
					if (aeTitlesArray.Length > 0)
						auditHelper.AddUser(new AuditProcessActiveParticipant(aeTitlesArray));
					auditHelper.AddPatientParticipantObject(patient);
					foreach (AuditStudyParticipantObject study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(operation, auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Begin Transferring DICOM Instances" send event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="aeTitle">The application entity to which the transfer was started.</param>
		/// <param name="hostname">The hostname of the application entity to which the transfer was started.</param>
		/// <param name="instances">The studies that were queued for transfer.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogBeginSendInstances(string operation, string aeTitle, string hostname, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!_auditingEnabled)
				return;

			try
			{
				foreach (AuditPatientParticipantObject patient in instances.EnumeratePatients())
				{
					BeginTransferringDicomInstancesAuditHelper auditHelper = new BeginTransferringDicomInstancesAuditHelper(eventSource, eventResult,
						LocalAETitle, LocalHostname, aeTitle ?? LocalAETitle, hostname ?? LocalHostname, patient);
					foreach (AuditStudyParticipantObject study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(operation, auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "DICOM Instances Transferred" sent event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="aeTitle">The application entity to which the transfer was completed.</param>
		/// <param name="hostname">The hostname of the application entity to which the transfer was completed.</param>
		/// <param name="instances">The studies that were transferred.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogSentInstances(string operation, string aeTitle, string hostname, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!_auditingEnabled)
				return;

			try
			{
				foreach (AuditPatientParticipantObject patient in instances.EnumeratePatients())
				{
					DicomInstancesTransferredAuditHelper auditHelper = new DicomInstancesTransferredAuditHelper(eventSource, eventResult, EventReceiptAction.ActionUnknown,
						LocalAETitle, LocalHostname, aeTitle ?? LocalAETitle, hostname ?? LocalHostname);
					foreach (AuditStudyParticipantObject study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(operation, auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Begin Transferring DICOM Instances" receive event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="aeTitle">The application entity from which the transfer was started.</param>
		/// <param name="hostname">The hostname of the application entity from which the transfer was started.</param>
		/// <param name="instances">The studies that were requested for transfer.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogBeginReceiveInstances(string operation, string aeTitle, string hostname, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!_auditingEnabled)
				return;

			try
			{
				foreach (AuditPatientParticipantObject patient in instances.EnumeratePatients())
				{
					BeginTransferringDicomInstancesAuditHelper auditHelper = new BeginTransferringDicomInstancesAuditHelper(eventSource, eventResult,
						aeTitle ?? LocalAETitle, hostname ?? LocalHostname, LocalAETitle, LocalHostname, patient);
					foreach (AuditStudyParticipantObject study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(operation, auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "DICOM Instances Transferred" received event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="aeTitle">The application entity from which the transfer was completed.</param>
		/// <param name="hostname">The hostname of the application entity from which the transfer was completed.</param>
		/// <param name="instances">The studies that were transferred.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogReceivedInstances(string operation, string aeTitle, string hostname, AuditedInstances instances, EventSource eventSource, EventResult eventResult, EventReceiptAction action)
		{
			if (!_auditingEnabled)
				return;

			try
			{
				foreach (AuditPatientParticipantObject patient in instances.EnumeratePatients())
				{
					DicomInstancesTransferredAuditHelper auditHelper = new DicomInstancesTransferredAuditHelper(eventSource, eventResult, action,
						aeTitle ?? LocalAETitle, hostname ?? LocalHostname, LocalAETitle, LocalHostname);
					foreach (AuditStudyParticipantObject study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(operation, auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Data Import" event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// One audit event is generated for each file system volume from which data is imported.
		/// </remarks>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="instances">The files that were imported.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogImportStudies(string operation, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!_auditingEnabled)
				return;

			try
			{
				foreach (string volume in instances.EnumerateFileVolumes())
				{
					DataImportAuditHelper auditHelper = new DataImportAuditHelper(eventSource, eventResult, volume);
					auditHelper.AddImporter(eventSource);
					if (eventSource != EventSource.CurrentProcess)
						auditHelper.AddImporter(EventSource.CurrentProcess);
					foreach (AuditPatientParticipantObject patient in instances.EnumeratePatients())
						auditHelper.AddPatientParticipantObject(patient);
					foreach (AuditStudyParticipantObject study in instances.EnumerateStudies())
						auditHelper.AddStudyParticipantObject(study);
					Log(operation, auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Data Export" event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// One audit event is generated for each file system volume to which data is exported.
		/// </remarks>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="instances">The files that were exported.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogExportStudies(string operation, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!_auditingEnabled)
				return;

			try
			{
				foreach (string volume in instances.EnumerateFileVolumes())
				{
					DataExportAuditHelper auditHelper = new DataExportAuditHelper(eventSource, eventResult, volume);
					auditHelper.AddExporter(eventSource);
					if (eventSource != EventSource.CurrentProcess)
						auditHelper.AddExporter(EventSource.CurrentProcess);
					foreach (AuditPatientParticipantObject patient in instances.EnumeratePatients())
						auditHelper.AddPatientParticipantObject(patient);
					foreach (AuditStudyParticipantObject study in instances.EnumerateStudies())
						auditHelper.AddStudyParticipantObject(study);
					Log(operation, auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Generates a "Dicom Study Deleted" event in the audit log, according to DICOM Supplement 95.
		/// </summary>
		/// <remarks>
		/// This method automatically separates different patients into separately logged events, as required by DICOM.
		/// </remarks>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="aeTitle">The application entity from which the instances were deleted.</param>
		/// <param name="instances">The studies that were deleted.</param>
		/// <param name="eventSource">The source user or application entity which invoked the operation.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogDeleteStudies(string operation, string aeTitle, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
			if (!_auditingEnabled)
				return;

			try
			{
				foreach (AuditPatientParticipantObject patient in instances.EnumeratePatients())
				{
					DicomStudyDeletedAuditHelper auditHelper = new DicomStudyDeletedAuditHelper(eventSource, eventResult);
					auditHelper.AddUserParticipant(eventSource);
					auditHelper.AddUserParticipant(new AuditProcessActiveParticipant(aeTitle));
					auditHelper.AddPatientParticipantObject(patient);
					foreach (AuditStudyParticipantObject study in instances.EnumerateStudies(patient))
						auditHelper.AddStudyParticipantObject(study);
					Log(operation, auditHelper);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Warn, ex, _messageAuditFailed);
			}
		}

		/// <summary>
		/// Gets the current or last known AETitle of the local server.
		/// </summary>
		public static string LocalAETitle
		{
			get { return DicomServerConfigurationHelper.OfflineAETitle; }
		}

		private static string LocalHostname
		{
			get { return Dns.GetHostName(); }
		}

		private static T[] ToArray<T>(IEnumerable<T> source)
		{
			// prevent as much unnecessary list copying as is possible
			if (source is T[])
				return (T[]) source;
			if (source is List<T>)
				return ((List<T>) source).ToArray();
			return new List<T>(source).ToArray();
		}
	}
}