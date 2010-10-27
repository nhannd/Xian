#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using System.Text;
using System.Xml;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Provides statistics logging mechanism.
    /// </summary>
    public class StatisticsLogger
    {
        private static readonly XmlDocument doc = new XmlDocument();
        private static readonly object[] _extensions;
        private static readonly StatisticsLoggerExtensionPoint _xp = new StatisticsLoggerExtensionPoint();

        static StatisticsLogger()
        {
			try
			{
				_extensions = _xp.CreateExtensions();
			}
			catch (PluginException)
			{
				_extensions = new object[0];
			}
        }

		/// <summary>
        /// Logs a statistics.
        /// </summary>
        /// <param name="level">The log level used for logging the statistics</param>
        /// <param name="recursive">Bool telling if the log should be recursive, or just display averages.</param>
        /// <param name="statistics">The statistics to be logged</param>
		public static void Log(LogLevel level, bool recursive, StatisticsSet statistics)
		{
			XmlElement el = statistics.GetXmlElement(doc, recursive);

			using (StringWriter sw = new StringWriter())
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.NewLineOnAttributes = false;
				settings.OmitXmlDeclaration = true;
				settings.Encoding = Encoding.UTF8;

				XmlWriter writer = XmlWriter.Create(sw, settings);
				el.WriteTo(writer);
				writer.Flush();

				Platform.Log(level, sw.ToString());

				writer.Close();
			}

			foreach (IStatisticsLoggerListener extension in _extensions)
			{
				extension.OnStatisticsLogged(statistics);
			}
		}

    	/// <summary>
        /// Logs a statistics.
        /// </summary>
        /// <param name="level">The log level used for logging the statistics</param>
        /// <param name="statistics">The statistics to be logged</param>
        public static void Log(LogLevel level, StatisticsSet statistics)
        {
    		Log(level, true, statistics);
        }
    }
}