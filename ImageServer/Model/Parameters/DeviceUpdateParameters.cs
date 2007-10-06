using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class DeviceUpdateParameters : ProcedureParameters
    {
        public DeviceUpdateParameters()
            : base("Device")
        {
        }

        public ServerEntityKey DeviceKey
        {
            set { this.SubCriteria["GUID"] = new ProcedureParameter<ServerEntityKey>("GUID", value); }
            get { return ((ProcedureParameter<ServerEntityKey>)this.SubCriteria["GUID"]).Value; }
        }

        //public string ServerPartitionGUID
        //{
        //    set { this.SubCriteria["ServerPartitionGUID"] = new ProcedureParameter<string>("ServerPartitionGUID", value); }
        //    get { return ((ProcedureParameter<string>)this.SubCriteria["ServerPartitionGUID"]).Value; }
        //}
        public ServerEntityKey ServerPartitionKey
        {
            set { this.SubCriteria["ServerPartitionGUID"] = new ProcedureParameter<ServerEntityKey>("ServerPartitionGUID", value); }
            get { return ((ProcedureParameter<ServerEntityKey>)this.SubCriteria["ServerPartitionGUID"]).Value; }
            
        }

        public string AETitle
        {
            set { this.SubCriteria["AETitle"] = new ProcedureParameter<string>("AETitle", value); }
            get { return ((ProcedureParameter<string>)this.SubCriteria["AETitle"]).Value; }
        }

        public string IPAddress
        {
            set { this.SubCriteria["IPAddress"] = new ProcedureParameter<string>("IPAddress", value); }
            get { return ((ProcedureParameter<string>)this.SubCriteria["IPAddress"]).Value; }
        }


        public int Port
        {
            set { this.SubCriteria["Port"] = new ProcedureParameter<int>("Port", value); }
            get { return ((ProcedureParameter<int>)this.SubCriteria["Port"]).Value; }
        }

        
        public string Description
        {
            set { this.SubCriteria["Description"] = new ProcedureParameter<string>("Description", value); }
            get { return ((ProcedureParameter<string>)this.SubCriteria["Description"]).Value; }
        }
        public bool DHCP
        {
            set { this.SubCriteria["DHCP"] = new ProcedureParameter<bool>("DHCP", value); }
            get { return ((ProcedureParameter<bool>)this.SubCriteria["DHCP"]).Value; }
        }

        public bool Active
        {
            set { this.SubCriteria["Active"] = new ProcedureParameter<bool>("Active", value); }
            get { return ((ProcedureParameter<bool>)this.SubCriteria["Active"]).Value; }
        }

    }
}
