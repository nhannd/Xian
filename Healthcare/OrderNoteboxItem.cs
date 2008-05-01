using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare
{
	public class OrderNoteboxItem
	{
		private readonly EntityRef _orderNoteRef;
		private readonly EntityRef _orderRef;
		private readonly EntityRef _patientRef;
		private readonly EntityRef _patientProfileRef;

		private readonly PatientIdentifier _mrn;
		private readonly PersonName _patientName;
		private readonly DateTime? _dateOfBirth;
		private readonly string _accessionNumber;
		private readonly string _diagnosticServiceName;
		private readonly string _category;
		private readonly DateTime? _postTime;
		private readonly Staff _author;
		private readonly bool _isAcknowledged;
		private NoteRecipient[] _recipients;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="note"></param>
		/// <param name="order"></param>
		/// <param name="patient"></param>
		/// <param name="patientProfile"></param>
		/// <param name="isAcknowledged"></param>
		public OrderNoteboxItem(OrderNote note, Order order, Patient patient, PatientProfile patientProfile, bool isAcknowledged)
		{
			_orderNoteRef = note.GetRef();
			_orderRef = order.GetRef();
			_patientRef = patient.GetRef();
			_patientProfileRef = patientProfile.GetRef();
			_mrn = patientProfile.Mrn;
			_patientName = patientProfile.Name;
			_dateOfBirth = patientProfile.DateOfBirth;
			_accessionNumber = order.AccessionNumber;
			_diagnosticServiceName = order.DiagnosticService.Name;
			_category = note.Category;
			_postTime = note.PostTime;
			_author = note.Author;
			_isAcknowledged = isAcknowledged;

			_recipients = note.IsPosted ? CollectionUtils.Map<NotePosting, NoteRecipient>(note.Postings,
				delegate(NotePosting nr) { return nr.Recipient; }).ToArray() : new NoteRecipient[] { };
		}

		public EntityRef OrderNoteRef
		{
			get { return _orderNoteRef; }
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

		public DateTime? PostTime
		{
			get { return _postTime; }
		}

		public Staff Author
		{
			get { return _author; }
		}

		public bool IsAcknowledged
		{
			get { return _isAcknowledged; }
		}

		public NoteRecipient[] Recipients
		{
			get { return _recipients; }
		}
	}
}
