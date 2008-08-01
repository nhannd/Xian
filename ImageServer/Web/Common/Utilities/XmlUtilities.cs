using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using System.Xml;

namespace ClearCanvas.ImageServer.Web.Common.Utilities
{
    /// <summary>
    /// A Utility class that provides methods related to working with XmlDocuments.
    /// </summary>
    public class XmlUtilities
    {
        /// <summary>
        /// A method for converting and XmlDocument to it's string representation, with the option
        /// of escaping the special characters if desired.
        /// </summary>
        
        public static string GetXmlDocumentAsString(XmlDocument doc, bool escapeChars)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            doc.WriteTo(xw);

            return escapeChars ? SecurityElement.Escape(sw.ToString()) : sw.ToString();
        }
    }
}
