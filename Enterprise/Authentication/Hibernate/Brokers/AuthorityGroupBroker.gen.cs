// This file is machine generated - changes will be lost.
using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Hibernate;
using ClearCanvas.Enterprise.Authentication.Brokers;

namespace ClearCanvas.Enterprise.Authentication.Hibernate.Brokers
{
    /// <summary>
    /// NHibernate implementation of <see cref="IAuthorityGroupBroker"/>.
    /// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(BrokerExtensionPoint))]
	public partial class AuthorityGroupBroker : EntityBroker<AuthorityGroup, AuthorityGroupSearchCriteria>, IAuthorityGroupBroker
	{
	}
}