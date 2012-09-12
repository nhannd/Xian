#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using System.Web.UI;

namespace ClearCanvas.ImageServer.Web.Common.Extensions
{
    public sealed class ExtensibleAttribute : Attribute
    {
        public Type ExtensionPoint;
    }

    public interface ILoginPage
    {
        Control SplashScreenControl { get; }
    }

    public interface ILoginPageExtension
    {
        void OnInit(Page page);

        void OnPageLoad();
    }

    [ExtensionPoint]
    public class LoginPageExtensionPoint : ExtensionPoint<ILoginPageExtension>
    {

    }
}
