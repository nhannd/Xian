#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.Brokers;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    /// <summary>
    /// Input parameters for <see cref="IInsertFilesystem"/>.
    /// </summary>
    public class FilesystemInsertParameters : ProcedureParameters
    {
        public FilesystemInsertParameters()
            : base("InsertFilesystem")
        {
        }

        public FilesystemTierEnum TypeEnum
        {
            set { SubCriteria["FilesystemTierEnum"] = new ProcedureParameter<ServerEnum>("FilesystemTierEnum", value); }
        }

        public String FilesystemPath
        {
            set { SubCriteria["FilesystemPath"] = new ProcedureParameter<String>("FilesystemPath", value); }
        }

        public String Description
        {
            set { SubCriteria["Description"] = new ProcedureParameter<String>("Description", value); }
        }

        public bool Enabled
        {
            set { SubCriteria["Enabled"] = new ProcedureParameter<bool>("Enabled", value); }
        }

        public bool ReadOnly
        {
            set { SubCriteria["ReadOnly"] = new ProcedureParameter<bool>("ReadOnly", value); }
        }

        public bool WriteOnly
        {
            set { SubCriteria["WriteOnly"] = new ProcedureParameter<bool>("WriteOnly", value); }
        }

        public Decimal HighWatermark
        {
            set { SubCriteria["HighWatermark"] = new ProcedureParameter<Decimal>("HighWatermark", value); }
        }

        public Decimal LowWatermark
        {
            set { SubCriteria["LowWatermark"] = new ProcedureParameter<Decimal>("LowWatermark", value); }
        }

        public Decimal PercentFull
        {
            set { SubCriteria["PercentFull"] = new ProcedureParameter<Decimal>("PercentFull", value); }
        }
    }
}
