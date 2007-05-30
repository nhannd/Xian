using System;

using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
    public abstract class HtmlApplicationComponent : ApplicationComponent
    {
        /// <summary>
        /// Returns the base URL for HtmlApplication web resources
        /// URL is prefixed by 'http://' and does not end with a "/"
        /// </summary>
        public string Server
        {
            get
            {
                string server = WebResourcesSettings.Default.BaseUrl;
                if (!server.StartsWith("http://"))
                {
                    server = "http://" + server;
                }
                if (server.EndsWith("/"))
                {
                    server.Remove(server.Length - 1);
                }
                return server;
            }
        }
    }
}
