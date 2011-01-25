#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare
{
	[ExtensionOf(typeof(VirtualOrderNoteProviderExtensionPoint))]
	public class OrderMergeHistoryVirtualOrderNoteProvider : IOrderNoteProvider
	{
		#region Helper classes

		/// <summary>
		/// Represents a tuple of (DateTime, Staff)
		/// </summary>
		internal struct TimeStaffPair : IEquatable<TimeStaffPair>
		{
			internal Staff Staff { get; set; }
			internal DateTime Time { get; set; }

			public bool Equals(TimeStaffPair other)
			{
				return Equals(other.Staff, Staff) && other.Time.Equals(Time);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (obj.GetType() != typeof(TimeStaffPair)) return false;
				return Equals((TimeStaffPair)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return (Staff.GetHashCode() * 397) ^ Time.GetHashCode();
				}
			}
		}

		#endregion

		/// <summary>
		/// Gets the list of notes for the specified order.
		/// </summary>
		/// <param name="order"></param>
		/// <param name="categories"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public IList<OrderNote> GetNotes(Order order, IList<string> categories, IPersistenceContext context)
		{
			if (categories != null && !categories.Contains(OrderNote.Categories.General))
				return new List<OrderNote>();


			var notes = new List<OrderNote>();
			if (order.Status == OrderStatus.MG)
			{
				notes.Add(CreateMergeSourceNote(order));
			}
			if (order.Status == OrderStatus.RP)
			{
				notes.Add(CreateReplaceSourceNote(order));
			}

			if (order.MergeSourceOrders.Count > 0)
			{
				notes.AddRange(CreateMergeDestinationNotes(order));
			}

			notes.AddRange(CreateReplaceDestinationNotes(order, context));

			foreach (var note in notes)
			{
				note.Body = "Auto-generated note: " + note.Body;
			}

			return notes;
		}

		private static List<OrderNote> CreateMergeDestinationNotes(Order order)
		{
			var groups = CollectionUtils.GroupBy(order.MergeSourceOrders,
				o => new TimeStaffPair { Staff = o.MergeInfo.MergedBy, Time = o.MergeInfo.MergedTime.Value });

			return CollectionUtils.Map(groups,
				(KeyValuePair<TimeStaffPair, List<Order>> kvp)
					=> CreateMergeDestinationNote(kvp.Value, order, kvp.Key.Staff, kvp.Key.Time));
		}

		private static OrderNote CreateMergeDestinationNote(ICollection<Order> sourceOrders, Order order, Staff staff, DateTime time)
		{
			var text = sourceOrders.Count == 1 ?
						string.Format("Order {0} is merged into this order.", CollectionUtils.FirstElement(sourceOrders).AccessionNumber)
						: string.Format("Orders {0} are merged into this order.", StringUtilities.Combine(sourceOrders, ", ", o => o.AccessionNumber));

			return OrderNote.CreateVirtualNote(order, OrderNote.Categories.General, staff, text, time);
		}

		private static OrderNote CreateMergeSourceNote(Order order)
		{
			var destOrder = order.MergeInfo.MergeDestinationOrder;
			var text = string.Format("This order is merged into order {0}.", destOrder.AccessionNumber);
			return OrderNote.CreateVirtualNote(
				order,
				OrderNote.Categories.General,
				order.MergeInfo.MergedBy,
				text,
				order.MergeInfo.MergedTime.Value);
		}

		private static List<OrderNote> CreateReplaceDestinationNotes(Order order, IPersistenceContext context)
		{
			var where = new OrderSearchCriteria();
			where.CancelInfo.ReplacementOrder.EqualTo(order);

			// expect 0 or 1 results
			var replacedOrder = CollectionUtils.FirstElement(context.GetBroker<IOrderBroker>().Find(where));
			if (replacedOrder == null)
				return new List<OrderNote>();

			var text = string.Format("Order {0} is replaced by this order.", replacedOrder.AccessionNumber);
			var note = OrderNote.CreateVirtualNote(
				order,
				OrderNote.Categories.General,
				replacedOrder.CancelInfo.CancelledBy,
				text,
				replacedOrder.EndTime.Value);

			return new List<OrderNote> { note };
		}

		private static OrderNote CreateReplaceSourceNote(Order order)
		{
			var replacementOrder = order.CancelInfo.ReplacementOrder;
			var reason = order.CancelInfo.Reason;
			var text = string.Format("This order is replaced by order {0}. Reason: {1}",
				replacementOrder.AccessionNumber,
				reason.Value);
			
			return OrderNote.CreateVirtualNote(
				order,
				OrderNote.Categories.General,
				order.CancelInfo.CancelledBy,
				text,
				order.EndTime.Value);
		}

	}
}
