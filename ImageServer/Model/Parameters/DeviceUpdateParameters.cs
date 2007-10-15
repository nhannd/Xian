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

using ClearCanvas.ImageServer.Enterprise;

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
