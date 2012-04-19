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