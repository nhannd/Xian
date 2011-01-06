#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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

		public const string ProcedureCreated = "ProcedureCreated";
		public const string ProcedureModified = "ProcedureModified";
		public const string ProcedureCancelled = "ProcedureCancelled";

		public const string ReportPublished = "ReportPublished";
	}

	public class InvalidLogicalHL7EventTypeException : Exception
	{ }
}