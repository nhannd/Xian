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
using System.ComponentModel;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Model.SelectBrokers;

/// <summary>
/// Summary description for DeviceDataAdapter
/// </summary>
/// 
namespace ClearCanvas.ImageServer.Web.Common
{
    public class DeviceDataAdapter
    {

        private IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();


        public DeviceDataAdapter()
        {
            
        }

        public IList<Device> GetDevices()
        {

            IReadContext read = _store.OpenReadContext();
            ISelectDevice find = read.GetBroker<ISelectDevice>();
            DeviceSelectCriteria criteria = new DeviceSelectCriteria();
            IList<Device> list = find.Find(criteria);
            read.Dispose();
            return list;
        }


        public void Update(Device dev)
        {

            IReadContext ctx = _store.OpenReadContext();
            IUpdateDevice update = ctx.GetBroker<IUpdateDevice>();
            DeviceUpdateParameters param = new DeviceUpdateParameters();
            param.DeviceKey = dev.GetKey();
            param.ServerPartitionKey = dev.ServerPartitionKey;
            param.Active = dev.Active;
            param.AETitle = dev.AeTitle;
            param.Description = dev.Description;
            param.DHCP = dev.Dhcp;
            param.IPAddress = dev.IpAddress;
            param.Port = dev.Port;



            update.Execute(param);

        }

        public IList<Device> DummyList
        {
            get
            {
                // return dummy list
                List<Device> list = new List<Device>();
                Device dev = new Device();
                dev.AeTitle = "Checking";

                dev.ServerPartitionKey = new ServerEntityKey("Testing", "Checking");
                list.Add(dev);

                return list;
            }
        }

        public IList<Device> GetDevices(string AETitle, ServerEntityKey serverPartitionkey)
        {
            IReadContext read = _store.OpenReadContext();
            ISelectDevice find = read.GetBroker<ISelectDevice>();
            DeviceSelectCriteria criteria = new DeviceSelectCriteria();
            criteria.ServerPartitionKey.EqualTo(serverPartitionkey);
            IList<Device> list = find.Find(criteria);
            return list;
        }

        public void AddDevice(string AETitle, string Description, string IPAddress, int Port, bool Active, bool DHCP, string ServerPartitionGUID)
        {
            IReadContext ctx = _store.OpenReadContext();

            IInsertDevice insert = ctx.GetBroker<IInsertDevice>();
            DeviceInsertParameters param = new DeviceInsertParameters();
            param.ServerPartitionKey = new ServerEntityKey("ServerPartition", ServerPartitionGUID);
            param.AeTitle = AETitle;
            param.Description = Description;
            param.IpAddress = IPAddress;
            param.Port = Port;
            param.Active = Active;
            param.Dhcp = DHCP;

            insert.Execute(param);
        }

        public void AddDevice(Device newDev)
        {
            IReadContext ctx = _store.OpenReadContext();

            IInsertDevice insert = ctx.GetBroker<IInsertDevice>();
            DeviceInsertParameters param = new DeviceInsertParameters();
            param.ServerPartitionKey = newDev.ServerPartitionKey;
            param.AeTitle = newDev.AeTitle;
            param.Description = newDev.Description;
            param.IpAddress = newDev.IpAddress;
            param.Port = newDev.Port;
            param.Active = newDev.Active;
            param.Dhcp = newDev.Dhcp;

            insert.Execute(param);
        }



    }
}

