#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Reporting
{
	/// <summary>
	/// Extension point for views onto <see cref="ReportEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ReportEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// Default ReportEditorComponent class
	/// </summary>
	[AssociateView(typeof(ReportEditorComponentViewExtensionPoint))]
	public class ReportEditorComponent : ApplicationComponent, IReportEditor
	{
		#region EditingComponent

		/// <summary>
		/// A DHMTL component that allows editing of the report content.
		/// </summary>
		public class EditingComponent : DHtmlComponent
		{
			private readonly ReportEditorComponent _owner;

			public EditingComponent(ReportEditorComponent owner)
			{
				_owner = owner;
			}

			public void Refresh()
			{
				NotifyAllPropertiesChanged();
			}

			protected override string GetTag(string tag)
			{
				if (tag == ReportPartDetail.ReportContentKey)
					return _owner.ReportContent;

				return base.GetTag(tag);
			}

			protected override void SetTag(string tag, string data)
			{
				if (tag == ReportPartDetail.ReportContentKey)
					_owner.ReportContent = data;
				else
					base.SetTag(tag, data);
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _owner._reportingContext.WorklistItem;
			}
		}

		#endregion

		#region PreviewComponent

		/// <summary>
		/// A DHMTL component that shows a preview of the existing report parts.
		/// </summary>
		public class PreviewComponent : DHtmlComponent
		{
			private readonly ReportEditorComponent _owner;

			public PreviewComponent(ReportEditorComponent owner)
			{
				_owner = owner;
			}

			public void Refresh()
			{
				NotifyAllPropertiesChanged();
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _owner._reportingContext.Report;
			}
		}

		#endregion

		private readonly EditingComponent _editingComponent;
		private ChildComponentHost _editingHost;

		private readonly PreviewComponent _previewComponent;
		private ChildComponentHost _previewHost;

		private readonly IReportingContext _reportingContext;


		public ReportEditorComponent(IReportingContext context)
		{
			_reportingContext = context;
			_editingComponent = new EditingComponent(this);
			_previewComponent = new PreviewComponent(this);
		}

		public override void Start()
		{
			_editingComponent.SetUrl(this.EditorUrl);
			_editingHost = new ChildComponentHost(this.Host, _editingComponent);
			_editingHost.StartComponent();

			_previewComponent.SetUrl(this.PreviewUrl);
			_previewHost = new ChildComponentHost(this.Host, _previewComponent);
			_previewHost.StartComponent();

			base.Start();
		}

		#region IReportEditor Members

		IApplicationComponent IReportEditor.GetComponent()
		{
			return this;
		}

		bool IReportEditor.Save(ReportEditorCloseReason reason)
		{
			_editingComponent.SaveData();
			return true;
		}

		#endregion

		#region Presentation Model

		public virtual bool PreviewVisible
		{
			get { return IsAddendum; }
		}

		public ApplicationComponentHost ReportEditorHost
		{
			get { return _editingHost; }
		}

		public ApplicationComponentHost ReportPreviewHost
		{
			get { return _previewHost; }
		}

		public string ReportContent
		{
			get { return _reportingContext.ReportContent; }
			set { _reportingContext.ReportContent = value; }
		}

		#endregion

		private string EditorUrl
		{
			get
			{
				return IsAddendum
						? WebResourcesSettings.Default.AddendumEditorPageUrl
						: WebResourcesSettings.Default.ReportEditorPageUrl;
			}
		}

		private string PreviewUrl
		{
			get
			{
				return IsAddendum ? WebResourcesSettings.Default.ReportPreviewPageUrl : "about:blank";
			}
		}

		private bool IsAddendum
		{
			get { return _reportingContext.ActiveReportPartIndex > 0; }
		}

	}
}
