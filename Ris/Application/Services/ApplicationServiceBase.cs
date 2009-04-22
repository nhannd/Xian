#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System.Threading;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using System.ServiceModel;
using ClearCanvas.Common.Utilities;
using System;

namespace ClearCanvas.Ris.Application.Services
{
    /// <summary>
    /// Base class for all RIS application services.
    /// </summary>
    /// <remarks>
    /// This class makes several important assumptions:
    /// 1. Instances are never shared across threads.
    /// 2. Instances are used on a per-call basis.  That is, an instance services a single request and is then discarded.
    /// </remarks>
    public abstract class ApplicationServiceBase : IApplicationServiceLayer
    {
        /// <summary>
        /// Cached current-user Staff object.  Caching is acceptable assuming this service instance is not used for more than 1 call.
        /// </summary>
        private Staff _currentUserStaff;
        /// <summary>
        /// Cached working-facility Facility object.  Caching is acceptable assuming this service instance is not used for more than 1 call.
        /// </summary>
        private Facility _workingFacility;

        private bool _workingFacilityLoaded = false;

        /// <summary>
        /// Gets the current user (on whose behalf this service call is executing).
        /// </summary>
        public string CurrentUser
        {
            get { return Thread.CurrentPrincipal.Identity.Name; }
        }

		/// <summary>
		/// Checks that the current user has the specified authority token.
		/// </summary>
		/// <param name="authorityToken"></param>
		/// <returns></returns>
		public bool UserHasToken(string authorityToken)
		{
			return Thread.CurrentPrincipal.IsInRole(authorityToken);
		}

		/// <summary>
		/// Checks that the current user has all of the specified authority tokens.
		/// </summary>
		/// <param name="authorityTokens"></param>
		/// <returns></returns>
		public bool UserHasAllTokens(params string[] authorityTokens)
		{
			return authorityTokens.Length > 0 
				&& CollectionUtils.TrueForAll(authorityTokens,
					delegate(string t) { return UserHasToken(t); });
		}

		/// <summary>
		/// Checks that the current user has at least one of the specified authority tokens.
		/// </summary>
		/// <param name="authorityTokens"></param>
		/// <returns></returns>
		public bool UserHasAnyTokens(params string[] authorityTokens)
		{
			return CollectionUtils.Contains(authorityTokens,
				delegate(string t) { return UserHasToken(t); });
		}

        /// <summary>
        /// Obtains the staff associated with the current user.  If no <see cref="Staff"/> is associated with the current user,
        /// a <see cref="RequestValidationException"/> is thrown.
        /// </summary>
        public Staff CurrentUserStaff
        {
            get
            {
				if (_currentUserStaff == null)
				{
                    StaffSearchCriteria where = new StaffSearchCriteria();
                    where.UserName.EqualTo(CurrentUser);
					_currentUserStaff = CollectionUtils.FirstElement(
						PersistenceContext.GetBroker<IStaffBroker>().Find(
							new StaffSearchCriteria[] { where }, new SearchResultPage(0, 1), true));

					if(_currentUserStaff == null)
                        throw new RequestValidationException(SR.ExceptionNoStaffForUser);
                }

                return _currentUserStaff;
            }
        }

        /// <summary>
        /// Obtains the working facility associated with the current user, or null if 
        /// there is no working facility associated with the current user.
        /// </summary>
        public Facility WorkingFacility
        {
            get
            {
                if(!_workingFacilityLoaded)
                {
                    WorkingFacilitySettings settings = new WorkingFacilitySettings();
                    if (!string.IsNullOrEmpty(settings.WorkingFacilityCode))
                    {
                        FacilitySearchCriteria where = new FacilitySearchCriteria();
                        where.Code.EqualTo(settings.WorkingFacilityCode);

                        // this will be null if the working facility code is invalid, but this should not happen
                        // (and if it does, there is nothing we can do about it)
						_workingFacility = CollectionUtils.FirstElement(PersistenceContext.GetBroker<IFacilityBroker>().Find(
							new FacilitySearchCriteria[] { where }, new SearchResultPage(0, 1), true));
                    }
                    _workingFacilityLoaded = true;
                }
                return _workingFacility;
            }
        }

        /// <summary>
        /// Gets the current <see cref="IPersistenceContext"/>.
        /// </summary>
        public IPersistenceContext PersistenceContext
        {
            get { return PersistenceScope.CurrentContext; }
        }
    }
}
