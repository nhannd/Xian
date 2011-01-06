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
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Services.Common.Authentication
{
    class SessionTokenManager
    {
        private readonly Cache _cache = HttpRuntime.Cache;
        private static readonly SessionTokenManager _instance = new SessionTokenManager();
        private readonly object _sync = new object();
        static public SessionTokenManager Instance
        {
            get{ return _instance; }
        }

        private SessionTokenManager()
        {
        }

        public SessionToken FindSession(string username)
        {
            lock (_sync)
            {
                SessionToken session = _cache[username] as SessionToken;
                if (session != null)
                {
                    return session;
                }
                else
                    return null;
            }
        }

        public void AddSession(SessionToken session)
        {
            if (session == null)
                throw new Exception("Token cannot be null");
            if (session.ExpiryTime < Platform.Time)
            {
                throw new Exception(String.Format("Token {0} already expired. Cannot be updated.", session.Id));
            }

            lock (_sync)
            {
                _cache.Insert(session.Id, session, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration);
                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, "Session {0} is added", session.Id);
            }
        }

        public void RemoveSession(SessionToken session)
        {
            if (session == null)
                throw new Exception("Token cannot be null");

            lock (_sync)
            {
                _cache.Remove(session.Id);
                if (Platform.IsLogLevelEnabled(LogLevel.Debug))
                    Platform.Log(LogLevel.Debug, "Session {0} is removed", session.Id);
            }
        }

        public SessionToken UpdateSession(SessionToken token)
        {
            if (token == null)
                throw new Exception("Token cannot be null");

            if (token.ExpiryTime < Platform.Time)
            {
                throw new Exception(String.Format("Token {0} already expired. Cannot be updated.", token.Id));
            }

            lock (_sync)
            {
                RemoveSession(token);

                var newSession = new SessionToken(token.Id, Platform.Time + ServerPlatform.WebSessionTimeout);

                AddSession(newSession);

                return newSession;
            }
        }
    }
}