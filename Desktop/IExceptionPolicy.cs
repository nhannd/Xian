using System;
using System.Collections.Generic;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    [ExtensionPoint()]
    public sealed class ExceptionPolicyExtensionPoint : ExtensionPoint<IExceptionPolicy> { }

    /// <summary>
    /// 
    /// </summary>
    public interface IExceptionPolicy
    {
        ExceptionReport Handle(Exception e);
        ExceptionReport Handle(Exception e, string userMessage);
    }

    public enum ExceptionReportAction
    {
        ReportInDialog,
        Ignore
    }

    public class ExceptionReport
    {
        public ExceptionReport(string message, ExceptionReportAction action)
        {
            _message = message;
            _action = action;
        }

        private ExceptionReportAction _action;
        private string _message;

        public ExceptionReportAction Action
        {
            get { return _action; }
        }

        public string Message
        {
            get { return _message; }
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ExceptionPolicyForAttribute : Attribute
    {
        private Type _exceptionType;

        public ExceptionPolicyForAttribute(Type exceptionType)
        {
            _exceptionType = exceptionType;
        }

        public Type ExceptionType
        {
            get { return _exceptionType; }
        }
    }
}

