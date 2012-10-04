#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers;
using System.ServiceModel.Channels;
using System.Windows.Browser;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    public enum ApplicationServiceMode
    {
        BasicHttp        
    }

    public class ApplicationStartupParameters
    {
        static private ApplicationStartupParameters _current = new ApplicationStartupParameters(Application.Current.Host.InitParams);

        public string TimeoutUrl { get; set; }
        public string Username { get; private set; }
        public string SessionToken { get; private set; }
		public bool IsSessionShared { get; private set; }
        public bool LogPerformance { get; private set; }
        public string LocalIPAddress { get; private set; }
        public ServerConfiguration ServerSettings { get; set; }
        public ApplicationServiceMode Mode { get; set; }

        private ApplicationStartupParameters(System.Collections.Generic.IDictionary<string, string> initParams)
        {
            if (initParams.ContainsKey(Constants.SilverlightInitParameters.TimeoutUrl))
                TimeoutUrl = initParams[Constants.SilverlightInitParameters.TimeoutUrl];

            if (initParams.ContainsKey(Constants.SilverlightInitParameters.Username))
                Username = initParams[Constants.SilverlightInitParameters.Username];

            if (initParams.ContainsKey(Constants.SilverlightInitParameters.Session))
                SessionToken = initParams[Constants.SilverlightInitParameters.Session];

			if (initParams.ContainsKey(Constants.SilverlightInitParameters.IsSessionShared))
				IsSessionShared = initParams[Constants.SilverlightInitParameters.IsSessionShared] == "true";
            if (initParams.ContainsKey(Constants.SilverlightInitParameters.LocalIPAddress))
                LocalIPAddress = initParams[Constants.SilverlightInitParameters.LocalIPAddress];
            else
            {
                LocalIPAddress = "UNKNOWN";
            }

            bool logPerformance;
            if (initParams.ContainsKey(Constants.SilverlightInitParameters.LogPerformance) && 
                bool.TryParse(initParams[Constants.SilverlightInitParameters.LogPerformance], out logPerformance))
            {
                LogPerformance = logPerformance;
            }
            else
            {
                LogPerformance = false;
            }

            TimeSpan inactivityTimeout = TimeSpan.FromMinutes(15);
            if (initParams.ContainsKey(Constants.SilverlightInitParameters.InactivityTimeout))
                inactivityTimeout = TimeSpan.Parse(initParams[Constants.SilverlightInitParameters.InactivityTimeout]);

            int serverPort = 4520;
            if (initParams.ContainsKey(Constants.SilverlightInitParameters.Port))
                serverPort = int.Parse(initParams[Constants.SilverlightInitParameters.Port]);

            Mode = ApplicationServiceMode.BasicHttp; 
            if (initParams.ContainsKey(Constants.SilverlightInitParameters.Mode))
            {
                ApplicationServiceMode mode;
                if (Enum.TryParse<ApplicationServiceMode>(initParams[Constants.SilverlightInitParameters.Mode], out mode))
                    Mode = mode;
            }


            ServerSettings = new ServerConfiguration
            {
                InactivityTimeout = inactivityTimeout,
                LANMode = true, // for SL4, always true
                Port = serverPort
            };
        }

        static public ApplicationStartupParameters Current
        {
            get
            {
                return _current;
            }
        }

    }
}
