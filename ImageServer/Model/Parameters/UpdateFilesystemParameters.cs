#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    public class UpdateFilesystemParameters : UpdateBrokerParameters
    {
        public UpdateFilesystemParameters()
            : base("Filesystem")
        {
        }

        public String FilesystemPath
        {
            set { SubParameters["FilesystemPath"] = new UpdateBrokerParameter<String>("FilesystemPath", value); }
        }

        public String Description
        {
            set { SubParameters["Description"] = new UpdateBrokerParameter<String>("Description", value); }
        }

        
        public bool Enabled
        {
            set { SubParameters["Enabled"] = new UpdateBrokerParameter<bool>("Enabled", value); }
        }

        public bool  ReadOnly
        {
            set { SubParameters["ReadOnly"] = new UpdateBrokerParameter<bool>("ReadOnly", value); }
        }

        public bool  WriteOnly
        {
            set { SubParameters["WriteOnly"] = new UpdateBrokerParameter<bool>("WriteOnly", value); }
        }


        public Decimal LowWatermark
        {
            set { SubParameters["LowWatermark"] = new UpdateBrokerParameter<Decimal>("LowWatermark", value); }
        }

        public Decimal HighWatermark
        {
            set { SubParameters["HighWatermark"] = new UpdateBrokerParameter<Decimal>("HighWatermark", value); }
        }

        public Decimal PercentFull
        {
            set { SubParameters["PercentFull"] = new UpdateBrokerParameter<Decimal>("PercentFull", value); }
        }

        public FilesystemTierEnum FilesystemTier
        {
            set { SubParameters["FilesystemTier"] = new UpdateBrokerParameter<ServerEnum>("FilesystemTier", value); }
        }
        
    }
}
