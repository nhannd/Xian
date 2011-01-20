#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Enterprise.Desktop.Login
{
    public enum LoginDialogMode
    {
        InitialLogin,
        RenewLogin
    }

    public interface ILoginDialog : IDisposable
    {
        bool Show();

        LoginDialogMode Mode { get; set; }
        //string[] DomainChoices { get; set; }
        //string Domain { get; set; }

        string UserName { get; set; }
        string Password { get; }
    }
}