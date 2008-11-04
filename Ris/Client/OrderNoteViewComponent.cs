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
			}

			[DataMember]
			public List<OrderNoteDetail> OrderNotes;
		}

		private event EventHandler _checkedItemsChanged;
		private readonly IDictionary<OrderNoteDetail, bool> _checkedProperties;

		private readonly OrderNoteContext _context;

		public OrderNoteViewComponent()
			: this(null)
		{
		}

		public OrderNoteViewComponent(List<OrderNoteDetail> orderNotes)
		{
			_context = orderNotes == null ? null : new OrderNoteContext(orderNotes);
			_checkedProperties = new Dictionary<OrderNoteDetail, bool>();
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

		public bool HasUncheckedUnacknowledgedNotes
		{
			get
			{
				return CollectionUtils.Contains(_context.OrderNotes,
							delegate(OrderNoteDetail note)
							{
								return !IsChecked(note) && note.CanAcknowledge;
							});
			}
		}

		public bool HasUnacknowledgedNotes
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

		public List<EntityRef> GetOrderNotesToAcknowledge()
		{
			List<EntityRef> orderNotesToAcknowledge = new List<EntityRef>();

			CollectionUtils.ForEach(_checkedProperties.Keys,
				delegate(OrderNoteDetail note)
				{
					if (_checkedProperties[note] && note.CanAcknowledge)
						orderNotesToAcknowledge.Add(note.OrderNoteRef);
				});

			return orderNotesToAcknowledge;
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
			OrderNoteDetail noteMatch = CollectionUtils.SelectFirst(_context.OrderNotes,
				delegate(OrderNoteDetail note)
					{
						OrderNoteDetail n = JsmlSerializer.Deserialize<OrderNoteDetail>(tag);
						return note.OrderNoteRef.Equals(n.OrderNoteRef, true);
					});

			if (noteMatch != null)
			{
				bool isChecked;
				_checkedProperties[noteMatch] = bool.TryParse(data, out isChecked) && isChecked;
				EventsHelper.Fire(_checkedItemsChanged, this, EventArgs.Empty);
			}
		}

		private bool IsChecked(OrderNoteDetail note)
		{
			if (_checkedProperties.ContainsKey(note))
				return _checkedProperties[note];				

			return false;
		}
	}
}
