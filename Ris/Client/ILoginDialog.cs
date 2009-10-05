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
using System.Drawing;

namespace ClearCanvas.Ris.Client
{
    public enum LoginDialogMode
    {
        InitialLogin,
        RenewLogin
    }

	/// <summary>
	/// Defines an interface to a login dialog that interacts with a user to obtain login credentials.
	/// </summary>
    public interface ILoginDialog : IDisposable
    {
		/// <summary>
		/// Shows the dialog, returning true if the user pressed OK, or false if cancelled.
		/// </summary>
		/// <returns></returns>
        bool Show();

		/// <summary>
		/// Gets or set the location of the dialog on the screen.
		/// </summary>
		Point Location { get; set; }

		/// <summary>
		/// Gets or sets the dialogs mode.
		/// </summary>
        LoginDialogMode Mode { get; set; }

		/// <summary>
		/// Gets or sets the list of facility choices.
		/// </summary>
        string[] FacilityChoices { get; set; }

		/// <summary>
		/// Gets or sets the facility.
		/// </summary>
        string Facility { get; set; }

		/// <summary>
		/// Gets or sets the user name.
		/// </summary>
        string UserName { get; set; }

		/// <summary>
		/// Gets the password entered by the user.
		/// </summary>
        string Password { get; }
    }
}
