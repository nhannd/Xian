using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	public class OrderNoteViewComponent : DHtmlComponent
	{
		// Internal data contract used for jscript deserialization
		[DataContract]
		public class OrderNoteContext : DataContractBase
		{
			public OrderNoteContext(List<OrderNoteDetail> orderNotes)
			{
				this.OrderNotes = orderNotes;
				this.NotesAcknowledged = CollectionUtils.Map<OrderNoteDetail, bool>(
					orderNotes,
					delegate(OrderNoteDetail note)
						{
							return !note.CanAcknowledge;
						});
			}

			[DataMember]
			public List<OrderNoteDetail> OrderNotes;

			[DataMember]
			public List<bool> NotesAcknowledged;

			public bool IsNoteAcknowledged(OrderNoteDetail note)
			{
				int index = this.OrderNotes.IndexOf(note);
				return this.NotesAcknowledged[index];
			}
		}

		private event EventHandler _checkedItemsChanged;

		private readonly OrderNoteContext _context;

		public OrderNoteViewComponent()
			: this(null)
		{
		}

		public OrderNoteViewComponent(List<OrderNoteDetail> orderNotes)
		{
			_context = orderNotes == null ? null : new OrderNoteContext(orderNotes);
		}

		public override void Start()
		{
			SetUrl(WebResourcesSettings.Default.OrderNotePreviewPageUrl);
			base.Start();
		}

		public bool HasExistingNotes
		{
			get { return _context.OrderNotes.Count > 0; }
		}

		public bool HasNotesToBeAcknowledged
		{
			get
			{
				return CollectionUtils.Contains(_context.OrderNotes,
							delegate(OrderNoteDetail note)
							{
								return note.CanAcknowledge;
							});
			}
		}

		public bool HasUnacknowledgedNotes
		{
			get
			{
				return CollectionUtils.Contains(_context.NotesAcknowledged,
							delegate(bool noteAcknowledged)
							{
								return !noteAcknowledged;
							});
			}
		}

		public List<OrderNoteDetail> NotesToBeAcknowledged
		{
			get
			{
				return CollectionUtils.Select(_context.OrderNotes,
					delegate(OrderNoteDetail note)
					{
						return note.CanAcknowledge;
					});
			}
		}

		public event EventHandler CheckedItemsChanged
		{
			add { _checkedItemsChanged += value; }
			remove { _checkedItemsChanged -= value; }
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
		}

		protected override void SetTag(string tag, string data)
		{
			// the page uses SetTag to indicate that the check state has changed
			_context.NotesAcknowledged = JsmlSerializer.Deserialize<List<bool>>(data);
			EventsHelper.Fire(_checkedItemsChanged, this, EventArgs.Empty);
		}

	}
}
