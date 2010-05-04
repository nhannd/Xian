#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Encapsulates a WorkQueueItem for HL7 event processing that has extended properties specifying an HL7 event type,
	/// an order and optionally a procedure.
	/// </summary>
	public class LogicalHL7EventWorkQueueItem
	{
		#region Private fields

		private readonly WorkQueueItem _item;

		#endregion

		#region Constructor

		public LogicalHL7EventWorkQueueItem(WorkQueueItem item)
		{
			_item = item;
		}

		#endregion

		#region Public properties

		public const string ItemType = "Logical HL7 Event";

		public WorkQueueItem Item
		{
			get { return _item; }
		}

		public IDictionary<string, string> ExtendedProperties
		{
			get { return _item.ExtendedProperties; }
		}

		public string Type
		{
			get { return _item.ExtendedProperties["EventType"]; }
		}

		public Guid OrderOID
		{
			get { return GetGuidProperty("OrderOID"); }
		}

		public Guid ProcedureOID
		{
			get { return GetGuidProperty("ProcedureOID"); }
		}

		#endregion

		public static LogicalHL7EventWorkQueueItem CreateOrderLogicalEvent(string logicalHL7EventType, Order order)
		{
			if (!logicalHL7EventType.Contains("Order"))
				throw new InvalidLogicalHL7EventTypeException();

			var queueItem = CreateBaseWorkQueueItem(logicalHL7EventType, order);

			return new LogicalHL7EventWorkQueueItem(queueItem);
		}

		public static LogicalHL7EventWorkQueueItem CreateProcedureLogicalEvent(string logicalHL7EventType, Procedure procedure)
		{
			if (!logicalHL7EventType.Contains("Order") && !logicalHL7EventType.Contains("Procedure"))
				throw new InvalidLogicalHL7EventTypeException();

			var queueItem = CreateBaseWorkQueueItem(logicalHL7EventType, procedure.Order);

			queueItem.ExtendedProperties.Add("ProcedureOID", procedure.OID.ToString());
			queueItem.ExtendedProperties.Add("ProcedureIndex", procedure.Index);

			return new LogicalHL7EventWorkQueueItem(queueItem);
		}

		public static IList<LogicalHL7EventWorkQueueItem> CreateReportLogicalEvents(string logicalHL7EventType, Report report)
		{
			if (!logicalHL7EventType.Contains("Report"))
				throw new InvalidLogicalHL7EventTypeException();

			var orders = CollectionUtils.Unique(
				CollectionUtils.Map<Procedure, Order>(
					report.Procedures,
					procedure => procedure.Order));

			return CollectionUtils.Map<Order, LogicalHL7EventWorkQueueItem>(orders,
				order =>
				{
					var queueItem = CreateBaseWorkQueueItem(logicalHL7EventType, order);
					queueItem.ExtendedProperties.Add("ReportOID", report.OID.ToString());
					return new LogicalHL7EventWorkQueueItem(queueItem);
				});
		}

		public bool IsOrder()
		{
			return _item.ExtendedProperties.ContainsKey("OrderOID") 
				&& !_item.ExtendedProperties.ContainsKey("ProcedureOID")
				&& !_item.ExtendedProperties.ContainsKey("ReportOID");
		}

		public bool IsProcedure()
		{
			return _item.ExtendedProperties.ContainsKey("ProcedureOID");
		}

		public bool IsReport()
		{
			return _item.ExtendedProperties.ContainsKey("ReportOID");
		}

		private static WorkQueueItem CreateBaseWorkQueueItem(string logicalHL7EventType, Order order)
		{
			Platform.Log(LogLevel.Info, "Creating Logical HL7 Event {0} AccessionNumber {1}", ItemType, order.AccessionNumber);

			var queueItem = new WorkQueueItem(ItemType);
			queueItem.ExtendedProperties.Add("EventType", logicalHL7EventType);
			queueItem.ExtendedProperties.Add("OrderOID", order.OID.ToString());
			queueItem.ExtendedProperties.Add("AccessionNumber", order.AccessionNumber);
			return queueItem;
		}

		private Guid GetGuidProperty(string property)
		{
			return !String.IsNullOrEmpty(_item.ExtendedProperties[property])
				? new Guid(_item.ExtendedProperties[property])
				: Guid.Empty;
		}
	}
}