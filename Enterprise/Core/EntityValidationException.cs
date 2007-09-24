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

        public EntityValidationException(string message, TestResultReason[] reasons)
            :base(message)
        {
            _reasons = reasons;
        }

        public TestResultReason[] Reasons
        {
            get { return _reasons; }
        }
    }
}
