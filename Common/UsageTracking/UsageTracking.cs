using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace ClearCanvas.Common.UsageTracking
{
    /// <summary>
    /// 
    /// </summary>
    public class UsageTracking
    {
        private static void Send(object theMessage)
        {
            try
            {
                UsageMessage message = theMessage as UsageMessage;
                if (message != null)
                {
                    message.Product = ProductInformation.Name;

                    XmlSerializer serializer = new XmlSerializer(typeof(UsageMessage));

                    using (MemoryStream ms = new MemoryStream())
                    {
                        serializer.Serialize(ms, message);

                        using (UsageTrackingServiceClient client = new UsageTrackingServiceClient())
                        {
                            client.Register(ms.ToString());
                        }
                    }       
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Debug, e);
            }
        }

        /// <summary>
        /// Register the usage of the application with a ClearCanvas server.
        /// </summary>
        /// <param name="message"></param>
        public static void Register(UsageMessage message)
        {
            if (UsageTrackingSettings.Default.Enabled)
                try
                {
                    UsageMessage theMessage = message;

                    ThreadPool.QueueUserWorkItem(Send,theMessage);
                }
                catch (Exception e)
                {
                    // Fail silently
                    Platform.Log(LogLevel.Debug, e);
                }
        }

        /// <summary>
        /// Get a <see cref="UsageMessage"/> for the application.
        /// </summary>
        /// <returns></returns>
        public static UsageMessage GetUsageMessage()
        {
            UsageMessage msg = new UsageMessage
                                   {
                                       Version = ProductInformation.GetVersion(true, true),
                                       Product = ProductInformation.Name,
                                       Region = CultureInfo.CurrentCulture.Name,
                                       Timestamp = Platform.Time,
                                       OS = Environment.OSVersion.ToString(),
                                       License = ProductInformation.License
                                   };
            return msg;
        }
    }
}
