#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
