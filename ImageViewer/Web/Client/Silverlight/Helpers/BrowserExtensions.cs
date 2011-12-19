#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Browser;
using ClearCanvas.Web.Client.Silverlight.Utilities;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Helpers
{
    public static class BrowserWindow
    {
        public static void SetStatus(string msg){
            HtmlPage.Window.SetProperty("status", msg);
        }


        public static void Close()
        {
            UIThread.Execute(delegate()
            {
                HtmlPage.Window.Eval("window.open('','_self');window.close();");
            });

        }

        public static void Reload()
        {
            HtmlPage.Window.Navigate(HtmlPage.Document.DocumentUri);
        }
    }
}
