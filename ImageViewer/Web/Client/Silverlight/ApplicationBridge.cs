#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Windows.Browser;
using ClearCanvas.ImageViewer.Web.Client.Silverlight.AppServiceReference;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    /// <summary>
    /// Facilitates the communication between silverlight application and the host
    /// </summary>
    [ScriptableType]
    public class ApplicationBridge
    {        
        [ScriptableMember]
        public event EventHandler<SessionUpdatedEventArgs> ViewerSessionUpdated;

        [ThreadStatic]
        private static ApplicationBridge _current;

        [ThreadStatic]
        private static object _syncLock = new object();

        private ApplicationBridge()
        {
            HtmlPage.RegisterScriptableObject("ApplicationBridge", this);
        }

        /// <summary>
        /// Notifies the host when the session in the silverlight is updated.
        /// </summary>
        public void OnViewerSessionUpdated(object sender, SessionUpdatedEvent @event)
        {
            if (ViewerSessionUpdated != null)
            {
                ViewerSessionUpdated(sender, new SessionUpdatedEventArgs() { ExpiryTimeUtc = @event.ExpiryTimeUtc });
            }
        }


        [ScriptableMember]
        public void HostSessionUpdated()
        {
            
        }

        public static void Initialize()
        {
            var current = Current; 
        }

        static public ApplicationBridge Current
        {
            get
            {
                if (_current == null)
                {
                    lock (_syncLock)
                    {
                        if (_current == null)
                        {
                            _current = new ApplicationBridge();
                        }
                    }
                }

                return _current;
            }
        }

    }
}
