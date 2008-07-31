using System;
using System.Collections.Generic;
using System.Text;
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
        private readonly SearchResultPage _page;
    	private readonly bool _downtimeRecoveryMode;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="page"></param>
        /// <param name="downtimeRecoveryMode"></param>
        public WorklistQueryContext(ApplicationServiceBase service, SearchResultPage page, bool downtimeRecoveryMode)
        {
            _applicationService = service;
            _page = page;
        	_downtimeRecoveryMode = downtimeRecoveryMode;
        }

        #region IWorklistQueryContext Members

        /// <summary>
        /// Gets the current user <see cref="Healthcare.Staff"/> object.
        /// </summary>
        public Staff Staff
        {
            get { return _applicationService.CurrentUserStaff; }
        }

        /// <summary>
        /// Gets the working <see cref="Facility"/> associated with the current user, or null if no facility is associated.
        /// </summary>
        public Facility WorkingFacility
        {
            get { return _applicationService.WorkingFacility; }
        }

    	/// <summary>
    	/// Gets a value indicating whether the worklist is being invoked in downtime recovery mode.
    	/// </summary>
    	public bool DowntimeRecoveryMode
    	{
    		get { return _downtimeRecoveryMode; }
    	}

    	/// <summary>
        /// Gets the <see cref="SearchResultPage"/> that specifies which page of the worklist is requested.
        /// </summary>
        public SearchResultPage Page
        {
            get { return _page; }
        }

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
