#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
