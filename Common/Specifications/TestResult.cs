using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public class TestResult
    {
        private bool _success;
        private TestResultReason _reason;

        public TestResult(bool success)
            : this(success, null)
        {
        }

        public TestResult(bool success, TestResultReason reason)
        {
            _success = success;
            _reason = reason;
        }

        public bool Success
        {
            get { return _success; }
        }

        public bool Fail
        {
            get { return !_success; }
        }

        public TestResultReason Reason
        {
            get { return _reason; }
        }
    }
}
