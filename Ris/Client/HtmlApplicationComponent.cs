using System;

using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
    public abstract class HtmlApplicationComponent : ApplicationComponent
    {
        /// <summary>
        /// Returns the base URL for HtmlApplication web resources
        /// Returned URL has trailing "/" removed if present
        /// </summary>
        public string Server
        {
            get
            {
                string server = WebResourcesSettings.Default.BaseUrl;
                if (server.EndsWith("/"))
                {
                    server.Remove(server.Length - 1);
                }
                return server;
            }
        }
    }
}
