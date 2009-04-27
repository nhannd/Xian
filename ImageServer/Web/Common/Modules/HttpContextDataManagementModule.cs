using System;
using System.Web;

namespace ClearCanvas.ImageServer.Web.Common.Modules
{
    class HttpContextDataManagementModule: IHttpModule
    {
        private HttpApplication _application;

        #region IHttpModule Members

        public void Dispose()
        {
            if (_application!=null)
            {
                _application.EndRequest -= context_EndRequest;
            }
            

        }

        public void Init(HttpApplication context)
        {
            _application = context;
            context.EndRequest += new EventHandler(context_EndRequest);
            
        }

        static void context_EndRequest(object sender, EventArgs e)
        {
            if (HttpContextData.Current!=null)
            {
                HttpContextData.Current.Dispose();
            }
        }

        #endregion
    }
}
