using System.Threading;
using ClearCanvas.Enterprise.Authentication;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public abstract class ApplicationServiceBase : IApplicationServiceLayer
    {
        private static Staff _currentUserStaff;
        private static User _currentUser;

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

        /// <summary>
        /// Obtains the <see cref="User"/> entity for the current user.
        /// </summary>
        /// <exception cref="RequestValidationException">Thrown if no <see cref="User"/> is found, though this should not happen.</exception>
        protected User CurrentUser
        {
            get
            {
                try
                {
                    UserSearchCriteria criteria = new UserSearchCriteria();
                    criteria.UserName.EqualTo(Thread.CurrentPrincipal.Identity.Name);
                    _currentUser = this.PersistenceContext.GetBroker<IUserBroker>().FindOne(criteria);
                }
                catch (EntityNotFoundException)
                {
                    throw new RequestValidationException(SR.ExceptionNoUser);
                }

                return _currentUser;
            }
        }

        protected IPersistenceContext PersistenceContext
        {
            get { return PersistenceScope.Current; }
        }
    }
}
