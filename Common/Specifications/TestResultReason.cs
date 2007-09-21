using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public class TestResultReason
    {
        private string _message;
        private TestResultReason[] _reasons;

        public TestResultReason(string message)
            : this(message, new TestResultReason[] {})
        {
        }

        public TestResultReason(string message, TestResultReason reason)
            : this(message, new TestResultReason[] { reason })
        {
        }

        public TestResultReason(string message, TestResultReason[] reasons)
        {
            _message = message;
            _reasons = reasons;
        }

        public string Message
        {
            get { return _message; }
        }

        public TestResultReason[] Reasons
        {
            get { return _reasons; }
        }
    }
}
