using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using System.Threading;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public abstract class ApplicationServiceBase : IApplicationServiceLayer
    {
        private static Staff _currentUserStaff;

        /// <summary>
        /// Obtains the staff associated with the current user.  If no <see cref="Staff"/> is associated with the current user,
        /// a <see cref="RequestValidationException"/> is thrown.
        /// </summary>
        protected Staff CurrentUserStaff
        {
            get
            {
                if (_currentUserStaff == null)
                {
                    try
                    {
                        _currentUserStaff = PersistenceContext.GetBroker<IStaffBroker>().FindStaffForUser(Thread.CurrentPrincipal.Identity.Name);
                    }
                    catch (EntityNotFoundException)
                    {
                        throw new RequestValidationException(SR.ExceptionNoStaffForUser);
                    }
                }

                return _currentUserStaff;
            }
        }

        protected IPersistenceContext PersistenceContext
        {
            get { return PersistenceScope.Current; }
        }
    }
}
