#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
	public abstract class ServerTreeTool : Tool<IServerTreeToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChangedEvent;

		public ServerTreeTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Context.SelectedServerChanged += new EventHandler(OnSelectedServerChanged);
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		protected abstract void OnSelectedServerChanged(object sender, EventArgs e);

		public event EventHandler EnabledChanged
		{
			add { _enabledChangedEvent += value; }
			remove { _enabledChangedEvent -= value; }
		}
	}
}