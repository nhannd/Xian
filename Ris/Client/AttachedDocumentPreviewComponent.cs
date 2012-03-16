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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="AttachedDocumentPreviewComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class AttachedDocumentPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[ExtensionPoint]
	public class AttachedDocumentToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IAttachedDocumentToolContext : IToolContext
	{
		event EventHandler SelectedDocumentChanged;
		EntityRef SelectedDocumentRef { get; }

		void RemoveSelectedDocument();
		event EventHandler ChangeCommitted;
		bool IsReadonly { get; }

		IDesktopWindow DesktopWindow { get; }
	}

	/// <summary>
	/// AttachedDocumentPreviewComponent class
	/// </summary>
	[AssociateView(typeof(AttachedDocumentPreviewComponentViewExtensionPoint))]
	public class AttachedDocumentPreviewComponent : ApplicationComponent
	{
		public enum AttachmentMode
		{
			Patient,
			Order
		}

		class AttachedDocumentToolContext : ToolContext, IAttachedDocumentToolContext
		{
			private readonly AttachedDocumentPreviewComponent _component;

			internal AttachedDocumentToolContext(AttachedDocumentPreviewComponent component)
			{
				_component = component;
			}

			#region IAttachedDocumentToolContext Members

			public event EventHandler SelectedDocumentChanged
			{
				add { _component.SelectedDocumentChanged += value; }
				remove { _component.SelectedDocumentChanged -= value; }
			}

			public EntityRef SelectedDocumentRef
			{
				get { return _component.SelectedDocument == null ? null : _component.SelectedDocument.DocumentRef; }
			}

			public void RemoveSelectedDocument()
			{
				_component.RemoveSelectedDocument();
			}

			public event EventHandler ChangeCommitted
			{
				add { _component.ChangeCommited += value; }
				remove { _component.ChangeCommited -= value; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public bool IsReadonly
			{
				get { return _component.Readonly; }
			}

			#endregion
		}

		class AttachedDocumentDHtmlPreviewComponent : DHtmlComponent
		{
			private readonly AttachedDocumentPreviewComponent _component;

			public AttachedDocumentDHtmlPreviewComponent(AttachedDocumentPreviewComponent component)
			{
				_component = component;
				this.SetUrl(this.PreviewUrl);
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _component.SelectedDocument;
			}

			public string PreviewUrl
			{
				get { return WebResourcesSettings.Default.AttachedDocumentPreviewUrl; }
			}

			public void Refresh()
			{
				this.SetUrl(this.PreviewUrl);
			}
		}

		// Summary component members
		private AttachmentMode _mode;
		private AttachmentSummary _selectedAttachment;
		private AttachmentSummary _initialSelection;
		private event EventHandler _changeCommitted;
		private event EventHandler _selectedDocumentChanged;

		private readonly PatientAttachmentTable _patientAttachmentTable;
		private readonly OrderAttachmentTable _orderAttachmentTable;

		private AttachedDocumentDHtmlPreviewComponent _previewComponent;
		private ChildComponentHost _previewComponentHost;

		private ToolSet _toolSet;
		private bool _readonly;

		private EntityRef _patientProfileRef;
		private EntityRef _orderRef;

		/// <summary>
		/// Constructor to show/hide the summary section
		/// </summary>
		/// <param name="readonly">True to show the summary toolbar, false to hide it</param>
		/// <param name="mode">Set the component attachment mode</param>
		public AttachedDocumentPreviewComponent(bool @readonly, AttachmentMode mode)
		{
			_readonly = @readonly;
			_mode = mode;

			_patientAttachmentTable = new PatientAttachmentTable();
			_orderAttachmentTable = new OrderAttachmentTable();
		}

		public override void Start()
		{
			_toolSet = new ToolSet(new AttachedDocumentToolExtensionPoint(), new AttachedDocumentToolContext(this));

			_previewComponent = new AttachedDocumentDHtmlPreviewComponent(this);
			_previewComponentHost = new ChildComponentHost(this.Host, _previewComponent);
			_previewComponentHost.StartComponent();

			if (_mode == AttachmentMode.Patient)
				LoadPatientAttachments();
			else
				LoadOrderAttachments();

			base.Start();
		}

		public override void Stop()
		{
			if (_previewComponentHost != null)
			{
				_previewComponentHost.StopComponent();
				_previewComponentHost = null;
			}

			_toolSet.Dispose();

			base.Stop();
		}

		public void SaveChanges()
		{
			EventsHelper.Fire(_changeCommitted, this, EventArgs.Empty);
		}

		#region Events

		public event EventHandler ChangeCommited
		{
			add { _changeCommitted += value; }
			remove { _changeCommitted -= value; }
		}

		public event EventHandler SelectedDocumentChanged
		{
			add { _selectedDocumentChanged += value; }
			remove { _selectedDocumentChanged -= value; }
		}

		#endregion

		#region Presentation Models

		public ApplicationComponentHost PreviewHost
		{
			get { return _previewComponentHost; }
		}

		public bool Readonly
		{
			get { return _readonly; }
			set { _readonly = value; }
		}

		/// <summary>
		/// Gets and sets the patient owner.
		/// </summary>
		public EntityRef PatientProfileRef
		{
			get { return _patientProfileRef; }
			set
			{
				if (_patientProfileRef == value)
					return;

				_mode = AttachmentMode.Patient;
				_patientProfileRef = value;

				if (this.IsStarted)
					LoadPatientAttachments();
			}
		}

		/// <summary>
		/// Gets and sets the order owner.
		/// </summary>
		public EntityRef OrderRef
		{
			get { return _orderRef; }
			set
			{
				if (_orderRef == value)
					return;

				_mode = AttachmentMode.Order;
				_orderRef = value;
				LoadOrderAttachments();
			}
		}

		public IList<PatientAttachmentSummary> PatientAttachments
		{
			get { return _mode != AttachmentMode.Patient ? null : _patientAttachmentTable.Items; }
			set
			{
				_mode = AttachmentMode.Patient;
				_patientAttachmentTable.Items.Clear();
				_patientAttachmentTable.Items.AddRange(value);
			}
		}

		public IList<OrderAttachmentSummary> OrderAttachments
		{
			get { return _mode != AttachmentMode.Order ? null : _orderAttachmentTable.Items; }
			set
			{
				_mode = AttachmentMode.Order;
				_orderAttachmentTable.Items.Clear();
				_orderAttachmentTable.Items.AddRange(value);
			}
		}

		public ITable AttachmentTable
		{
			get
			{
				if (_mode == AttachmentMode.Patient)
					return _patientAttachmentTable;

				return _orderAttachmentTable;
			}
		}

		public ActionModelRoot AttachmentActionModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "attached-document-items", _toolSet.Actions); }
		}

		public override IActionSet ExportedActions
		{
			get { return _toolSet.Actions; }
		}

		public ISelection Selection
		{
			get { return new Selection(_selectedAttachment); }
			set
			{
				var newSelection = (AttachmentSummary)value.Item;
				if (_selectedAttachment != newSelection)
				{
					_selectedAttachment = newSelection;
					NotifyPropertyChanged("Selection");
					EventsHelper.Fire(_selectedDocumentChanged, this, EventArgs.Empty);
					_previewComponent.Refresh();
				}
			}
		}

		public void OnControlLoad()
		{
			if (_initialSelection != null)
				this.Selection = new Selection(_initialSelection);
		}

		public void SetInitialSelection(AttachmentSummary attachmentSummary)
		{
			_initialSelection = attachmentSummary;
		}

		#endregion

		private AttachedDocumentSummary SelectedDocument
		{
			get
			{
				if (_selectedAttachment == null)
					return null;

				var attachment = _selectedAttachment;
				return attachment == null ? null : attachment.Document;
			}
		}

		private void RemoveSelectedDocument()
		{
			if (_selectedAttachment == null)
				return;

			this.AttachmentTable.Items.Remove(_selectedAttachment);
			this.Modified = true;
		}

		private void LoadPatientAttachments()
		{
			if (_patientProfileRef == null)
				return;

			Async.Request(
				this,
				(IBrowsePatientDataService service) =>
				{
					var request = new GetDataRequest
					{
						GetPatientProfileDetailRequest = new GetPatientProfileDetailRequest
						{
							PatientProfileRef = _patientProfileRef,
							IncludeAttachments = true
						}
					};
					return service.GetData(request);
				},
				response =>
				{
					this.PatientAttachments = response.GetPatientProfileDetailResponse.PatientProfile.Attachments;
					if (this.PatientAttachments.Count > 0)
						this.SetInitialSelection(this.PatientAttachments[0]);
				});
		}

		private void LoadOrderAttachments()
		{
			if (_orderRef == null)
				return;

			Async.Request(
				this,
				(IBrowsePatientDataService service) =>
				{
					var request = new GetDataRequest
					{
						GetOrderDetailRequest = new GetOrderDetailRequest
						{
							OrderRef = _orderRef,
							IncludeAttachments = true
						}
					};
					return service.GetData(request);
				},
				response =>
				{
					this.OrderAttachments = response.GetOrderDetailResponse.Order.Attachments;
					if (this.OrderAttachments.Count > 0)
						this.SetInitialSelection(this.OrderAttachments[0]);
				});
		}
	}
}
