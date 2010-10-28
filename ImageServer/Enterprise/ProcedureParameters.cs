#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise
{
    public class ProcedureParameters : SearchCriteria
    {
        public ProcedureParameters(string key)
            : base(key)
        {
        }


        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected ProcedureParameters(ProcedureParameters other)
            : base(other)
        {
        }

        public override object Clone()
        {
            return new ProcedureParameters(this);
        }
    }
}
