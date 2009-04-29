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

using ClearCanvas.Enterprise.Core;
using System;

namespace ClearCanvas.Healthcare
{
    public class WorklistItemSearchCriteria : SearchCriteria
    {
        private Type _procedureStepClass;
        private WorklistTimeField _timeField;

		public WorklistItemSearchCriteria()
		{
		}

    	public WorklistItemSearchCriteria(Type procedureStepClass)
    	{
    		_procedureStepClass = procedureStepClass;
    	}

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected WorklistItemSearchCriteria(WorklistItemSearchCriteria other)
            : base(other)
        {
        }

        public override object Clone()
        {
            return new WorklistItemSearchCriteria(this);
        }


    	public PatientProfileSearchCriteria PatientProfile
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("PatientProfile"))
                {
                    this.SubCriteria["PatientProfile"] = new PatientProfileSearchCriteria("PatientProfile");
                }
                return (PatientProfileSearchCriteria)this.SubCriteria["PatientProfile"];
            }
        }

        public OrderSearchCriteria Order
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Order"))
                {
                    this.SubCriteria["Order"] = new OrderSearchCriteria("Order");
                }
                return (OrderSearchCriteria)this.SubCriteria["Order"];
            }
        }

        public ProcedureSearchCriteria Procedure
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Procedure"))
                {
                    this.SubCriteria["Procedure"] = new ProcedureSearchCriteria("Procedure");
                }
                return (ProcedureSearchCriteria)this.SubCriteria["Procedure"];
            }
        }

        public ProcedureStepSearchCriteria ProcedureStep
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("ProcedureStep"))
                {
                    this.SubCriteria["ProcedureStep"] = new ProcedureStepSearchCriteria("ProcedureStep");
                }
                return (ProcedureStepSearchCriteria)this.SubCriteria["ProcedureStep"];
            }
        }

        public Type ProcedureStepClass
        {
            get { return _procedureStepClass; }
            set { _procedureStepClass = value; }
        }

        public WorklistTimeField TimeField
        {
            get { return _timeField; }
            set { _timeField = value; }
        }
    }
}
