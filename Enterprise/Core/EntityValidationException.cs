using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core
{
    public class EntityValidationException : Exception
    {
        private TestResultReason[] _reasons;

        public EntityValidationException(TestResultReason[] reasons)
        {
            _reasons = reasons;
        }

        public override string Message
        {
            get
            {
                // return the first reaons
                // TODO: should we concatenate all top-level reasons???
                return _reasons.Length > 0 ? _reasons[0].Message : "Unknown reason";
            }
        }
    }
}
