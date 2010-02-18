using System;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Events that may generate HL7 messages. The string constant must contain either "Order" or "Procedure", this is
	/// checked in the Event constructor to provide some safety that a good event type is passed in
	/// </summary>
	public class LogicalHL7EventType
	{
		public const string OrderCreated = "OrderCreated";
		public const string OrderModified = "OrderModified";
		public const string OrderCancelled = "OrderCancelled";
	}

	public class InvalidLogicalHL7EventTypeException : Exception
	{ }
}