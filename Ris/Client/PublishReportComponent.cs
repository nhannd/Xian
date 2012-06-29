#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="PublishReportComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ReportPublishComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// PublishReportComponent class.
	/// </summary>
	[AssociateView(typeof(ReportPublishComponentViewExtensionPoint))]
	public class PublishReportComponent : ApplicationComponent
	{
		public class PublishReportPreviewComponent : DHtmlComponent
		{
			[DataContract]
			public class PublishReportPreviewContext : DataContractBase
			{
				public PublishReportPreviewContext(
					EntityRef patientProfileRef,
					EntityRef orderRef,
					EntityRef procedureRef,
					EntityRef reportRef,
					ResultRecipientDetail selectedResultRecipient)
				{
					this.PatientProfileRef = patientProfileRef;
					this.OrderRef = orderRef;
					this.ProcedureRef = procedureRef;
					this.ReportRef = reportRef;
					this.SelectedResultRecipient = selectedResultRecipient;
				}

				[DataMember]
				public EntityRef PatientProfileRef;

				[DataMember]
				public EntityRef OrderRef;

				[DataMember]
				public EntityRef ProcedureRef;

				[DataMember]
				public EntityRef ReportRef;

				[DataMember]
				public ResultRecipientDetail SelectedResultRecipient;
			}

			private PublishReportPreviewContext _context;

			public PublishReportPreviewComponent(EntityRef patientProfileRef, EntityRef orderRef, EntityRef procedureRef, EntityRef reportRef)
			{
				Platform.CheckForNullReference(patientProfileRef, "patientProfileRef");
				Platform.CheckForNullReference(orderRef, "orderRef");

				_context = new PublishReportPreviewContext(patientProfileRef, orderRef, procedureRef, reportRef, null);
			}

			public override void Start()
			{
				SetUrl(this.PageUrl);
				base.Start();
			}

			public void Refresh()
			{
				NotifyAllPropertiesChanged();
			}

			protected virtual string PageUrl
			{
				get { return WebResourcesSettings.Default.PrintReportPreviewUrl; }
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return this.Context;
			}

			public PublishReportPreviewContext Context
			{
				get { return _context; }
				set
				{
					_context = value;
					Refresh();
				}
			}
		}

		class ResultRecipientTable : Table<Checkable<ResultRecipientDetail>>
		{
			private readonly PublishReportComponent _owner;

			public ResultRecipientTable(PublishReportComponent owner)
			{
				_owner = owner;

				this.Columns.Add(new TableColumn<Checkable<ResultRecipientDetail>, bool>(SR.ColumnSelect,
					checkable => checkable.IsChecked, OnRecipientChecked, 0.3f));
				this.Columns.Add(new TableColumn<Checkable<ResultRecipientDetail>, string>(SR.ColumnPractitioner,
					checkable => PersonNameFormat.Format(checkable.Item.Practitioner.Name)));
				this.Columns.Add(new TableColumn<Checkable<ResultRecipientDetail>, string>(SR.ColumnContactPoint,
					checkable => checkable.Item.ContactPoint.Name));
			}

			private void OnRecipientChecked(Checkable<ResultRecipientDetail> checkable, bool isChecked)
			{
				checkable.IsChecked = isChecked;
				_owner.NotifyPropertyChanged("AcceptEnabled");
			}
		}
		
		private readonly EntityRef _patientProfileRef;
		private readonly EntityRef _orderRef;
		private readonly EntityRef _procedureRef;
		private readonly EntityRef _reportRef;

		private readonly ResultRecipientTable _recipientsTable;
		private readonly CrudActionModel _recipientsActionModel;
		private Checkable<ResultRecipientDetail> _selectedRecipient;
		private ExternalPractitionerLookupHandler _recipientLookupHandler;
		private ExternalPractitionerSummary _recipientToAdd;
		private ExternalPractitionerContactPointDetail _recipientContactPointToAdd;
		private List<ExternalPractitionerContactPointDetail> _recipientContactPointChoices;

		private PublishReportPreviewComponent _publishReportPreviewComponent;
		private ChildComponentHost _publishReportPreviewComponentHost;

		private readonly Queue<ResultRecipientDetail> _localPrintQueue = new Queue<ResultRecipientDetail>();

		private IEHeaderFooterSettings _headerFooterSettings;
		private IEPrintBackgroundSettings _printBackgroundSettings;

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public PublishReportComponent(EntityRef patientProfileRef, EntityRef orderRef, EntityRef procedureRef, EntityRef reportRef)
		{
			Platform.CheckForNullReference(patientProfileRef, "patientProfileRef");
			Platform.CheckForNullReference(orderRef, "orderRef");

			_patientProfileRef = patientProfileRef;
			_orderRef = orderRef;
			_procedureRef = procedureRef;
			_reportRef = reportRef;

			_recipientsTable = new ResultRecipientTable(this);

			_recipientsActionModel = new CrudActionModel(true, false, false);
			_recipientsActionModel.Add.SetClickHandler(AddRecipient);
			_recipientsActionModel.Add.Visible = false;    // hide this action on the menu/toolbar - we'll use a special button instead
		}

		#endregion

		#region ApplicationComponent overrides

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			_publishReportPreviewComponent = new PublishReportPreviewComponent(_patientProfileRef, _orderRef, _procedureRef, _reportRef);
			_publishReportPreviewComponentHost = new ChildComponentHost(this.Host, _publishReportPreviewComponent);
			_publishReportPreviewComponentHost.StartComponent();

			_recipientLookupHandler = new ExternalPractitionerLookupHandler(this.Host.DesktopWindow);

			Platform.GetService(
				delegate(IBrowsePatientDataService service)
				{
					var request = new GetDataRequest
						{ GetOrderDetailRequest = new GetOrderDetailRequest(_orderRef, true, true, false, false, false, true) };

					var response = service.GetData(request);

					_recipientsTable.Items.AddRange(
						CollectionUtils.Map<ResultRecipientDetail, Checkable<ResultRecipientDetail>>(
							response.GetOrderDetailResponse.Order.ResultRecipients,
							summary => new Checkable<ResultRecipientDetail>(summary, false)));
				});

			_headerFooterSettings = new IEHeaderFooterSettings(
				PublishReportComponentSettings.Default.ReportHeader,
				PublishReportComponentSettings.Default.ReportFooter);
			_printBackgroundSettings = new IEPrintBackgroundSettings();

			base.Start();
		}

		public override void Stop()
		{
			if (_publishReportPreviewComponentHost != null)
			{
				_publishReportPreviewComponentHost.StopComponent();
				_publishReportPreviewComponentHost = null;
			}

			base.Stop();
		}

		#endregion

		protected EntityRef ReportRef
		{
			get { return _reportRef; }
		}

		protected EntityRef OrderRef
		{
			get { return _orderRef; }
		}

		protected EntityRef ProcedureRef
		{
			get { return _procedureRef; }
		}

		protected EntityRef PatientProfileRef
		{
			get { return _patientProfileRef; }
		}

		protected PublishReportPreviewComponent PreviewComponent
		{
			get { return _publishReportPreviewComponent; }
		}

		#region Presentation model

		public ApplicationComponentHost PublishReportPreviewComponentHost
		{
			get { return _publishReportPreviewComponentHost; }
		}

		public string FormatContactPoint(object cp)
		{
			return ExternalPractitionerContactPointFormat.Format((ExternalPractitionerContactPointDetail)cp);
		}

		public ITable Recipients
		{
			get { return _recipientsTable; }
		}

		public CrudActionModel RecipientsActionModel
		{
			get { return _recipientsActionModel; }
		}

		public ILookupHandler RecipientsLookupHandler
		{
			get { return _recipientLookupHandler; }
		}

		public ExternalPractitionerSummary RecipientToAdd
		{
			get { return _recipientToAdd; }
			set
			{
				if (Equals(value, _recipientToAdd))
					return;

				_recipientToAdd = value;
				NotifyPropertyChanged("RecipientToAdd");

				_recipientContactPointToAdd = null;
				UpdateConsultantContactPointChoices();
				NotifyPropertyChanged("RecipientContactPointChoices");

				// must do this after contact point choices have been updated
				UpdateRecipientsActionModel();
			}
		}

		public IList RecipientContactPointChoices
		{
			get { return _recipientContactPointChoices; }
		}

		public ExternalPractitionerContactPointDetail RecipientContactPointToAdd
		{
			get { return _recipientContactPointToAdd; }
			set
			{
				if (_recipientContactPointToAdd == value)
					return;

				_recipientContactPointToAdd = value;
				NotifyPropertyChanged("RecipientContactPointToAdd");
			}
		}

		public ISelection SelectedRecipient
		{
			get { return new Selection(_selectedRecipient); }
			set
			{
				if (Equals(value, _selectedRecipient))
					return;

				_selectedRecipient = (Checkable<ResultRecipientDetail>)value.Item;
				UpdatePreview();
				UpdateRecipientsActionModel();
				NotifyPropertyChanged("SelectedRecipient");
			}
		}

		public void AddRecipient()
		{
			if (_recipientToAdd == null || _recipientContactPointToAdd == null)
				return;

			var newRecipient = new ResultRecipientDetail(_recipientToAdd, _recipientContactPointToAdd, new EnumValueInfo("ANY", null, null));
			_recipientsTable.Items.Add(new Checkable<ResultRecipientDetail>(newRecipient, true));
			NotifyPropertyChanged("AcceptEnabled");
		}

		public bool PublishVisible
		{
			get { return Thread.CurrentPrincipal.IsInRole(Application.Common.AuthorityTokens.Workflow.Report.SendToFaxQueue); }
		}

		public bool AcceptEnabled
		{
			get { return CollectionUtils.Contains( _recipientsTable.Items, checkable => checkable.IsChecked); }
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		public void SendReportToQueue()
		{
			var checkedRecipients = CollectionUtils.Map<Checkable<ResultRecipientDetail>, ResultRecipientDetail>(
				CollectionUtils.Select<Checkable<ResultRecipientDetail>>(this.Recipients.Items, checkableItem => checkableItem.IsChecked),
				checkableItem => checkableItem.Item);

			// Checks for invalid selections and prevents any from reaching the queue
			List<string> failedReasons;
			if (ValidateResultRecipients(checkedRecipients, out failedReasons) == false)
			{
				failedReasons.Insert(0, SR.MessageWarningInvalidResultRecipient);
				var reason = StringUtilities.Combine(failedReasons, Environment.NewLine);
				this.Host.ShowMessageBox(reason, MessageBoxActions.Ok);
				return;
			}

			try
			{
				var publishedRecipients = CollectionUtils.Map<ResultRecipientDetail, PublishRecipientDetail>(checkedRecipients,
					recipient => new PublishRecipientDetail(recipient.Practitioner.PractitionerRef, recipient.ContactPoint.ContactPointRef));

				Platform.GetService(
					delegate(IReportingWorkflowService service)
					{
						var request = new SendReportToQueueRequest(this.ProcedureRef) { Recipients = publishedRecipients };
						service.SendReportToQueue(request);
					});

				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
				this.Exit(ApplicationComponentExitCode.Error);
			}
		}

		/// <summary>
		/// Validate a list of recipients based on abscence of mail, fax, or email
		/// </summary>
		/// <param name="recipients"></param>
		/// <param name="failedReasons"></param>
		/// <returns></returns>
		private static bool ValidateResultRecipients(IEnumerable<ResultRecipientDetail> recipients, out List<string> failedReasons)
		{
			failedReasons = new List<string>();
			foreach (var recipient in recipients)
			{
				var formattedResultRecipient = string.Format("{0} ({1})", PersonNameFormat.Format(recipient.Practitioner.Name), recipient.ContactPoint.Name);

				if (recipient.PreferredCommunicationMode.Code == "FAX" && recipient.ContactPoint.CurrentFaxNumber == null)
					failedReasons.Add(string.Format(SR.MessageResultRecipientFaxNumberMissing, formattedResultRecipient));
				else if (recipient.PreferredCommunicationMode.Code == "MAIL" && recipient.ContactPoint.CurrentAddress == null)
					failedReasons.Add(string.Format(SR.MessageResultRecipientAddressMissing, formattedResultRecipient));
				else if (recipient.PreferredCommunicationMode.Code == "EMAIL" && recipient.ContactPoint.CurrentEmailAddress == null)
					failedReasons.Add(string.Format(SR.MessageResultRecipientEmailMissing, formattedResultRecipient));
				else if (recipient.ContactPoint.CurrentFaxNumber == null &&
						recipient.ContactPoint.CurrentAddress == null &&
						recipient.ContactPoint.CurrentEmailAddress == null)
					failedReasons.Add(string.Format(SR.MessageResultRecipientFaxAddressEmailMissing, formattedResultRecipient));
			}

			return failedReasons.Count == 0;
		}

		public void PrintLocal()
		{
			// add all recipients to queue
			_localPrintQueue.Clear();

			var checkedRecipients = CollectionUtils.Map<Checkable<ResultRecipientDetail>, ResultRecipientDetail>(
				CollectionUtils.Select<Checkable<ResultRecipientDetail>>(this.Recipients.Items, checkableItem => checkableItem.IsChecked),
				checkableItem => checkableItem.Item);

			foreach (var recipient in checkedRecipients)
			{
				_localPrintQueue.Enqueue(recipient);
			}

			// hook up the script completed event so that we print document when rendered
			_publishReportPreviewComponent.ScriptCompleted += OnDocumentRendered;

			// start processing the print queue
			PrintNextCopy();
		}

		#endregion

		#region private methods

		private void UpdateRecipientsActionModel()
		{
			_recipientsActionModel.Add.Enabled = (_recipientToAdd != null && _recipientContactPointToAdd != null);
		}

		private void UpdatePreview()
		{
			_publishReportPreviewComponent.Context = new PublishReportPreviewComponent.PublishReportPreviewContext(
				_patientProfileRef,
				_orderRef,
				_procedureRef,
				_reportRef,
				_selectedRecipient != null ? _selectedRecipient.Item : null);
		}

		private void UpdateConsultantContactPointChoices()
		{
			_recipientContactPointChoices = GetPractitionerContactPoints(_recipientToAdd);
		}

		private static List<ExternalPractitionerContactPointDetail> GetPractitionerContactPoints(ExternalPractitionerSummary prac)
		{
			var choices = new List<ExternalPractitionerContactPointDetail>();
			if (prac != null)
			{
				Platform.GetService(
					delegate(IOrderEntryService service)
					{
						var response = service.GetExternalPractitionerContactPoints(
							new GetExternalPractitionerContactPointsRequest(prac.PractitionerRef));
						choices = response.ContactPoints;
					});
			}

			return choices;
		}

		private void PrintNextCopy()
		{
			if(_localPrintQueue.Count > 0)
			{
				var recipient = _localPrintQueue.Dequeue();

				_publishReportPreviewComponent.Context = new PublishReportPreviewComponent.PublishReportPreviewContext(
					this.PatientProfileRef,
					this.OrderRef,
					this.ProcedureRef,
					this.ReportRef,
					recipient);
			}
			else
			{
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
		}

		private void OnDocumentRendered(object sender, EventArgs e)
		{
			// print the rendered document
			_publishReportPreviewComponent.PrintDocument();

			// TODO why is this here????
			// perhaps just a hokey way of ensuring that the browser actually finishes printing before we render next document?
			Thread.Sleep(1000);

			// process next queued item
			PrintNextCopy();
		}

		#endregion
	}
}
