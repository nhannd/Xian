#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Browser;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight
{
    [ScriptableType] // facility communication with the host
    public class SessionUpdatedEventArgs:EventArgs
    {
        [ScriptableMember]
        public string Username { get; set;  }

        [ScriptableMember]
        public string SessionId { get; set;  }

        [ScriptableMember]
        public DateTime ExpiryTimeUtc { get; set; }

    }
}
