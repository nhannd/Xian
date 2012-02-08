#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Hibernate.Ddl;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	[ExtensionOf(typeof(BrokerExtensionPoint))]
	[ExtensionOf(typeof(DdlScriptGeneratorExtensionPoint))]
	public class MrnBroker : SequenceBroker, IMrnBroker, IDdlScriptGenerator
	{
		private const string TableName = "MrnSequence_";
		private const string ColumnName = "NextValue_";
		private const long InitialValue = 1000000;

		public MrnBroker()
			: base(TableName, ColumnName, InitialValue)
		{
		}
	}
}
