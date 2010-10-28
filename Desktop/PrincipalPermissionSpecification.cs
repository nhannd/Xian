#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// An implementation of <see cref="ISpecification"/> that tests if the current thread principal is in a given role.
    /// </summary>
    public class PrincipalPermissionSpecification : ISpecification
    {
        private readonly string _role;

        /// <summary>
        /// Constructs an instance of this class for the specified role.
        /// </summary>
        public PrincipalPermissionSpecification(string role)
        {
            _role = role;
        }

        #region ISpecification Members

        /// <summary>
        /// Tests the <see cref="Thread.CurrentPrincipal"/> for the permission represented by this object.
        /// </summary>
		/// <remarks>
		/// If the application is running in non-authenticated (stand-alone) mode, the test will always
		/// succeed.  If the application is running in authenticated (enterprise) mode, the test succeeds only
		/// if the thread current principal is in the role assigned to this instance.
		/// </remarks>
		/// <param name="obj">This parameter is ignored.</param>
        public TestResult Test(object obj)
        {
			// if the thread is running in a non-authenticated mode, then we have no choice but to allow.
			// this seems a little counter-intuitive, but basically we're counting on the fact that if
			// the desktop is running in an enterprise environment, then the thread *will* be authenticated,
			// and that this is enforced by some mechanism outside the scope of this class.  The only
			// scenario in which the thread would ever be unauthenticated is the stand-alone scenario.
			if(Thread.CurrentPrincipal == null || Thread.CurrentPrincipal.Identity.IsAuthenticated == false)
				return new TestResult(true);

			// if running in authenticated mode, test the role
            return new TestResult(Thread.CurrentPrincipal.IsInRole(_role));
        }

        #endregion
    }
}
