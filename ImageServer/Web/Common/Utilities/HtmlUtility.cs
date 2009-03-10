using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Web;

namespace ClearCanvas.ImageServer.Web.Common.Utilities
{
    public static class HtmlUtility
    {
    	///
    	/// Encode a string so that it is suitable for rendering in an Html page.
    	/// Also ensure all Xml escape characters are encoded properly.
        public static string Encode(string text)
        {
            if (text == null) return string.Empty;
            String encodedText = new SecurityElement("dummy", text).Text; //decode any escaped xml characters.
            return HttpUtility.HtmlEncode(encodedText).Replace(Environment.NewLine, "<BR/>");
            
        }
    }
}
