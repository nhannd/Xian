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

using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// OrderNote component
    /// </summary>
	public partial class OrderNote
	{
		public static IList<OrderNote> GetNotesForOrder(Order order)
		{
			return GetNotesForOrder(order, null);
		}

		public static IList<OrderNote> GetNotesForOrder(Order order, string category)
		{
			OrderNoteSearchCriteria where = new OrderNoteSearchCriteria();
			where.Order.EqualTo(order);
			where.PostTime.IsNotNull(); // only posted notes
			if(!string.IsNullOrEmpty(category))
			{
				where.Category.EqualTo(category);
			}

			//run a query to find order notes
			//TODO: using PersistenceScope is maybe not ideal but no other option right now (fix #3472)
			return PersistenceScope.CurrentContext.GetBroker<IOrderNoteBroker>().Find(where);
		}

        /// <summary>
        /// Constructor for creating a new note with recipients.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="category"></param>
        /// <param name="author"></param>
        /// <param name="onBehalfOf"></param>
        /// <param name="body"></param>
        public OrderNote(Order order, string category, Staff author, StaffGroup onBehalfOf, string body, bool urgent)
            :base(category, author, onBehalfOf, body, urgent)
        {
            _order = order;
        }

        /// <summary>
        /// Overridden to validate that the order does not have any notes that are pending acknowledgement
        /// that could be acknowledged by the author of this note.
        /// </summary>
        protected override void BeforePost()
        {
            // does the order have any notes, in the same category as this note,
            // that could have been acknowledged by the author of this note but haven't been?
        	IList<OrderNote> allNotes = GetNotesForOrder(_order, this.Category);
			bool unAckedNotes = CollectionUtils.Contains(allNotes,
                delegate(OrderNote note)
                {
					// ignore this note
					return !Equals(this, note) && note.CanAcknowledge(this.Author);
                });

            if(unAckedNotes)
                throw new NoteAcknowledgementException("Order has associated notes that must be acknowledged by this author prior to posting a new note.");

            base.BeforePost();
        }


		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}