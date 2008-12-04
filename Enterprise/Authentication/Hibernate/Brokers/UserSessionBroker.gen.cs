// This file is machine generated - changes will be lost.
using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Authentication.Brokers;

namespace ClearCanvas.Enterprise.Authentication.Hibernate.Brokers
{
    /// <summary>
    /// NHibernate implementation of <see cref="IUserSessionBroker"/>.
    /// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(BrokerExtensionPoint))]
	public partial class UserSessionBroker : EntityBroker<UserSession, UserSessionSearchCriteria>, IUserSessionBroker
	{
	}
}