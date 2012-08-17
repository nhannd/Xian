#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension to <see cref="DesktopWindowFactoryExtensionPoint"/> to provide instances of <see cref="RisDesktopWindow"/> subclass.
    /// </summary>
    [ExtensionOf(typeof(DesktopWindowFactoryExtensionPoint))]
    public class RisDesktopWindowFactory : IDesktopWindowFactory
    {
        /// <summary>
        /// Creates a new desktop window for the specified arguments.
        /// </summary>
        /// <param name="args">Arguments that control the creation of the desktop window.</param>
        /// <param name="application">The application with which the window is associated.</param>
        /// <returns>A new desktop window instance.</returns>
        public DesktopWindow CreateWindow(DesktopWindowCreationArgs args, Desktop.Application application)
        {
            return new RisDesktopWindow(args, application);
        }
    }

    /// <summary>
    /// RIS custom subclass of <see cref="DesktopWindow"/>.
    /// </summary>
    /// <remarks>
    /// So far the only reason this class exists is to display the RIS user-name in the title bar.
    /// </remarks>
    public class RisDesktopWindow : DesktopWindow
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="args"></param>
        /// <param name="application"></param>
        protected internal RisDesktopWindow(DesktopWindowCreationArgs args, Desktop.Application application)
            :base(args, application)
        {
			// set the current session before attempting to access other services, as these will require authentication
			LoginSession.Create(SessionManager.FacilityCode);
		}

        /// <summary>
        /// Creates the title that is displayed in the title bar.  Overridden to display user name. 
        /// </summary>
        /// <param name="baseTitle"></param>
        /// <param name="activeWorkspace"></param>
        /// <returns></returns>
        protected override string MakeTitle(string baseTitle, Workspace activeWorkspace)
        {
            LoginSession currentSession = LoginSession.Current;

            // if there is a full person name associated with the session, use it
            // otherwise use the UserName (login id)
            string username = (currentSession.FullName == null) ? currentSession.UserName :
                PersonNameFormat.Format(currentSession.FullName);

            // show working facility if specified
            if (currentSession.WorkingFacility != null)
                username = string.Format("{0} @ {1}", username, currentSession.WorkingFacility.Name);

            // show the user name before the base title
            return string.Format("{0} - {1}", username, base.MakeTitle(baseTitle, activeWorkspace));
        }
    }
}
