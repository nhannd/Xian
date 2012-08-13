#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	public interface ILogicalHL7EventListener
	{
		void OnEvent(LogicalHL7EventArgs eventArgs);
	}

	[ExtensionPoint]
	public class LogicalHL7EventListenerExtensionPoint : ExtensionPoint<ILogicalHL7EventListener>
	{
	}

	/// <summary>
	/// Encapsulates a WorkQueueItem for HL7 event processing that has extended properties specifying an HL7 event type,
	/// an order and optionally a procedure.
	/// </summary>
	public class LogicalHL7EventArgs
	{
		private readonly WorkQueueItem _item;

		public LogicalHL7EventArgs(WorkQueueItem item)
		{
			_item = item;
		}


		#region Public properties

		public WorkQueueItem Item
		{
			get { return _item; }
		}

		public IDictionary<string, string> ExtendedProperties
		{
			get { return _item.ExtendedProperties; }
		}

		public string EventType
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

		public Guid ReportOID
		{
			get { return GetGuidProperty("ReportOID"); }
		}

		#endregion

		public bool IsOrderEvent()
		{
			return _item.ExtendedProperties.ContainsKey("OrderOID") 
				&& !_item.ExtendedProperties.ContainsKey("ProcedureOID")
				&& !_item.ExtendedProperties.ContainsKey("ReportOID");
		}

		public bool IsProcedureEvent()
		{
			return _item.ExtendedProperties.ContainsKey("ProcedureOID");
		}

		public bool IsReportEvent()
		{
			return _item.ExtendedProperties.ContainsKey("ReportOID");
		}

		private Guid GetGuidProperty(string property)
		{
			return !String.IsNullOrEmpty(_item.ExtendedProperties[property])
				? new Guid(_item.ExtendedProperties[property])
				: Guid.Empty;
		}
	}
}