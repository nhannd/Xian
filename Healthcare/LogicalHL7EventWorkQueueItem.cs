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