using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Specifications;
using System.Security.Principal;
using ClearCanvas.Common;
using System.Threading;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// An implementation of <see cref="ISpecification"/> that tests if the current thread principal is in a given role.
    /// </summary>
    public class PrincipalPermissionSpecification : ISpecification
    {
        private string _role;

        /// <summary>
        /// Constructs an instance of this class for the specified role.
        /// </summary>
        /// <param name="role"></param>
        public PrincipalPermissionSpecification(string role)
        {
            _role = role;
        }

        #region ISpecification Members

        /// <summary>
        /// Tests the <see cref="Thread.CurrentPrincipal"/> for the permission represented by this object.  Note that the
        /// argument obj is ignored.
        /// </summary>
        /// <param name="obj">Ignored</param>
        /// <returns></returns>
        public TestResult Test(object obj)
        {
            return new TestResult(Thread.CurrentPrincipal == null ? false : Thread.CurrentPrincipal.IsInRole(_role));
        }

        public IEnumerable<ISpecification> SubSpecs
        {
            get { return new ISpecification[0]; }
        }

        #endregion
    }
}
