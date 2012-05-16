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
using System.Linq;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	public abstract class StudyBrowserTool : Tool<IStudyBrowserToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChangedEvent;

		public override void Initialize()
		{
			base.Initialize();
			Context.SelectedStudyChanged += new EventHandler(OnSelectedStudyChanged);
			Context.SelectedServerChanged += new EventHandler(OnSelectedServerChanged);
		}

		protected virtual void OnSelectedStudyChanged(object sender, EventArgs e)
		{
			if (Context.SelectedStudy != null)
				Enabled = true;
			else
				Enabled = false;
		}

		protected abstract void OnSelectedServerChanged(object sender, EventArgs e);

		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChangedEvent += value; }
			remove { _enabledChangedEvent -= value; }
		}

		protected int ProcessItemsAsync<T>(IEnumerable<T> items, Action<T> processAction, bool cancelable)
		{
			var itemsToProcess = items.ToList();
			return ProgressDialog.Show(this.Context.DesktopWindow,
				itemsToProcess,
				(item, i) =>
				{
					processAction(item);
					return string.Format(SR.MessageProcessedItemsProgress, i + 1, itemsToProcess.Count);
				},
				cancelable);
		}
	}
}
