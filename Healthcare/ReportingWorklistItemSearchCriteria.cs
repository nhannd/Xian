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

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    public class ReportingWorklistItemSearchCriteria : WorklistItemSearchCriteria
    {
        public ReportPartSearchCriteria ReportPart
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("ReportPart"))
                {
                    this.SubCriteria["ReportPart"] = new ReportPartSearchCriteria("ReportPart");
                }
                return (ReportPartSearchCriteria)this.SubCriteria["ReportPart"];
            }
        }

        public ProtocolSearchCriteria Protocol
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Protocol"))
                {
                    this.SubCriteria["Protocol"] = new ProtocolSearchCriteria("Protocol");
                }
                return (ProtocolSearchCriteria)this.SubCriteria["Protocol"];
            }
        }

        public ISearchCondition<bool> HasErrors
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("HasErrors"))
                {
                    this.SubCriteria["HasErrors"] = new SearchCondition<bool>("HasErrors");
                }
                return (ISearchCondition<bool>)this.SubCriteria["HasErrors"];
            }
        }
    }
}
