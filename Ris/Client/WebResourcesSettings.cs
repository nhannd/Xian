#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Provides application settings for all core RIS web content URLss
	/// </summary>
	/// <remarks>
	/// This code is adapted from the Visual Studio generated template code;  the generated code has been removed from the project.  Additional 
	/// settings need to be manually added to this class.
	/// </remarks>
	[SettingsGroupDescription("Configures web content URLs.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	public sealed class WebResourcesSettings : global::System.Configuration.ApplicationSettingsBase
	{
		private static WebResourcesSettings defaultInstance = ((WebResourcesSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new WebResourcesSettings())));

		public WebResourcesSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public static WebResourcesSettings Default
		{
			get
			{
				return defaultInstance;
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("http://localhost/RIS")]
		[global::System.Configuration.SettingsDescription("Provides base URL for HtmlApplicationComponent web resources.  URL should specify protocol (i.e. http://server, file:///C:/directory, etc.)")]
		public string BaseUrl
		{
			get
			{
				return ((string)(this["BaseUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("banner.htm")]
		public string BannerPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BannerPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("downtime-form-template.htm")]
		public string DowntimeFormTemplatePageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["DowntimeFormTemplatePageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("PatientProfileDiff.html")]
		public string PatientReconciliationPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["PatientReconciliationPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("work-queue-preview.htm")]
		public string WorkQueuePreviewPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["WorkQueuePreviewPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("order-note-preview.htm")]
		public string OrderNotePreviewPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["OrderNotePreviewPageUrl"]));
			}
		}

		#region Home Page Preview settings

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/registration.htm")]
		public string RegistrationFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["RegistrationFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/booking.htm")]
		public string BookingFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BookingFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/performing.htm")]
		public string PerformingFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["PerformingFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/protocolling.htm")]
		public string ProtocollingFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ProtocollingFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/reporting.htm")]
		public string ReportingFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ReportingFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/transcription.htm")]
		public string TranscriptionFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["TranscriptionFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/emergency.htm")]
		public string EmergencyFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["EmergencyFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/order-notes.htm")]
		public string OrderNoteboxFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["OrderNoteboxFolderSystemUrl"]));
			}
		}

		#endregion

		#region Patient Biography settings

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("biography/order-detail.htm")]
		public string BiographyOrderDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BiographyOrderDetailPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("biography/visit-detail.htm")]
		public string BiographyVisitDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BiographyVisitDetailPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("biography/patient-profile-detail.htm")]
		public string BiographyPatientProfilePageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BiographyPatientProfilePageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("biography/report-detail.htm")]
		public string BiographyReportDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BiographyReportDetailPageUrl"]));
			}
		}

		#endregion

		#region Reporting Settings

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("reporting/order-detail.htm")]
		public string ReportingOrderDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ReportingOrderDetailPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("reporting/default-report-editor.htm")]
		public string DefaultReportEditorPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["DefaultReportEditorPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("reporting/default-addendum-editor.htm")]
		public string DefaultAddendumEditorPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["DefaultAddendumEditorPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("reporting/report-preview.htm")]
		public string ReportPreviewPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ReportPreviewPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("reporting/prior-report-preview.htm")]
		public string PriorReportPreviewPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["PriorReportPreviewPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("reporting/print-report-preview.htm")]
		public string PrintReportPreviewUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["PrintReportPreviewUrl"]));
			}
		}

		#endregion

		#region Protocolling Settings

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("protocolling/order-detail.htm")]
		public string ProtocollingOrderDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ProtocollingOrderDetailPageUrl"]));
			}
		}

		#endregion

		#region Performing Settings

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("performing/order-detail.htm")]
		public string PerformingOrderDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["PerformingOrderDetailPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("performing/order-additional-info.htm")]
		public string OrderAdditionalInfoPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["OrderAdditionalInfoPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("forms/performing/mpps.htm")]
		public string DetailsPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["DetailsPageUrl"]));
			}
		}

		#endregion

		#region Transcription Settings

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("transcription/report-preview.htm")]
		public string TranscriptionPreviewPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["TranscriptionPreviewPageUrl"]));
			}
		}
		#endregion
	}

	public static class WebResourceAbsoluteUrlHelper
	{
		private static readonly char[] _slash = new char[] {'/'};

		public static string FromRelative(string relativeUrl)
		{
			return WebResourcesSettings.Default.BaseUrl.TrimEnd(_slash) + '/' + relativeUrl.TrimStart(_slash);
		}
	}
}
