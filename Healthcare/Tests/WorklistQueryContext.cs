using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Tests
{
	public class WorklistQueryContext : IWorklistQueryContext
	{
		private readonly bool _downtimeMode;
		private readonly Staff _staff;

		public WorklistQueryContext(Staff staff, bool downtimeMode)
		{
			_downtimeMode = downtimeMode;
			_staff = staff;
		}

		#region IWorklistQueryContext Members

		public Staff Staff
		{
			get { return _staff; }
		}

		public Facility WorkingFacility
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public bool DowntimeRecoveryMode
		{
			get { return _downtimeMode; }
		}

		public SearchResultPage Page
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public TBrokerInterface GetBroker<TBrokerInterface>()
			where TBrokerInterface : ClearCanvas.Enterprise.Core.IPersistenceBroker
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
