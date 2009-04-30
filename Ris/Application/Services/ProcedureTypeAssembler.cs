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

using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using System.Xml;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
    public class ProcedureTypeAssembler
    {
		public ProcedureTypeSummary CreateSummary(ProcedureType rpt)
        {
            return new ProcedureTypeSummary(rpt.GetRef(), rpt.Name, rpt.Id, rpt.Deactivated);
        }

		public ProcedureTypeDetail CreateDetail(ProcedureType procedureType)
        {
			// write plan to string
			string planXml;
			StringBuilder sb = new StringBuilder();
			using (XmlTextWriter writer = new XmlTextWriter(new StringWriter(sb)))
			{
				writer.Formatting = Formatting.Indented;
				procedureType.GetPlanXml().Save(writer);
				planXml = sb.ToString();
			}

            return new ProcedureTypeDetail(
                procedureType.GetRef(),
                procedureType.Id,
                procedureType.Name,
				procedureType.BaseType == null ? null : CreateSummary(procedureType.BaseType),
				planXml,
				procedureType.Deactivated);
        }

		public void UpdateProcedureType(ProcedureType procType, ProcedureTypeDetail detail, IPersistenceContext context)
		{
			procType.Id = detail.Id;
			procType.Name = detail.Name;
			procType.BaseType = detail.BaseType == null
			                    	? null
			                    	: context.Load<ProcedureType>(detail.BaseType.ProcedureTypeRef, EntityLoadFlags.Proxy);
			procType.Deactivated = detail.Deactivated;

			try
			{
				XmlDocument xmlPlan = new XmlDocument();
				xmlPlan.LoadXml(detail.PlanXml);
				procType.SetPlanXml(xmlPlan);
			}
			catch (XmlException e)
			{
				throw new RequestValidationException(string.Format("Procedure plan XML is invalid: {0}", e.Message));
			}
		}
    }
}
