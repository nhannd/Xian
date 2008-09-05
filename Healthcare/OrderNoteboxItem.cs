using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using System.Collections;

namespace ClearCanvas.Healthcare
{
	public class OrderNoteboxItem
	{
		private readonly EntityRef _noteRef;
		private readonly EntityRef _orderRef;
		private readonly EntityRef _patientRef;
		private readonly EntityRef _patientProfileRef;

		private readonly PatientIdentifier _mrn;
		private readonly PersonName _patientName;
		private readonly DateTime? _dateOfBirth;
		private readonly string _accessionNumber;
		private readonly string _diagnosticServiceName;
		private readonly string _category;
		private readonly bool _urgent;
		private readonly DateTime? _postTime;
		private readonly Staff _author;
		private readonly StaffGroup _onBehalfOfGroup;
		private readonly bool _isAcknowledged;
		private readonly IList _recipients;


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="note"></param>
		/// <param name="order"></param>
		/// <param name="patient"></param>
		/// <param name="patientProfile"></param>
		/// <param name="isAcknowledged"></param>
		public OrderNoteboxItem(Note note, Order order,
			Patient patient, PatientProfile patientProfile, bool isAcknowledged)
		{
			_noteRef = note.GetRef();
			_orderRef = order.GetRef();
			_patientRef = patient.GetRef();
			_patientProfileRef = patientProfile.GetRef();
			_mrn = patientProfile.Mrn;
			_patientName = patientProfile.Name;
			_dateOfBirth = patientProfile.DateOfBirth;
			_accessionNumber = order.AccessionNumber;
			_diagnosticServiceName = order.DiagnosticService.Name;
			_category = note.Category;
			_urgent = note.Urgent;
			_postTime = note.PostTime;
			_author = note.Author;
			_onBehalfOfGroup = note.OnBehalfOfGroup;
			_isAcknowledged = isAcknowledged;

			_recipients = CollectionUtils.Map<NotePosting, object>(note.Postings,
				delegate(NotePosting posting)
				{
					 return posting is StaffNotePosting ? (object)((StaffNotePosting)posting).Recipient :
						 (object)((GroupNotePosting)posting).Recipient;
				});
		}

		public EntityRef NoteRef
		{
			get { return _noteRef; }
		}

		public EntityRef OrderRef
		{
			get { return _orderRef; }
		}

		public EntityRef PatientRef
		{
			get { return _patientRef; }
		}

		public EntityRef PatientProfileRef
		{
			get { return _patientProfileRef; }
		}

		public PatientIdentifier Mrn
		{
			get { return _mrn; }
		}

		public PersonName PatientName
		{
			get { return _patientName; }
		}

		public DateTime? DateOfBirth
		{
			get { return _dateOfBirth; }
		}

		public string AccessionNumber
		{
			get { return _accessionNumber; }
		}

		public string DiagnosticServiceName
		{
			get { return _diagnosticServiceName; }
		}

		public string Category
		{
			get { return _category; }
		}

		public bool Urgent
		{
			get { return _urgent; }
		}

		public DateTime? PostTime
		{
			get { return _postTime; }
		}

		public Staff Author
		{
			get { return _author; }
		}

		public StaffGroup OnBehalfOfGroup
		{
			get { return _onBehalfOfGroup; }
		}

		public bool IsAcknowledged
		{
			get { return _isAcknowledged; }
		}

		public IList Recipients
		{
			get { return _recipients; }
		}
	}
}
