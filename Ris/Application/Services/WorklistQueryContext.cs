#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Services
{
	/// <summary>
	/// Implementation of <see cref="IWorklistQueryContext"/>.
	/// </summary>
	class WorklistQueryContext : IWorklistQueryContext
	{
		private readonly ApplicationServiceBase _applicationService;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="service"></param>
		/// <param name="workingFacility"></param>
		/// <param name="page"></param>
		/// <param name="downtimeRecoveryMode"></param>
		public WorklistQueryContext(ApplicationServiceBase service, Facility workingFacility, SearchResultPage page, bool downtimeRecoveryMode)
		{
			_applicationService = service;
			WorkingFacility = workingFacility;
			Page = page;
			DowntimeRecoveryMode = downtimeRecoveryMode;
		}

		#region IWorklistQueryContext Members

		/// <summary>
		/// Gets the current user <see cref="Healthcare.Staff"/> object.
		/// </summary>
		public Staff ExecutingStaff
		{
			get { return _applicationService.CurrentUserStaff; }
		}

		/// <summary>
		/// Gets the working <see cref="Facility"/> associated with the current user, or null if no facility is associated.
		/// </summary>
		public Facility WorkingFacility { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the worklist is being invoked in downtime recovery mode.
		/// </summary>
		public bool DowntimeRecoveryMode { get; private set; }

		/// <summary>
		/// Gets the <see cref="SearchResultPage"/> that specifies which page of the worklist is requested.
		/// </summary>
		public SearchResultPage Page { get; private set; }

		/// <summary>
		/// Obtains an instance of the specified broker.
		/// </summary>
		/// <typeparam name="TBrokerInterface"></typeparam>
		/// <returns></returns>
		public TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker
		{
			return _applicationService.PersistenceContext.GetBroker<TBrokerInterface>();
		}

		#endregion
	}
}
