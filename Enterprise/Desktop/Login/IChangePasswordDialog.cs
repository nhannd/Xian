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
    public interface IChangePasswordDialog : IDisposable
    {
        bool Show();

        string UserName { get; set; }
        string Password { get; set; }
        string NewPassword { get; }
    }
}