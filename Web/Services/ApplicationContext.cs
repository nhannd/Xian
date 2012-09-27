#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Web.Common;
using System.Security.Principal;

namespace ClearCanvas.Web.Services
{
	public interface IApplicationContext
	{
		EntityHandlerStore EntityHandlers { get; }
		IPrincipal Principal { get; }
        Guid ApplicationId { get; }
		void FireEvent(Event e);
		void FatalError(Exception e);

        EventSet GetPendingOutboundEvent(int wait);

	    event EventHandler EventFired;

	    bool HasProperty(string key);
        bool TryGetValue<T>(string key, out T value);
	    void SetProperty<T>(string key, T value);
    }

	public class ApplicationContext : IApplicationContext, IDisposable
    {
		private readonly Application _application;
        private EventQueue _eventQueue;
	    private readonly Dictionary<string, object> _properties;

        internal ApplicationContext(Application application)
        {
            _properties = new Dictionary<string, object>();
			_application = application;
			_eventQueue = new EventQueue(application);
			EntityHandlers = new EntityHandlerStore();
        }

		public static IApplicationContext Current
		{
			get
			{
				IApplication application = Application.Current;
				return application != null ? application.Context : null;
			}
		}

		public IPrincipal Principal
		{
			get { return _application.Principal; }
		}

	    public Guid ApplicationId
	    {
            get { return _application.Identifier; }
	    }

	    // TODO (CR Sep 2012): Yuck.
	    public event EventHandler EventFired;

		#region IApplicationContext Members

		public EntityHandlerStore EntityHandlers { get; private set; }

		public void FatalError(Exception e)		
        {
			_application.Stop(e);
        }

        public EventSet GetPendingOutboundEvent(int wait)
	    {
            
            if (_eventQueue == null)
                return null;
	        return _eventQueue.GetPendingEvent(wait);
	    }

        public bool TryGetValue<T>(string key, out T value)
	    {
            object temp;
            if (_properties.TryGetValue(key, out temp))
            {
                value = (T) temp;
                return true;
            }
            value = default(T);
            return false;
	    }

        public bool HasProperty(string key)
        {
            return _properties.ContainsKey(key);
        }

	    public void SetProperty<T>(string key, T value)
	    {
            _properties[key] = value;
	    }

	    public void FireEvent(Event @event)
        {
			InjectSenderName(@event);
			if (_eventQueue != null)
			    _eventQueue.Send(@event);

	        EventsHelper.Fire(EventFired, this, EventArgs.Empty);
        }

	    #endregion
        
        private void InjectSenderName(Event @event)
        {
            IEntityHandler handler = EntityHandlers[@event.SenderId];
        	if (handler == null || !String.IsNullOrEmpty(@event.Sender))
				return;
        	
			@event.Sender = !String.IsNullOrEmpty(handler.Name) ? handler.Name : handler.GetType().Name;
        }

		public void Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Debug, e);
            }
        }

        /// <summary>
        /// Implementation of the <see cref="IDisposable"/> pattern
        /// </summary>
        /// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _eventQueue == null) 
				return;

        	_eventQueue.Dispose();
        	_eventQueue = null;
        }
	}
}