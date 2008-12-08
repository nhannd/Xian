using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ClearCanvas.ImageServer.Web.Application.Helpers
{
    public static class HtmlEncoder
    {
        static public string EncodeText(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            return HttpUtility.HtmlEncode(text).Replace("\n", "<BR/>");
        }
    }
}
