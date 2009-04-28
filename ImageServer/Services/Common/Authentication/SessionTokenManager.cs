using System;
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
                session = new SessionToken(session.Id, Platform.Time.AddMinutes(5));
                AddSession(session);
                return session;
            }
            else
                return null;
        }

        public void AddSession(SessionToken session)
        {
            _cache.Insert(session.Id, session, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
        }

        public void RemoveSession(SessionToken session)
        {
            _cache.Remove(session.Id);
        }

    }
}