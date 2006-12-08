using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public class TestResultReason
    {
        private string _message;
        private TestResultReason _reason;

        public TestResultReason(string message)
            :this(message, null)
        {
        }

        public TestResultReason(string message, TestResultReason reason)
        {
            _message = message;
            _reason = reason;
        }

        public string Message
        {
            get { return _message; }
        }

        public TestResultReason Reason
        {
            get { return _reason; }
        }
    }
}
