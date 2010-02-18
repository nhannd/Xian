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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	public class LogicalHL7EventWorkQueueItem
	{
		public WorkQueueItem Item;
		public const string ItemType = "Logical HL7 Event";

		public IDictionary<string, string> ExtendedProperties
		{
			get { return Item.ExtendedProperties; }
		}

		public string Type
		{
			get { return Item.ExtendedProperties["EventType"]; }
		}

		public string OrderOID
		{
			get { return Item.ExtendedProperties["OrderOID"]; }
		}

		public string ProcedureOID
		{
			get { return Item.ExtendedProperties["ProcedureOID"]; }
		}

		public static LogicalHL7EventWorkQueueItem CreateOrderLogicalEvent(string logicalHL7EventType, Order order)
		{
			if (!logicalHL7EventType.Contains("Order"))
				throw new InvalidLogicalHL7EventTypeException();

			WorkQueueItem queueItem = CreateBaseWorkQueueItem(logicalHL7EventType, order);

			return new LogicalHL7EventWorkQueueItem(queueItem);
		}

		public static LogicalHL7EventWorkQueueItem CreateProcedureLogicalEvent(string logicalHL7EventType, Procedure procedure)
		{
			if (!logicalHL7EventType.Contains("Order") && !logicalHL7EventType.Contains("Procedure"))
				throw new InvalidLogicalHL7EventTypeException();

			WorkQueueItem queueItem = CreateBaseWorkQueueItem(logicalHL7EventType, procedure.Order);
			queueItem.ExtendedProperties.Add("ProcedureOID", procedure.OID.ToString());
			queueItem.ExtendedProperties.Add("ProcedureIndex", procedure.Index);

			return new LogicalHL7EventWorkQueueItem(queueItem);
		}

		public LogicalHL7EventWorkQueueItem(WorkQueueItem item)
		{
			Item = item;
		}

		public bool IsOrder()
		{
			return Item.ExtendedProperties.ContainsKey("OrderOID") && !Item.ExtendedProperties.ContainsKey("ProcedureOID");
		}

		public bool IsProcedure()
		{
			return Item.ExtendedProperties.ContainsKey("ProcedureOID");
		}

		private static WorkQueueItem CreateBaseWorkQueueItem(string logicalHL7EventType, Order order)
		{
			var queueItem = new WorkQueueItem(ItemType);
			Platform.Log(LogLevel.Info, "Creating Logical HL7 Event {0} AccessionNumber {1}", ItemType, order.AccessionNumber);

			queueItem.ExtendedProperties.Add("EventType", logicalHL7EventType);
			queueItem.ExtendedProperties.Add("OrderOID", order.OID.ToString());
			queueItem.ExtendedProperties.Add("AccessionNumber", order.AccessionNumber);
			return queueItem;
		}

	}
}