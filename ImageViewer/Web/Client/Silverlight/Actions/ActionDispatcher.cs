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
using ClearCanvas.ImageViewer.Web.Client.Silverlight.AppServiceReference;

namespace ClearCanvas.ImageViewer.Web.Client.Silverlight.Actions
{
	//TODO (CR May 2010): do we need this class?
	public class ActionDispatcher : IDisposable
	{
		private readonly Dictionary<Guid,IActionUpdate> _map = new Dictionary<Guid, IActionUpdate>();
        private ServerEventMediator _eventDispatcher;

        public ServerEventMediator EventDispatcher
        {
            get { return _eventDispatcher; }
        }

		public ActionDispatcher(ServerEventMediator dispatcher)
		{
		    _eventDispatcher = dispatcher;

		}
		public void Register(Guid identifier, IActionUpdate update)
		{
			if (!_map.ContainsKey(identifier))
			{
				_map.Add(identifier, update);
                _eventDispatcher.RegisterEventHandler(identifier, ActionUpdate);
			}
		}

		public void Remove(Guid identifier)
		{
            if (_map.ContainsKey(identifier))
            {
                _map.Remove(identifier);
				_eventDispatcher.UnregisterEventHandler(identifier);
            }
		}

		private void ActionUpdate(object sender, ServerEventArgs e)
		{
			PropertyChangedEvent ev = e.ServerEvent as PropertyChangedEvent;
			if (ev != null)
			{
				IActionUpdate updateObject;

				if (_map.TryGetValue(ev.SenderId, out updateObject))
				{
					updateObject.Update(ev);
				}
			}
		}

	    public void Dispose()
	    {
            if (_eventDispatcher != null)
            {
                foreach (Guid g in _map.Keys)
                {
                    _eventDispatcher.UnregisterEventHandler(g);
                }

                _eventDispatcher = null;
                _map.Clear();
            }
	    }
	}
}
