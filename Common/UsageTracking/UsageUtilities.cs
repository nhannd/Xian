#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Globalization;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.UsageTracking
{
    /// <summary>
    /// Enum for specifying if a Usage Tracking message will be sent on the current thread or a background thread.
    /// </summary>
    public enum UsageTrackingThread
    {
        /// <summary>
        /// The Usage Tracking message will be sent on a background thread
        /// </summary>
        Background,
        /// <summary>
        /// The UsageTracking message will be sent on the current thread.
        /// </summary>
        Current,
    }

    /// <summary>
    /// Static helper class for implementing usage tracking of ClearCanvas applications.
    /// </summary>
    public static class UsageUtilities
    {
        #region Private Members
        
        private static event EventHandler<ItemEventArgs<DisplayMessage>> Message;
        private static readonly object _syncLock = new object();
        private static bool _first = true;
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
                    Message += value;
            }
            remove
            {
                lock (_syncLock)
                    Message -= value;
            }
        }

        #endregion

        #region Private Methods

        private static bool TrySend(RegisterRequest message, Binding binding, EndpointAddress endpointAddress)
        {
            try
            {
                Platform.Log(LogLevel.Info, "Attempting {0}", endpointAddress.Uri.ToString());

                RegisterResponse response;
                using (UsageTrackingServiceClient client = new UsageTrackingServiceClient(binding, endpointAddress))
                {
                    response = client.Register(message);
                }
                if (response != null
                    && response.Message != null
                    && UsageTrackingSettings.Default.DisplayMessages)
                {
                    EventsHelper.Fire(Message, null, new ItemEventArgs<DisplayMessage>(response.Message));
                }

                return true;
            }
            catch (Exception ex)
            {
                Platform.Log(LogLevel.Error, ex);
                return false;
            }
        }


        /// <summary>
        /// Send the UsageTracking message.
        /// </summary>
        /// <param name="theMessage"></param>
        private static void Send(object theMessage)
        {
            try
            {
                lock (_syncLock)
                {
                    if (_first)
                    {
                        // Note, this is required when in debug mode and communicating with 4rf,
                        // which doesn't have an official cert, it isn't required for communicating with
                        // the production server.
                        ServicePointManager.ServerCertificateValidationCallback +=
                            ((sender, certificate, chain, sslPolicyErrors) =>
                             true);
                        _first = false;
                    }
                }

                UsageMessage message = theMessage as UsageMessage;
                if (message != null)
                {
                    RegisterRequest req = new RegisterRequest
                                              {
                                                  Message = message
                                              };

                  

#if UNIT_TESTS_USAGE_TRACKING
                    WSHttpBinding binding = new WSHttpBinding();
                    EndpointAddress endpointAddress = new EndpointAddress("http://localhost:8080/UsageTracking");
                    TrySend(req, binding, endpointAddress);
#elif	DEBUG
                    // WSHttpBinding binding = new WSHttpBinding(SecurityMode.None);
                   // EndpointAddress endpointAddress = new EndpointAddress("http://localhost/Tracking/Service.svc");
                    WSHttpBinding binding = new WSHttpBinding(SecurityMode.Transport);
                    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                    binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                    EndpointAddress endpointAddress = new EndpointAddress("https://4rf/Tracking/Service.svc");
                    TrySend(req, binding, endpointAddress);
#else
                    // This is updated to the real address as part of the build process, when appropriate and
                    // doing an official build.
                    WSHttpBinding binding = new WSHttpBinding(SecurityMode.Transport);
                    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                    binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                    EndpointAddress endpointAddress = new EndpointAddress("https://4rf/Tracking/Service.svc");
                    if (!TrySend(req, binding, endpointAddress))
                    {
                        endpointAddress = new EndpointAddress("https://10.19.20.122/Tracking/Service.svc");
                        TrySend(req, binding, endpointAddress); 
                    }
#endif
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
        /// Register the usage of the application with a ClearCanvas server on a background thread.
        /// </summary>
        /// <remarks>
        /// A check is done of the <see cref="UsageTrackingSettings"/>, and if usage tracking is enabled, the 
        /// <paramref name="message"/> is sent to the ClearCanvas server.
        /// </remarks>
        /// <param name="message">The usage message to send.</param>
        /// <param name="thread">Flag telling if the usage will be sent on the current thread or a background thread.</param>
        public static void Register(UsageMessage message, UsageTrackingThread thread)
        {
            if (UsageTrackingSettings.Default.Enabled)
                try
                {
                    UsageMessage theMessage = message;
                    if (thread == UsageTrackingThread.Current)
                        Send(theMessage);
                    else if (thread == UsageTrackingThread.Background)
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
            UsageMessage msg;

            // if license key cannot be retrieved, send an empty string to maintain the existing data on the server
            string licenseString = string.Empty; 
            try
            {
                licenseString = LicenseInformation.LicenseKey;
            }
            catch(Exception ex)
            {
                Platform.Log(LogLevel.Debug, ex, "An error has occurred when trying to get the license string");
            }
            finally
            {
                msg = new UsageMessage
                {
                    Version = ProductInformation.GetVersion(true, true),
                    Product = ProductInformation.Product,
                    Component = ProductInformation.Component,
                    Edition = ProductInformation.Edition,
                    Release = ProductInformation.Release,
                    //TODO (CR February 2011) - High: We should have left this as a property on ProductInformation rather than checking for empty string.
                    AllowDiagnosticUse = ProductInformation.Release == string.Empty,
                    Region = CultureInfo.CurrentCulture.Name,
                    Timestamp = Platform.Time,
                    OS = Environment.OSVersion.ToString(),
                    MachineIdentifier = EnvironmentUtilities.MachineIdentifier,
                    MessageType = UsageType.Other,
                    LicenseString = licenseString,
                };

            }

            return msg;
        }

        #endregion


    }
}
