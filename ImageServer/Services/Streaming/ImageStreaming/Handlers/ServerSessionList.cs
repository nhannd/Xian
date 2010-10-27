#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    class Session : Dictionary<object, object>
    {
        public new object this[object key]
        {
            get
            {
                if (this.ContainsKey(key))
                    return base[key];
                else
                    return null;
            }
        }
    }

    class ServerSessionList
    {
        private readonly Dictionary<string, Session> _sessions = new Dictionary<string, Session>();
        private readonly object _sync = new object();
        public Session this[string sessionId]
        {
            get
            {
                Session session;
                lock (_sync)
                {
                    if (!_sessions.TryGetValue(sessionId, out session))
                    {
                    
                        session = new Session();
                        _sessions.Add(sessionId, session);
                    }
                    
                }

                return session;
            }
        }
    }
}