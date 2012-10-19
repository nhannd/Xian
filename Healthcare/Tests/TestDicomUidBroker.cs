#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare.Tests
{
	class TestDicomUidBroker : IDicomUidBroker
	{
		public void SetContext(IPersistenceContext context)
		{
		}

		public string GetNewUid()
		{
			return "1.3.6.000000000000000000000";
		}
	}
}
