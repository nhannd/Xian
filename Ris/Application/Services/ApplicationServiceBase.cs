using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Application.Services
{
    public abstract class ApplicationServiceBase : IApplicationServiceLayer
    {
        private static Staff _currentUserStaff;

        protected Staff CurrentUserStaff
        {
            //TODO: a stub for getting current the user staff
            get
            {
                if (_currentUserStaff == null)
                    _currentUserStaff = PersistenceContext.GetBroker<IStaffBroker>().FindOne(new StaffSearchCriteria());

                return _currentUserStaff;
            }
        }

        protected IPersistenceContext PersistenceContext
        {
            get { return PersistenceScope.Current; }
        }
    }
}
