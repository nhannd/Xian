using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    ///  Class used to convert <seealso cref="BaseStatistics"/> to other types.
    /// </summary>
    public class BasicStatisticsConverter : TypeConverter
    {

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            else
                return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                // spit out all properties as XML
                BaseStatistics stats = value as BaseStatistics;

                StringBuilder sb = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = ("\t");

                using (XmlWriter writer = XmlWriter.Create(sb, settings))
                {
                    stats.WriteXml(writer);
                    writer.Flush();
                }

                return sb.ToString();
            }
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
