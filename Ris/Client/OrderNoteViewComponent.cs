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

		public List<OrderNoteDetail> NotesJustAcknowledged
		{
			get
			{
				return CollectionUtils.Select(_context.OrderNotes,
					delegate(OrderNoteDetail note)
					{
						return note.CanAcknowledge && _context.IsNoteAcknowledged(note);
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
