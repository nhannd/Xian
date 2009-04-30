#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Runtime.Serialization;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	public interface IReportEditorComponent : IApplicationComponent
	{
		ICannedTextLookupHandler CannedTextLookupHandler { get; }
		bool PreviewVisible { get; }
		ApplicationComponentHost ReportPreviewHost { get; }
		string EditorText { get; set; }
	}

	public abstract class ReportEditorComponentBase<TReportEditorContext, TCloseReason> : ApplicationComponent, IReportEditorComponent
		where TReportEditorContext : IReportEditorContextBase<TCloseReason>
	{

		#region PreviewComponent

		/// <summary>
		/// Provides the preview of existing report parts.
		/// </summary>
		public class PreviewComponent : DHtmlComponent
		{
			private readonly ReportEditorComponentBase<TReportEditorContext, TCloseReason> _owner;

			public PreviewComponent(ReportEditorComponentBase<TReportEditorContext, TCloseReason> owner)
			{
				_owner = owner;
			}

			public void Refresh()
			{
				NotifyAllPropertiesChanged();
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _owner._context.Report;
			}
		}

		#endregion

		private readonly PreviewComponent _previewComponent;
		private ChildComponentHost _previewHost;

		private ReportContent _reportContent;

		private ICannedTextLookupHandler _cannedTextLookupHandler;

		private readonly TReportEditorContext _context;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ReportEditorComponentBase(TReportEditorContext context)
		{
			_context = context;
			_previewComponent = new PreviewComponent(this);
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			_cannedTextLookupHandler = new CannedTextLookupHandler(this.Host.DesktopWindow);

			LoadOrCreateReportContent();

			_previewComponent.SetUrl(this.PreviewUrl);
			_previewHost = new ChildComponentHost(this.Host, _previewComponent);
			_previewHost.StartComponent();

			_context.WorklistItemChanged += WorklistItemChangedEventHandler;

			base.Start();
		}

		/// <summary>
		/// Called by the host when the application component is being terminated.
		/// </summary>
		public override void Stop()
		{
			if (_previewHost != null)
			{
				_previewHost.StopComponent();
				_previewHost = null;
			}

			base.Stop();
		}

		private void WorklistItemChangedEventHandler(object sender, EventArgs e)
		{
			if (_context.WorklistItem != null)
			{
				LoadOrCreateReportContent();
				_previewComponent.Refresh();
				NotifyPropertyChanged("EditorText");
			}
			else
			{
				//TODO: should clear everything
			}
		}

		protected virtual void LoadOrCreateReportContent()
		{
			if (!string.IsNullOrEmpty(_context.ReportContent))
			{
				// editing an existing report - just deserialize the content
				_reportContent = JsmlSerializer.Deserialize<ReportContent>(_context.ReportContent);
			}
			else
			{
				// create a new ReportContent object
				_reportContent = new ReportContent(null);

				// HACK: update the active ReportPart object with the structured report
				// (this is solely for the benefit of the Preview component, it does not have any affect on what is ultimately saved)
				ReportPartDetail activePart = _context.Report.GetPart(_context.ActiveReportPartIndex);
				activePart.Content = _reportContent.ToJsml();
			}
		}

		#region ITranscriptionEditor Members

		public IApplicationComponent GetComponent()
		{
			return this;
		}

		public virtual bool Save(TCloseReason reason)
		{
			_context.ReportContent = _reportContent.ToJsml();
			return true;
		}

		#endregion

		#region Presentation Model

		public ICannedTextLookupHandler CannedTextLookupHandler
		{
			get { return _cannedTextLookupHandler; }
		}

		public virtual bool PreviewVisible
		{
			get { return _context.IsAddendum; }
		}

		public ApplicationComponentHost ReportPreviewHost
		{
			get { return _previewHost; }
		}

		[ValidateNotNull]
		public string EditorText
		{
			get { return _reportContent.ReportText; }
			set
			{
				if (!Equals(value, _reportContent.ReportText))
				{
					_reportContent.ReportText = value;
					this.Modified = true;
				}
			}
		}

		#endregion

		protected abstract string PreviewUrl { get; }

		protected TReportEditorContext Context
		{
			get { return _context; }
		}

		private bool IsAddendum
		{
			get { return _context.ActiveReportPartIndex > 0; }
		}
	}
}