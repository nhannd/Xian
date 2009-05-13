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

using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Ris.Application.Common
{
	/// <summary>
	/// Defines constants for all core RIS authority tokens.
	/// </summary>
	public static class AuthorityTokens
	{
		/// <summary>
		/// Tokens that allow access to administrative functionality.
		/// </summary>
		public static class Admin
		{
			public static class Data
			{
				[AuthorityToken(Description = "Allow administration of Facilities.")]
				public const string Facility = "RIS/Admin/Data/Facility";

				[AuthorityToken(Description = "Allow administration of Patient Locations.")]
                public const string Location = "RIS/Admin/Data/Location";

				[AuthorityToken(Description = "Allow administration of Modalities.")]
                public const string Modality = "RIS/Admin/Data/Modality";

				[AuthorityToken(Description = "Allow administration of Procedure Types.")]
                public const string ProcedureType = "RIS/Admin/Data/Procedure Type";

				[AuthorityToken(Description = "Allow administration of Procedure Type Groups (such as Performing, Reading, and Relevance Groups.")]
                public const string ProcedureTypeGroup = "RIS/Admin/Data/Procedure Type Group";

				[AuthorityToken(Description = "Allow administration of Imaging Services and the Imaging Service Tree.")]
                public const string DiagnosticService = "RIS/Admin/Data/Imaging Service";

				[AuthorityToken(Description = "Allow administration of Enumerations.")]
                public const string Enumeration = "RIS/Admin/Data/Enumeration";

				[AuthorityToken(Description = "Allow administration of Worklists.")]
                public const string Worklist = "RIS/Admin/Data/Worklist";

				[AuthorityToken(Description = "Allow administration of Protocol Groups and Codes.")]
                public const string ProtocolGroups = "RIS/Admin/Data/Protocol Groups";

				[AuthorityToken(Description = "Allow administration of Staff.")]
                public const string Staff = "RIS/Admin/Data/Staff";

				[AuthorityToken(Description = "Allow administration of Staff Groups.")]
                public const string StaffGroup = "RIS/Admin/Data/Staff Group";

				[AuthorityToken(Description = "Allow administration of External Practitioners.")]
                public const string ExternalPractitioner = "RIS/Admin/Data/External Practitioner";

				[AuthorityToken(Description = "Allow administration of Patient Note Categories.")]
                public const string PatientNoteCategory = "RIS/Admin/Data/Patient Note Category";
			}
		}

		/// <summary>
		/// Tokens that allow access to management tools and functionality.
		/// </summary>
		public static class Management
		{
			public static class HL7
			{
				[AuthorityToken(Description = "Allow access to the HL7 Interface Queue Monitor.")]
                public const string QueueMonitor = "RIS/Management/HL7/Queue Monitor";
			}

			[AuthorityToken(Description = "Allow administration of the work queue.")]
            public const string WorkQueue = "RIS/Management/Work Queue";
		}

		/// <summary>
		/// Tokens that allow access to development tools and functionality.
		/// </summary>
		public static class Development
		{
			[AuthorityToken(Description = "Allow viewing of unfiltered worklists in top-level folders.")]
            public const string ViewUnfilteredWorkflowFolders = "RIS/Development/View Unfiltered Workflow Folders";

			[AuthorityToken(Description = "Allow creation of randomly generated test orders.")]
            public const string CreateTestOrder = "RIS/Development/Create Test Order";

			[AuthorityToken(Description = "Allow usage of the tool for manual publication of radiology reports.")]
            public const string TestPublishReport = "RIS/Development/Test Publish";

		}

		/// <summary>
		/// Tokens that permit workflow actions.
		/// </summary>
		public static class Workflow
		{
            public static class HomePage
            {
                [AuthorityToken(Description = "Allow access to the home page.")]
                public const string View = "RIS/Workflow/Home Page/View";
            }


            public static class PatientBiography
			{
				[AuthorityToken(Description = "Allow viewing of Patient Biography.")]
                public const string View = "RIS/Workflow/Patient Biography/View";
			}

			public static class CannedText
			{
                [AuthorityToken(Description = "Allow creation, modification and deletion of personal Canned Texts.")]
                public const string Personal = "RIS/Workflow/Canned Text/Personal";

				[AuthorityToken(Description = "Allow creation, modification and deletion of group Canned Texts.")]
                public const string Group = "RIS/Workflow/Canned Text/Group";
			}


			public static class Report
			{
				[AuthorityToken(Description = "Allow access to the Report Editor and creation of radiology reports.")]
                public const string Create = "RIS/Workflow/Report/Create";

				[AuthorityToken(Description = "Allow verification of radiology reports.")]
                public const string Verify = "RIS/Workflow/Report/Verify";

				[AuthorityToken(Description = "Allow radiology reports to be submitted for review by another radiologist.")]
                public const string SubmitForReview = "RIS/Workflow/Report/Submit for Review";

				[AuthorityToken(Description = "Allow creation of radiology reports without specifying a supervisor.")]
                public const string OmitSupervisor = "RIS/Workflow/Report/Omit Supervisor";

				[AuthorityToken(Description = "Allow re-assignment of a radiology report that is owned by one radiologist to another radiologist.")]
                public const string Reassign = "RIS/Workflow/Report/Reassign";

				[AuthorityToken(Description = "Allow cancellation of a radiology report that is owned by another radiologist.")]
                public const string Cancel = "RIS/Workflow/Report/Cancel";
			}

			public static class Protocol
			{
				[AuthorityToken(Description = "Allow access to the Protocol Editor and creation of procedure protocols.")]
                public const string Create = "RIS/Workflow/Protocol/Create";

				[AuthorityToken(Description = "Allow verification of procedure protocols.")]
                public const string Accept = "RIS/Workflow/Protocol/Verify";

				[AuthorityToken(Description = "Allow orders that were rejected by the radiologist to be re-submitted for protocoling.")]
                public const string Resubmit = "RIS/Workflow/Protocol/Resubmit";

				[AuthorityToken(Description = "Allow procedure protocols to be submitted for review by another radiologist.")]
                public const string SubmitForReview = "RIS/Workflow/Protocol/Submit for Review";

				[AuthorityToken(Description = "Allow creation of procedure protocols without specifying a supervisor.")]
                public const string OmitSupervisor = "RIS/Workflow/Protocol/Omit Supervisor";

				[AuthorityToken(Description = "Allow re-assignment of a procedure protocol that is owned by one radiologist to another radiologist.")]
                public const string Reassign = "RIS/Workflow/Protocol/Reassign";

				[AuthorityToken(Description = "Allow cancellation of a procedure protocol that is currently owned by another radiologist.")]
                public const string Cancel = "RIS/Workflow/Protocol/Cancel";
			}

			public static class Transcription
			{
				[AuthorityToken(Description = "Allow access to the Transcription Editor and creation of report transcriptions.")]
                public const string Create = "RIS/Workflow/Transcription/Create";

				[AuthorityToken(Description = "Allow transcriptions to be submitted for review by another party.")]
                public const string SubmitForReview = "RIS/Workflow/Transcription/Submit For Review";
			}

			public static class Patient
			{
				[AuthorityToken(Description = "Allow creation of new Patient records.")]
                public const string Create = "RIS/Workflow/Patient/Create";

				[AuthorityToken(Description = "Allow updating of Patient records (excluding Patient Profile information).")]
                public const string Update = "RIS/Workflow/Patient/Update";

				[AuthorityToken(Description = "Allow reconciliation of existing Patient records.")]
                public const string Reconcile = "RIS/Workflow/Patient/Reconcile";
			}

			public static class PatientProfile
			{
				[AuthorityToken(Description = "Allow updating of existing Patient Profile records.")]
                public const string Update = "RIS/Workflow/Patient Profile/Update";
			}

			public static class Visit
			{
				[AuthorityToken(Description = "Allow creation of new Visit records.")]
                public const string Create = "RIS/Workflow/Visit/Create";

				[AuthorityToken(Description = "Allow updating of existing Visit records.")]
                public const string Update = "RIS/Workflow/Visit/Update";
			}

			public static class Order
			{
				[AuthorityToken(Description = "Allow creation of new Orders.")]
                public const string Create = "RIS/Workflow/Order/Create";

				[AuthorityToken(Description = "Allow modification of existing Orders.")]
                public const string Modify = "RIS/Workflow/Order/Modify";

				[AuthorityToken(Description = "Allow replacement of existing Orders.")]
                public const string Replace = "RIS/Workflow/Order/Replace";

				[AuthorityToken(Description = "Allow cancellation of existing Orders.")]
                public const string Cancel = "RIS/Workflow/Order/Cancel";
			}

			public static class ExternalPractitioner
			{
				[AuthorityToken(Description = "Allow creation of External Practitioner records.")]
                public const string Create = "RIS/Workflow/External Practitioner/Create";

				[AuthorityToken(Description = "Allow updating of existing External Practitioner records.")]
                public const string Update = "RIS/Workflow/External Practitioner/Update";

				[AuthorityToken(Description = "Allow merging of existing External Practitioner records.")]
                public const string Merge = "RIS/Workflow/External Practitioner/Merge";
			}

			public static class Procedure
			{
				[AuthorityToken(Description = "Allow access to the Procedure Check-In function.")]
                public const string CheckIn = "RIS/Workflow/Procedure/Check In";
			}

			public static class Documentation
			{
				[AuthorityToken(Description = "Allow access to the Exam Documentation function, and creation of Exam Documentation.")]
                public const string Create = "RIS/Workflow/Documentation/Create";

				[AuthorityToken(Description = "Allow acceptance of Exam Documentation.")]
                public const string Accept = "RIS/Workflow/Documentation/Accept";
			}

			public static class PreliminaryDiagnosis
			{
				[AuthorityToken(Description = "Allow creation of Preliminary Diagnosis conversations.")]
                public const string Create = "RIS/Workflow/Preliminary Diagnosis/Create";
			}

			public static class Downtime
			{
				[AuthorityToken(Description = "Allow printing of downtime forms.")]
                public const string PrintForms = "RIS/Workflow/Downtime/Print Forms";

				[AuthorityToken(Description = "Allow access to the downtime recovery operations.")]
                public const string RecoveryOperations = "RIS/Workflow/Downtime/Recovery Operations";
			}

			public static class Worklist
			{
				[AuthorityToken(Description = "Allow creation, modification and deletion of personal worklists.")]
                public const string Personal = "RIS/Workflow/Worklist/Personal";

				[AuthorityToken(Description = "Allow creation, modification and deletion of group worklists.")]
                public const string Group = "RIS/Workflow/Worklist/Group";
			}

			public static class StaffProfile
			{
                [AuthorityToken(Description = "Allow a user to view own Staff Profile.")]
                public const string View = "RIS/Workflow/Staff Profile/View";

                [AuthorityToken(Description = "Allow a user to update own Staff Profile.")]
                public const string Update = "RIS/Workflow/Staff Profile/Update";
			}
		}

		/// <summary>
		/// Tokens that control access to Folder Systems.
		/// </summary>
		public static class FolderSystems
		{
			[AuthorityToken(Description = "Allow access to the Booking folder system.")]
            public const string Booking = "RIS/Folder Systems/Booking";

			[AuthorityToken(Description = "Allow access to the Registration folder system.")]
            public const string Registration = "RIS/Folder Systems/Registration";

			[AuthorityToken(Description = "Allow access to the Performing folder system.")]
            public const string Performing = "RIS/Folder Systems/Performing";

			[AuthorityToken(Description = "Allow access to the Reporting folder system.")]
            public const string Reporting = "RIS/Folder Systems/Reporting";

			[AuthorityToken(Description = "Allow access to the Protocolling folder system.")]
            public const string Protocolling = "RIS/Folder Systems/Protocolling";

			[AuthorityToken(Description = "Allow access to the Order Notes folder system.")]
            public const string OrderNotes = "RIS/Folder Systems/Order Notes";

			[AuthorityToken(Description = "Allow access to the Emergency folder system.")]
            public const string Emergency = "RIS/Folder Systems/Emergency";

			[AuthorityToken(Description = "Allow access to the Radiologist Admin folder system.")]
            public const string RadiologistAdmin = "RIS/Folder Systems/Radiologist Admin";

			[AuthorityToken(Description = "Allow access to the Transcription folder system.")]
            public const string Transcription = "RIS/Folder Systems/Transcription";
		}
	}
}
