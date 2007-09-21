using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public class TestResult
    {
        private bool _success;
        private TestResultReason[] _reasons;

        public TestResult(bool success)
            : this(success, new TestResultReason[] {})
        {
        }

        public TestResult(bool success, TestResultReason reason)
            :this(success, new TestResultReason[] { reason })
        {
        }

        public TestResult(bool success, TestResultReason[] reasons)
        {
            _success = success;
            _reasons = reasons;
        }

        public bool Success
        {
            get { return _success; }
        }

        public bool Fail
        {
            get { return !_success; }
        }

        public TestResultReason[] Reasons
        {
            get { return _reasons; }
        }
    }
}
