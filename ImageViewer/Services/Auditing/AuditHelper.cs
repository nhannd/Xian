using System;
using System.Collections.Generic;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Audit;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Dicom.Network;
using ClearCanvas.ImageViewer.Services.DicomServer;

namespace ClearCanvas.ImageViewer.Services.Auditing
{
	/// <summary>
	/// Provides static helper methods to records events in the audit log.
	/// </summary>
	public static class AuditHelper
	{
		private static readonly string _messageAuditFailed = "Event Audit Failed.";
		private static AuditLog _log;

		/// <summary>
		/// Logs an event to the audit log using the format as described in DICOM Supplement 95.
		/// </summary>
		/// <param name="operation">A description of the operation.</param>
		/// <param name="message">The audit message to log.</param>
		public static void Log(string operation, DicomAuditHelper message)
		{
			if (_log == null)
				_log = new AuditLog("ImageViewer");

			_log.WriteEntry(operation ?? string.Empty, message.Serialize(false));
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
			try
			{
				UserAuthenticationAuditHelper auditHelper = new UserAuthenticationAuditHelper(authenticationServer, eventResult, UserAuthenticationEventType.Login);
				auditHelper.AddUserParticipant(new AuditPersonActiveParticipant(username, string.Empty, username));
				auditHelper.AddNode(authenticationServer);
				Log(operation, auditHelper);

				if (eventResult != EventResult.Success)
				{
					SecurityAlertAuditHelper alertAuditHelper = new SecurityAlertAuditHelper(authenticationServer, eventResult, SecurityAlertEventTypeCodeEnum.NodeAuthentication);
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
		/// <param name="authenticationServer">The authentication server against which the operation was performed.</param>
		/// <param name="eventResult">The result of the operation.</param>
		public static void LogLogout(string operation, string username, EventSource authenticationServer, EventResult eventResult)
		{
			try
			{
				UserAuthenticationAuditHelper auditHelper = new UserAuthenticationAuditHelper(authenticationServer, eventResult, UserAuthenticationEventType.Logout);
				auditHelper.AddUserParticipant(new AuditPersonActiveParticipant(username, string.Empty, username));
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
		/// Generates a "Dicom Instances Accessed" event in the audit log, according to DICOM Supplement 95.
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

		public static void LogCreateInstances(string operation, IEnumerable<string> aeTitles, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
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

		public static void LogUpdateInstances(string operation, IEnumerable<string> aeTitles, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
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

		public static void LogBeginSendInstances(string operation, string aeTitle, string hostname, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
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

		public static void LogSentInstances(string operation, string aeTitle, string hostname, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
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

		public static void LogBeginReceiveInstances(string operation, string aeTitle, string hostname, AuditedInstances instances, EventSource eventSource, EventResult eventResult)
		{
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

		public static void LogReceivedInstances(string operation, string aeTitle, string hostname, AuditedInstances instances, EventSource eventSource, EventResult eventResult, EventReceiptAction action)
		{
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

		private static string LocalAETitle
		{
			get { return DicomServerConfigurationHelper.AETitle; }
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