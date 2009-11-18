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
				return new DicomAuditSource(AuditHelper.LocalAETitle, string.Empty, AuditSourceTypeCodeEnum.ApplicationServerProcessTierInMultiTierSystem);
			}

			protected override AuditActiveParticipant AsAuditActiveParticipant()
			{
				return new AuditProcessActiveParticipant(AuditHelper.LocalAETitle);
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