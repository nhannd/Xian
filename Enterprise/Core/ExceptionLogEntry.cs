using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Records information about an exception.
    /// </summary>
    public class ExceptionLogEntry : Entity
    {
        private DateTime _timestamp;
        private string _exceptionClass;
        private string _message;
        private string _details;


        /// <summary>
        /// Private no-args constructor to support NHibernate
        /// </summary>
        private ExceptionLogEntry()
        {

        }

        public ExceptionLogEntry(Exception e, string details)
        {
            _timestamp = Platform.Time;
            _exceptionClass = e.GetType().FullName;
            _message = e.Message;
            _details = details;
        }

        /// <summary>
        /// Time that the exception was thrown.
        /// </summary>
        public DateTime TimeStamp
        {
            get { return _timestamp; }
            private set { _timestamp = value; }
        }

        /// <summary>
        /// Name of the exception class.
        /// </summary>
        public string ExceptionClass
        {
            get { return _exceptionClass; }
            private set { _exceptionClass = value; }
        }

        /// <summary>
        /// Top-level message exposed by the exception.
        /// </summary>
        public string Message
        {
            get { return _message; }
            private set { _message = value; }
        }

        /// <summary>
        /// Details of the exception, including stack-trace.
        /// </summary>
        public string Details
        {
            get { return _details; }
            private set { _details = value; }
        }
    }
}
