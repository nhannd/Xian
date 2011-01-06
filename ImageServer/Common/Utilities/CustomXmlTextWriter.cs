#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using System.Xml;

namespace ClearCanvas.ImageServer.Common.Utilities
{
    /// <summary>
    /// Customized <see cref="XmlTextWriter"/> class that ensures the output xml
    /// is correctly encoded.
    /// </summary>
    /// <remarks>
    /// <see cref="CustomXmlTextWriter"/> escape character (0x1B) from the string value
    /// when serializing the data.
    /// </remarks>
    class CustomXmlTextWriter:XmlTextWriter
    {
        #region Constructors
        public CustomXmlTextWriter(TextWriter writer)
            : base(writer)
        {

        } 
        #endregion

        #region Overridden Public Methods
        public override void WriteString(string text)
        {
            string escapeChar = "\x1B";
            if (text!=null && text.Contains(escapeChar))
            {
                base.WriteString(text.Replace(escapeChar, string.Empty));
            }
            else
                base.WriteString(text);
        } 
        #endregion
    }
}