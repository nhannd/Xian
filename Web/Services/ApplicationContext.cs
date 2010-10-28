#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
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
	}

	public class ApplicationContext : IApplicationContext, IDisposable
    {
		private readonly Application _application;
		internal EventBroker _eventBroker;
		
        internal ApplicationContext(Application application, IApplicationServiceCallback callback)
        {
			_application = application;
			_eventBroker = new EventBroker(application, callback);
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

		internal bool SuspendEvents
		{
			get { return _eventBroker.Suspended; }
			set { _eventBroker.Suspended = value; }
		}

		public IPrincipal Principal
		{
			get { return _application.Principal; }
		}

	    public Guid ApplicationId
	    {
            get { return _application.Identifier; }
	    }
	
		#region IApplicationContext Members

		public EntityHandlerStore EntityHandlers { get; private set; }

		public void FatalError(Exception e)		
        {
			_application.Stop(e);
        }
		
		public void FireEvent(Event @event)
        {
			InjectSenderName(@event);
			if (_eventBroker != null)
			    _eventBroker.Send(@event);
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
            if (!disposing || _eventBroker == null) 
				return;

        	_eventBroker.Dispose();
        	_eventBroker = null;
        }
	}
}