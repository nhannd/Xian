#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Globalization;
using System.ServiceModel;
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.UsageTracking
{
    /// <summary>
    /// Static helper class for implementing usage tracking of ClearCanvas applications.
    /// </summary>
    public static class UsageTracking
    {

        #region Private Members
        
        private static event EventHandler<ItemEventArgs<DisplayMessage>> _message;
        private static readonly object _syncLock = new object();
        
        #endregion

        #region Public Static Properties

        /// <summary>
        /// Event which can receive display messages from the UsageTracking server
        /// </summary>
        /// <remarks>
        /// Note that the configuration option in <see cref="UsageTrackingSettings"/> must be enabled to receive these
        /// messages.
        /// </remarks>
        public static event EventHandler<ItemEventArgs<DisplayMessage>> MessageEvent
		{
            add
            {
                lock (_syncLock)
                    _message += value;
            }
            remove
            {
                lock (_syncLock)
                    _message -= value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Send the UsageTracking message.
        /// </summary>
        /// <param name="theMessage"></param>
        private static void Send(object theMessage)
        {
            try
            {
                UsageMessage message = theMessage as UsageMessage;
                if (message != null)
                {
                    RegisterRequest req = new RegisterRequest
                                              {
                                                  Message = message
                                              };

                    WSHttpBinding binding = new WSHttpBinding();
                    EndpointAddress endpointAddress = new EndpointAddress("http://localhost/UsageTracking/Service.svc");

                    RegisterResponse response;
                    using (UsageTrackingServiceClient client = new UsageTrackingServiceClient(binding, endpointAddress))
                    {
                        response = client.Register(req);
                    }
                    if (response != null 
                        && response.Message != null
                        && UsageTrackingSettings.Default.DisplayMessages)
                    {
                        EventsHelper.Fire(_message, null, new ItemEventArgs<DisplayMessage>(response.Message)); 
                    }
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Debug, e);
            }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Register the usage of the application with a ClearCanvas server.
        /// </summary>
        /// <remarks>
        /// A check is done of the <see cref="UsageTrackingSettings"/>, and if usage tracking is enabled, the 
        /// <paramref name="message"/> is sent to the ClearCanvas server.
        /// </remarks>
        /// <param name="message">The usage message to send.</param>
        public static void Register(UsageMessage message)
        {
            //#if	!DEBUG

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

            //#endif
        }

        /// <summary>
        /// Get a <see cref="UsageMessage"/> for the application.
        /// </summary>
        /// <returns>
        /// <para>
        /// A new <see cref="UsageMessage"/> object with product, region, timestamp, license, and OS information filled in.
        /// </para>
        /// <para>
        /// The <see cref="UsageMessage"/> instance is used in conjunction with <see cref="Register"/> to send a usage message
        /// to ClearCanvas servers.
        /// </para>
        /// </returns>
        public static UsageMessage GetUsageMessage()
        {
            UsageMessage msg = new UsageMessage
                                   {
                                       Version = ProductInformation.GetVersion(true, true),
                                       Product = ProductInformation.Product,
                                       Component = ProductInformation.Name,
                                       Region = CultureInfo.CurrentCulture.Name,
                                       Timestamp = Platform.Time,
                                       OS = Environment.OSVersion.ToString(),
                                       //LicenseString = ProductInformation.LicenseString
                                   };
            return msg;
        }

        #endregion
    }
}
