#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Model.SelectBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Used to create/update/delete server partition entries in the database.
    /// </summary>
    public class ServerPartitionDataAdapter : BaseAdaptor<ServerPartition,ServerPartitionSelectCriteria,ISelectServerPartition>
    {
        #region Public methods

        /// <summary>
        /// Gets a list of all server partitions.
        /// </summary>
        /// <returns></returns>
        public IList<ServerPartition> GetServerPartitions()
        {
            return Get();
        }

        public IList<ServerPartition> GetServerPartitions(ServerPartitionSelectCriteria criteria)
        {
            return Get(criteria);
        }

        /// <summary>
        /// Creats a new server parition.
        /// </summary>
        /// <param name="partition"></param>
        public bool AddServerPartition(ServerPartition partition)
        {
            bool ok = false;
            IList<ServerPartition> list = null;

            using (IUpdateContext ctx = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IInsertServerPartition insert = ctx.GetBroker<IInsertServerPartition>();
                ServerPartitionInsertParameters parms = new ServerPartitionInsertParameters();
                parms.AeTitle = partition.AeTitle;
                parms.Description = partition.Description;
                parms.Enabled = partition.Enabled;
                parms.PartitionFolder = partition.PartitionFolder;
                parms.Port = partition.Port;
                parms.DefaultRemotePort = partition.DefaultRemotePort;
                parms.AutoInsertDevice = partition.AutoInsertDevice;
                parms.AcceptAnyDevice = partition.AcceptAnyDevice;

                list = insert.Execute(parms);
                ok = list != null && list.Count > 0;

                if (ok)
                    ctx.Commit();
            }

            return ok;
        }

        public bool Update(ServerPartition partition)
        {
            bool ok = false;

            using (IUpdateContext ctx = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IUpdateServerPartition update = ctx.GetBroker<IUpdateServerPartition>();
                ServerPartitionUpdateParameters parms = new ServerPartitionUpdateParameters();
                parms.ServerPartitionGUID = partition.GetKey();
                parms.AeTitle = partition.AeTitle;
                parms.Description = partition.Description;
                parms.Enabled = partition.Enabled;
                parms.PartitionFolder = partition.PartitionFolder;
                parms.Port = partition.Port;
                parms.AcceptAnyDevice = partition.AcceptAnyDevice;
                parms.AutoInsertDevice = partition.AutoInsertDevice;
                parms.DefaultRemotePort = partition.DefaultRemotePort;

                ok = update.Execute(parms);

                if (ok)
                    ctx.Commit();
            }

            return ok;
        }

        #endregion Public methods
    }
}

