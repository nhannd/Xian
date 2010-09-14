#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails
{
	[ExtensionPoint]
	public sealed class SeriesDetailsToolExtensionPoint : ExtensionPoint<ITool> {}

	public interface ISeriesDetailsToolContext : IToolContext
	{
		IPatientData Patient { get; }
		IStudyData Study { get; }
		IList<ISeriesData> AllSeries { get; }
		IList<ISeriesData> SelectedSeries { get; }
		event EventHandler SelectedSeriesChanged;
		void RefreshSeriesTable();

		//TODO (CR Sept 2010): don't expose the component.
		SeriesDetailsComponent Component { get; }
		IDesktopWindow DesktopWindow { get; }
	}

	public abstract class SeriesDetailsTool : Tool<ISeriesDetailsToolContext>
	{
		protected internal const string ToolbarActionSite = "dicomseriesdetails-toolbar";
		protected internal const string ContextMenuActionSite = "dicomseriesdetails-contextmenu";

		private event EventHandler _enabledChanged;
		private bool _enabled;

		protected SeriesDetailsTool() {}

		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					OnEnabledChanged();
				}
			}
		}

		protected SeriesDetailsComponent SeriesDetailsComponent
		{
			get { return Context.Component; }
		}

		protected IPatientData Patient
		{
			get { return Context.Patient; }
		}

		protected IStudyData Study
		{
			get { return Context.Study; }
		}

		protected object Server
		{
			get { return Context.Component.StudyItem.Server; }
		}

		protected IList<ISeriesData> AllSeries
		{
			get { return Context.AllSeries; }
		}

		protected IList<ISeriesData> SelectedSeries
		{
			get { return Context.SelectedSeries; }
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public override void Initialize()
		{
			base.Initialize();
			this.Context.SelectedSeriesChanged += OnComponentSelectedSeriesChanged;
		}

		protected override void Dispose(bool disposing)
		{
			this.Context.SelectedSeriesChanged -= OnComponentSelectedSeriesChanged;
			base.Dispose(disposing);
		}

		private void OnComponentSelectedSeriesChanged(object sender, EventArgs e)
		{
			OnSelectedSeriesChanged();
		}

		protected virtual void OnEnabledChanged()
		{
			EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
		}

		protected virtual void OnSelectedSeriesChanged()
		{
			this.Enabled = this.Context.SelectedSeries.Count > 0;
		}

		protected bool IsStudyLoaderSupported
		{
			get { return ImageViewerComponent.IsStudyLoaderSupported(Context.Component.StudyItem.StudyLoaderName); }
		}
	}
}