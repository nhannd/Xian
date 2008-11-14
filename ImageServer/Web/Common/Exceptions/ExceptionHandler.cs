using System;
using System.Web;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Web.Common.Exceptions
{
    public class ExceptionHandler
    {
        private static ExceptionHandler instance = new ExceptionHandler();
        private static HttpContext context;
 
        public static ExceptionHandler Instance()
        {
            return instance;
        }
 
        //instance members
        private ExceptionHandler()
        {
            context = HttpContext.Current;
        }

        public static void ThrowException(Exception e)
        {
            context = HttpContext.Current;
            
            Exception baseException = context.Server.GetLastError();
            if (baseException != null)
            {
                baseException = baseException.GetBaseException();

                context.Server.ClearError();

                string message = baseException.Message;
                string source = baseException.Source;
                string stackTrace = baseException.StackTrace;

                string logMessage = string.Format("Message: {0}\nSource:{1}\nStack Trace:{2}", message, source, stackTrace);
                Platform.Log(LogLevel.Error, logMessage);
                context.Items.Add(ImageServerConstants.ContextKeys.StackTrace, logMessage);
            }
            context.Server.Transfer(ImageServerConstants.PageURLs.ErrorPage);   
        }

        public static void ThrowException(BaseWebException e)
        {
            if (e != null)
            {                
                context = HttpContext.Current;

                context.Server.ClearError();

                string logMessage = string.Format("<Error>\n\t<CustomMessage>{0}</CustomMessage>\n\t<SystemMessage>{1}</SystemMessage>\n\t<Source>{2}</Source>\n\t<StackTrace>{3}</StackTrace>", e.LogMessage, e.Message, e.Source, e.StackTrace);
                Platform.Log(LogLevel.Error, logMessage);
                
                context.Items.Add(ImageServerConstants.ContextKeys.StackTrace, logMessage);

                if(e.ErrorMessage != null && !e.ErrorMessage.Equals(string.Empty))
                    context.Items.Add(ImageServerConstants.ContextKeys.ErrorMessage, e.ErrorMessage);
                if (e.ErrorDescription != null && !e.ErrorDescription.Equals(string.Empty))
                    context.Items.Add(ImageServerConstants.ContextKeys.ErrorDescription, e.ErrorDescription);
                
                context.Server.Transfer(ImageServerConstants.PageURLs.ErrorPage);   
            }
        }

        public static string ThrowAJAXException(Exception e)
        {
            Exception baseException = e.GetBaseException();

            string message = baseException.Message;
            string source = baseException.Source;
            string stackTrace = baseException.StackTrace;

            if (e.Data["ExtraInfo"] != null)
            {
                message += "\nExtra Info: " + e.Data["ExtraInfo"].ToString();
            }
            else
            {
                message += "\nExtra Info: " + "An unspecified error occurred.";
            }

            string logMessage = string.Format("Message: {0}\nSource:{1}\nStack Trace:{2}", message, source, stackTrace);
            Platform.Log(LogLevel.Error, logMessage);

            return logMessage;
        }
    }
}
