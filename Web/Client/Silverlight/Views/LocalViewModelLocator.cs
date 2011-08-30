#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Web.Client.Silverlight.ViewModel;

namespace ClearCanvas.Web.Client.Silverlight.Views
{
    public class LocalViewModelLocator
    {
        private static readonly object SyncLock = new Object();

        private static LogPanelViewModel _search;
      

        public LogPanelViewModel LogPanel
        {
            get
            {
                lock (SyncLock)
                {
                    return _search ?? (_search = new LogPanelViewModel());
                }
            }
        }
    }
}
