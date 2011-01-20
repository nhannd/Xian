#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Tests
{
	public class WorklistQueryContext : IWorklistQueryContext
	{
		private readonly IPersistenceContext _ctx;

		public WorklistQueryContext(IPersistenceContext ctx, Staff staff, Facility workingFacility, SearchResultPage page, bool downtimeMode)
		{
			_ctx = ctx;
			this.ExecutingStaff = staff;
			this.WorkingFacility = workingFacility;
			this.Page = page;
			this.DowntimeRecoveryMode = downtimeMode;
		}

		#region IWorklistQueryContext Members

		public Staff ExecutingStaff { get; private set; }

		public Facility WorkingFacility { get; private set; }

		public bool DowntimeRecoveryMode { get; private set; }

		public SearchResultPage Page { get; private set; }

		public TBrokerInterface GetBroker<TBrokerInterface>()
			where TBrokerInterface : IPersistenceBroker
		{
			return _ctx.GetBroker<TBrokerInterface>();
		}

		#endregion
	}
}
