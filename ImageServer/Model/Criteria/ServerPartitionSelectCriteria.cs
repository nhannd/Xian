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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Criteria
{
    public class ServerPartitionSelectCriteria : SelectCriteria
    {
        public ServerPartitionSelectCriteria()
            : base("ServerPartition")
        {}


        public ISearchCondition<ServerEntityKey> Key
        {
            get
            {
                if (!SubCriteria.ContainsKey("Key"))
                {
                    SubCriteria["Key"] = new SearchCondition<ServerEntityKey>("Key");
                }
                return (ISearchCondition<ServerEntityKey>)SubCriteria["Key"];
            }
        }

        public ISearchCondition<string> AETitle
        {
            get
            {
                if (!SubCriteria.ContainsKey("AETitle"))
                {
                    SubCriteria["AETitle"] = new SearchCondition<string>("AETitle");
                }
                return (ISearchCondition<string>)SubCriteria["AETitle"];
            }
        }

        public ISearchCondition<string> Description
        {
            get
            {
                if (!SubCriteria.ContainsKey("Description"))
                {
                    SubCriteria["Description"] = new SearchCondition<string>("Description");
                }
                return (ISearchCondition<string>)SubCriteria["Description"];
            }
        }

        public ISearchCondition<string> PartitionFolder
        {
            get
            {
                if (!SubCriteria.ContainsKey("PartitionFolder"))
                {
                    SubCriteria["PartitionFolder"] = new SearchCondition<string>("PartitionFolder");
                }
                return (ISearchCondition<string>)SubCriteria["PartitionFolder"];
            }
        }

        public ISearchCondition<int> Port
        {
            get
            {
                if (!SubCriteria.ContainsKey("Port"))
                {
                    SubCriteria["Port"] = new SearchCondition<int>("Port");
                }
                return (ISearchCondition<int>)SubCriteria["Port"];
            }
        }

        public ISearchCondition<bool> Enabled
        {
            get
            {
                if (!SubCriteria.ContainsKey("Enabled"))
                {
                    SubCriteria["Enabled"] = new SearchCondition<bool>("Enabled");
                }
                return (ISearchCondition<bool>)SubCriteria["Enabled"];
            }
        }
    }
}
