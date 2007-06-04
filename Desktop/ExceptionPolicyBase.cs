using System;

namespace ClearCanvas.Desktop
{
    public class ExceptionPolicyBase : IExceptionPolicy
    {
        #region IExceptionPolicy Members

        public virtual ExceptionReport Handle(Exception e)
        {
            return Handle(e, null);
        }

        public virtual ExceptionReport Handle(Exception e, string userMessage)
        {
            string message = string.IsNullOrEmpty(userMessage) ? e.Message : userMessage;

            return new ExceptionReport(message, ExceptionReportAction.ReportInDialog);
        }

        #endregion
    }
}
