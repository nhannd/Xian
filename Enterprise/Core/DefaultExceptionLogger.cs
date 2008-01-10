using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace ClearCanvas.Enterprise.Core
{
    public class DefaultExceptionLogger : IExceptionLogger
    {
        public DefaultExceptionLogger()
        {

        }

        #region IExceptionLogger Members

        public ExceptionLogEntry CreateExceptionLogEntry(string operation, Exception e)
        {
            return new ExceptionLogEntry(operation, e, WriteXml(e));
        }

        #endregion

        private string WriteXml(Exception e)
        {
            StringWriter sw = new StringWriter();
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("exception");
                WriteExceptionXml(writer, e);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                return sw.ToString();
            }
        }

        private void WriteExceptionXml(XmlWriter writer, Exception e)
        {
            writer.WriteElementString("message", e.Message);
            writer.WriteElementString("source", e.Source);
            writer.WriteStartElement("stack-trace");
            writer.WriteCData(e.StackTrace);
            writer.WriteEndElement();

            if (e.InnerException != null)
            {
                writer.WriteStartElement("inner-exception");
                WriteExceptionXml(writer, e.InnerException);
                writer.WriteEndElement();
            }
        }
    }
}
