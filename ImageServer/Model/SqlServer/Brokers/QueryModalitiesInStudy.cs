#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise.SqlServer;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model.SqlServer.Brokers
{
    /// <summary>
    /// Broker implementation for <see cref="IQueryModalitiesInStudy"/>
    /// </summary>
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class QueryModalitiesInStudy : ProcedureQueryBroker<ModalitiesInStudyQueryParameters, Series>, IQueryModalitiesInStudy
    {
        public QueryModalitiesInStudy()
            : base("QueryModalitiesInStudy")
        {
        }
    }
}