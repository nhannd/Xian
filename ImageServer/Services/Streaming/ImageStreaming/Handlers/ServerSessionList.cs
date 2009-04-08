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