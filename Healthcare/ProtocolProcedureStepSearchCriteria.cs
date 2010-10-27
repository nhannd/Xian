#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    public class ProtocolProcedureStepSearchCriteria : ProcedureStepSearchCriteria
    {
        public ProtocolProcedureStepSearchCriteria()
        {
        }

        public ProtocolProcedureStepSearchCriteria(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected ProtocolProcedureStepSearchCriteria(ProtocolProcedureStepSearchCriteria other)
            : base(other)
        {
        }

        public override object Clone()
        {
            return new ProtocolProcedureStepSearchCriteria(this);
        }

        public ProtocolSearchCriteria Protocol
        {
            get
            {
                if(!this.SubCriteria.ContainsKey("Protocol"))
                {
                    this.SubCriteria["Protocol"] = new ProtocolSearchCriteria("Protocol");
                }
                return (ProtocolSearchCriteria)this.SubCriteria["Protocol"];
            }
        }
    }
}
