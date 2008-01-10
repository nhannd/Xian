using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Threading;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Records information about an exception.
    /// </summary>
    public class ExceptionLogEntry : Entity
    {
        private DateTime _timestamp;
        private string _user;
        private string _operation;
        private string _exceptionClass;
        private string _message;
        private string _details;


        /// <summary>
        /// Private no-args constructor to support NHibernate
        /// </summary>
        private ExceptionLogEntry()
        {

        }

        public ExceptionLogEntry(string operation, Exception e, string details)
        {
            _timestamp = Platform.Time;
            _user = Thread.CurrentPrincipal.Identity.Name;
            _operation = operation;
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
        /// User that invoked the service that threw the exception
        /// </summary>
        public string User
        {
            get { return _user; }
            private set { _user = value; }
        }

        /// <summary>
        /// The operation that was executing when the exception was thrown.
        /// </summary>
        public string Operation
        {
            get { return _operation; }
            private set { _operation = value; }
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
