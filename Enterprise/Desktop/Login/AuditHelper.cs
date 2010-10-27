#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Enterprise.Desktop.Login
{
    enum EventResult
    {
        Success,
        SeriousFailure
    }

    class AuditHelper
    {
        public static bool Enabled;

        public static void LogLogin(string username, EventResult result)
        {
            // TODO: Plugin?
        }

        public static void LogLogout(string username, EventResult result)
        {
            // TODO: Plugin?s
            
        }
    }
}
