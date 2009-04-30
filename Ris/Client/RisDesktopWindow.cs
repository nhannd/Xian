#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
                username = string.Format("{0} @ {1}", username, currentSession.WorkingFacility.Code);

            // show the user name before the base title
            return string.Format("{0} - {1}", username, base.MakeTitle(baseTitle, activeWorkspace));
        }
    }
}
