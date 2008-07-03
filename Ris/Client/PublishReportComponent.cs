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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
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
	public abstract class PublishReportComponent : ApplicationComponent
	{
		public class PublishReportPreviewComponent : DHtmlComponent
		{
			[DataContract]
			public class PublishReportPreviewContext : DataContractBase
			{
				public PublishReportPreviewContext(
					EntityRef patientProfileRef,
					EntityRef orderRef,
					EntityRef reportRef,
					ResultRecipientDetail selectedResultRecipient)
				{
					this.PatientProfileRef = patientProfileRef;
					this.OrderRef = orderRef;
					this.ReportRef = reportRef;
					this.SelectedResultRecipient = selectedResultRecipient;
				}

				[DataMember]
				public EntityRef PatientProfileRef;

				[DataMember]
				public EntityRef OrderRef;

				[DataMember]
				public EntityRef ReportRef;

				[DataMember]
				public ResultRecipientDetail SelectedResultRecipient;
			}

			private PublishReportPreviewContext _context;

			public PublishReportPreviewComponent(EntityRef patientProfileRef, EntityRef orderRef, EntityRef reportRef)
			{
				Platform.CheckForNullReference(patientProfileRef, "patientProfileRef");
				Platform.CheckForNullReference(orderRef, "orderRef");
				Platform.CheckForNullReference(reportRef, "reportRef");

				_context = new PublishReportPreviewContext(patientProfileRef, orderRef, reportRef, null);
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

		private readonly EntityRef _patientProfileRef;
		private readonly EntityRef _orderRef;
		private readonly EntityRef _reportRef;

		private readonly Table<Checkable<ResultRecipientDetail>> _recipientsTable;
		private readonly CrudActionModel _recipientsActionModel;
		private Checkable<ResultRecipientDetail> _selectedRecipient;
		private ExternalPractitionerLookupHandler _recipientLookupHandler;
		private ExternalPractitionerSummary _recipientToAdd;
		private ExternalPractitionerContactPointDetail _recipientContactPointToAdd;
		private List<ExternalPractitionerContactPointDetail> _recipientContactPointChoices;

		private PublishReportPreviewComponent _publishReportPreviewComponent;
		private ChildComponentHost _publishReportPreviewComponentHost;

		#region Constructor

		/// <summary>
		/// Constructor.
		/// </summary>
		public PublishReportComponent(EntityRef patientProfileRef, EntityRef orderRef, EntityRef reportRef)
		{
			Platform.CheckForNullReference(patientProfileRef, "patientProfileRef");
			Platform.CheckForNullReference(orderRef, "orderRef");
			Platform.CheckForNullReference(reportRef, "reportRef");

			_patientProfileRef = patientProfileRef;
			_orderRef = orderRef;
			_reportRef = reportRef;

			_recipientsTable = new Table<Checkable<ResultRecipientDetail>>();
			_recipientsTable.Columns.Add(new TableColumn<Checkable<ResultRecipientDetail>, bool>(
				"Select",
				delegate(Checkable<ResultRecipientDetail> checkable) { return checkable.IsChecked; },
				delegate(Checkable<ResultRecipientDetail> checkable, bool isChecked) { checkable.IsChecked = isChecked; NotifyPropertyChanged("AcceptEnabled"); },
				0.3f));
			_recipientsTable.Columns.Add(new TableColumn<Checkable<ResultRecipientDetail>, string>(
				"Practitioner",
				delegate(Checkable<ResultRecipientDetail> checkable) { return PersonNameFormat.Format(checkable.Item.Practitioner.Name); }));
			_recipientsTable.Columns.Add(new TableColumn<Checkable<ResultRecipientDetail>, string>(
				"Contact Point",
				delegate(Checkable<ResultRecipientDetail> checkable) { return checkable.Item.ContactPoint.Name; }));

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
			_publishReportPreviewComponent = new PublishReportPreviewComponent(_patientProfileRef, _orderRef, _reportRef);
			_publishReportPreviewComponentHost = new ChildComponentHost(this.Host, _publishReportPreviewComponent);
			_publishReportPreviewComponentHost.StartComponent();

			_recipientLookupHandler = new ExternalPractitionerLookupHandler(this.Host.DesktopWindow);

			Platform.GetService<IBrowsePatientDataService>(
				delegate(IBrowsePatientDataService service)
				{
					GetDataRequest request = new GetDataRequest();
					request.GetOrderDetailRequest =
						new GetOrderDetailRequest(_orderRef, true, true, false, false, false, true);

					GetDataResponse response = service.GetData(request);

					_recipientsTable.Items.AddRange(
						CollectionUtils.Map<ResultRecipientDetail, Checkable<ResultRecipientDetail>>(
							response.GetOrderDetailResponse.Order.ResultRecipients,
							delegate(ResultRecipientDetail summary)
							{
								return new Checkable<ResultRecipientDetail>(summary, false);
							}));
				});

			base.Start();
		}

		/// <summary>
		/// Called by the host when the application component is being terminated.
		/// </summary>
		public override void Stop()
		{
			// TODO prepare the component to exit the live phase
			// This is a good place to do any clean up
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
			ExternalPractitionerContactPointDetail detail = (ExternalPractitionerContactPointDetail)cp;
			return ExternalPractitionerContactPointFormat.Format(detail);
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
				if (!object.Equals(value, _recipientToAdd))
				{
					_recipientToAdd = value;
					NotifyPropertyChanged("RecipientToAdd");

					_recipientContactPointToAdd = null;
					UpdateConsultantContactPointChoices();
					NotifyPropertyChanged("RecipientContactPointChoices");

					// must do this after contact point choices have been updated
					UpdateRecipientsActionModel();
				}
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
				if (_recipientContactPointToAdd != value)
				{
					_recipientContactPointToAdd = value;
					NotifyPropertyChanged("RecipientContactPointToAdd");
				}
			}
		}

		public ISelection SelectedRecipient
		{
			get { return new Selection(_selectedRecipient); }
			set
			{
				if (!object.Equals(value, _selectedRecipient))
				{
					_selectedRecipient = (Checkable<ResultRecipientDetail>)value.Item;
					UpdatePreview();
					UpdateRecipientsActionModel();
					NotifyPropertyChanged("SelectedRecipient");
				}
			}
		}

		public void AddRecipient()
		{
			if (_recipientToAdd != null && _recipientContactPointToAdd != null)
			{
				_recipientsTable.Items.Add(new Checkable<ResultRecipientDetail>(
					new ResultRecipientDetail(_recipientToAdd, _recipientContactPointToAdd, new EnumValueInfo("ANY", null, null)),
					true));
			}
		}


		public abstract void Accept();

		public bool AcceptEnabled
		{
			get
			{
				return CollectionUtils.Contains(
					_recipientsTable.Items,
					delegate(Checkable<ResultRecipientDetail> checkable) { return checkable.IsChecked; });
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
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
				_reportRef,
				_selectedRecipient != null ? _selectedRecipient.Item : null);
		}

		private void UpdateConsultantContactPointChoices()
		{
			_recipientContactPointChoices = GetPractitionerContactPoints(_recipientToAdd);
		}

		private List<ExternalPractitionerContactPointDetail> GetPractitionerContactPoints(ExternalPractitionerSummary prac)
		{
			List<ExternalPractitionerContactPointDetail> choices = new List<ExternalPractitionerContactPointDetail>();
			if (prac != null)
			{
				Platform.GetService<IOrderEntryService>(
					delegate(IOrderEntryService service)
					{
						GetExternalPractitionerContactPointsResponse response = service.GetExternalPractitionerContactPoints(
							new GetExternalPractitionerContactPointsRequest(prac.PractitionerRef));
						choices = response.ContactPoints;
					});
			}
			return choices;
		}

		#endregion
	}

	public class PrintReportComponent : PublishReportComponent
	{
		public PrintReportComponent(EntityRef patientProfileRef, EntityRef orderRef, EntityRef reportRef)
			: base(patientProfileRef, orderRef, reportRef)
		{
		}

		public override void Accept()
		{
			using (new HeaderFooterSettings())
			{
				foreach (Checkable<ResultRecipientDetail> checkable in this.Recipients.Items)
				{
					if (checkable.IsChecked)
					{
						ResultRecipientDetail detail = checkable.Item;

						PublishReportPreviewComponent component = new PublishReportPreviewComponent(this.PatientProfileRef, this.OrderRef, this.ReportRef);
						ChildComponentHost host = new ChildComponentHost(this.Host, component);
						host.StartComponent();
						object view = host.ComponentView.GuiElement;

						component.Context = new PublishReportPreviewComponent.PublishReportPreviewContext(
							this.PatientProfileRef,
							this.OrderRef,
							this.ReportRef,
							detail);

						component.ScriptCompleted += PrintDocument;
					}
				}
			}

			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		private void PrintDocument(object sender, EventArgs e)
		{
			PublishReportPreviewComponent component = (PublishReportPreviewComponent)sender;
			component.PrintDocument();
		}
	}

	public class FaxReportComponent : PublishReportComponent
	{
		public FaxReportComponent(EntityRef patientProfileRef, EntityRef orderRef, EntityRef reportRef)
			: base(patientProfileRef, orderRef, reportRef)
		{
		}

		public override void Accept()
		{
			throw new System.NotImplementedException();
		}
	}
}
