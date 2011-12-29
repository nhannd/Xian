#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Web.Common
{
    public interface IThemeManager
    {
        
    }

    [ExtensionPoint]
    public class ThemeManagerExtensionPoint : ExtensionPoint<IThemeManager>
    {
    }

    public static class ThemeManager
    {
        public static string CurrentTheme
        {
            get;
            set;
        }

        public static void ApplyTheme( System.Web.UI.Page p)
        {
            if (CurrentTheme == null) CurrentTheme = "Vet";

            p.Theme = CurrentTheme;
        }

        public static string GetDefaultTheme()
        {
            return "Default";
        }
    }
}
