#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;
using System.Web;
using System.Web.Caching;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Services.Common.Authentication
{
    class SessionTokenManager
    {
        private readonly Cache _cache = HttpRuntime.Cache;
        private static readonly SessionTokenManager _instance = new SessionTokenManager();
        
        static public SessionTokenManager Instance
        {
            get{ return _instance; }
        }

        private SessionTokenManager()
        {
        }

        public SessionToken FindSession(string username)
        {
            SessionToken session = _cache[username] as SessionToken;
            if (session != null && session.ExpiryTime > Platform.Time)
            {
                session = new SessionToken(session.Id, Platform.Time.AddMinutes(Int32.Parse(ConfigurationManager.AppSettings["SessionTimeout"])));
                AddSession(session);
                return session;
            }
            else
                return null;
        }

        public void AddSession(SessionToken session)
        {
            _cache.Insert(session.Id, session, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(Int32.Parse(ConfigurationManager.AppSettings["SessionTimeout"])));
        }

        public void RemoveSession(SessionToken session)
        {
            _cache.Remove(session.Id);
        }

    }
}