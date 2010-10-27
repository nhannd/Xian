#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model
{
    public partial class Device
    {
        #region Private Members
        private ServerPartition _serverPartition;
        #endregion

        #region Public Properties
        public ServerPartition ServerPartition
        {
            get
            {
                if (_serverPartition == null)
                    _serverPartition = ServerPartition.Load(ServerPartitionKey);
                return _serverPartition;
            }
        }

        #endregion

        /// <summary>
        /// Gets a list of Web Study Move or AutoRoute WorkQueue entries 
        /// that are in progress for this device.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<WorkQueue> GetAllCurrentMoveEntries(IPersistenceContext context)
        {
            IQueryCurrentStudyMove broker = context.GetBroker<IQueryCurrentStudyMove>();
            QueryCurrentStudyMoveParameters criteria = new QueryCurrentStudyMoveParameters();
            criteria.DeviceKey = Key;
            return new List<WorkQueue>(broker.Find(criteria));
        }
    }
}
