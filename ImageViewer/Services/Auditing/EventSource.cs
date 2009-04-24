using System;
using System.Security.Principal;
using System.Threading;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.ImageViewer.Services.DicomServer;

namespace ClearCanvas.ImageViewer.Services.Auditing
{
	/// <summary>
	/// Represents the source of a particular auditable event.
	/// </summary>
	public abstract class EventSource
	{
		/// <summary>
		/// The source of the auditable event is the current end user.
		/// </summary>
		public static readonly EventSource CurrentUser = new CurrentUserEventSource();

		/// <summary>
		/// The source of the audtable event is the current DICOM server process.
		/// </summary>
		/// <remarks>
		/// The value of this source changes dynamically when the DICOM server configuration is changed.
		/// </remarks>
		public static readonly EventSource CurrentProcess = new CurrentProcessEventSource();

		/// <summary>
		/// A generic source for when the actual source is unknown.
		/// </summary>
		public static readonly EventSource UnknownSource = new OtherEventSource("Unknown");

		public static EventSource GetOtherEventSource(string otherSourceName)
		{
			return new OtherEventSource(otherSourceName);
		}

		private EventSource() {}

		/// <summary>
		/// Gets the <paramref name="eventSource"/> as a <see cref="DicomAuditSource"/>.
		/// </summary>
		public static implicit operator DicomAuditSource(EventSource eventSource)
		{
			return eventSource.AsDicomAuditSource();
		}

		/// <summary>
		/// Gets the <paramref name="eventSource"/> as an <see cref="AuditActiveParticipant"/>.
		/// </summary>
		public static implicit operator AuditActiveParticipant(EventSource eventSource)
		{
			return eventSource.AsAuditActiveParticipant();
		}

		/// <summary>
		/// Gets the <paramref name="eventSource"/> as an <see cref="AuditProcessActiveParticipant"/> if supported.
		/// </summary>
		public static implicit operator AuditProcessActiveParticipant(EventSource eventSource)
		{
			AuditProcessActiveParticipant result = eventSource.AsAuditActiveParticipant() as AuditProcessActiveParticipant;
			if (result == null)
				throw new InvalidCastException();
			return result;
		}

		protected abstract DicomAuditSource AsDicomAuditSource();
		protected abstract AuditActiveParticipant AsAuditActiveParticipant();

		private class OtherEventSource : EventSource
		{
			private readonly string _id;

			public OtherEventSource(string id)
			{
				_id = id;
			}

			protected override DicomAuditSource AsDicomAuditSource()
			{
				return new DicomAuditSource(_id, string.Empty, AuditSourceTypeCodeEnum.ExternalSourceOtherOrUnknownType);
			}

			protected override AuditActiveParticipant AsAuditActiveParticipant()
			{
				return new AuditProcessActiveParticipant(_id);
			}
		}

		private class CurrentProcessEventSource : EventSource
		{
			protected override DicomAuditSource AsDicomAuditSource()
			{
				return new DicomAuditSource(DicomServerConfigurationHelper.OfflineAETitle, string.Empty, AuditSourceTypeCodeEnum.ApplicationServerProcessTierInMultiTierSystem);
			}

			protected override AuditActiveParticipant AsAuditActiveParticipant()
			{
				return new AuditProcessActiveParticipant(DicomServerConfigurationHelper.OfflineAETitle);
			}
		}

		private class CurrentUserEventSource : EventSource
		{
			private static DicomAuditSource _currentUserAuditSource;
			private static AuditActiveParticipant _currentUserActiveParticipant;

			protected override DicomAuditSource AsDicomAuditSource()
			{
				if (_currentUserAuditSource == null)
					_currentUserAuditSource = new DicomAuditSource(GetUserName(), string.Empty, AuditSourceTypeCodeEnum.EndUserInterface);

				return _currentUserAuditSource;
			}

			protected override AuditActiveParticipant AsAuditActiveParticipant()
			{
				if (_currentUserActiveParticipant == null)
					_currentUserActiveParticipant = new AuditPersonActiveParticipant(GetUserName(), string.Empty, GetUserName());

				return _currentUserActiveParticipant;
			}

			private static string GetUserName()
			{
				IPrincipal p = Thread.CurrentPrincipal;
				if (p == null || p.Identity == null || string.IsNullOrEmpty(p.Identity.Name))
					return string.Format("{0}@{1}", Environment.UserName, Environment.UserDomainName);
				return p.Identity.Name;
			}
		}
	}
}