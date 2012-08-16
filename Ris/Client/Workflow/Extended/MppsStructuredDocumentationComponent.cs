#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionOf(typeof(PerformedStepEditorPageProviderExtensionPoint))]
	public class MppsStructuredDocumentationComponentProvider : IPerformedStepEditorPageProvider
	{
		public IPerformedStepEditorPage[] GetPages(IPerformedStepEditorContext context)
		{
			var component = new MppsStructuredDocumentationComponent(context);
			return new IPerformedStepEditorPage[] {component};
		}
	}

	class MppsStructuredDocumentationComponent : DHtmlComponent, IPerformedStepEditorPage
	{
		#region HealthcareContext

		[DataContract]
		class HealthcareContext : DataContractBase
		{
			private readonly MppsStructuredDocumentationComponent _owner;

			public HealthcareContext(MppsStructuredDocumentationComponent owner)
			{
				_owner = owner;
			}

			[DataMember]
			public EntityRef OrderRef
			{
				get { return _owner._context.OrderRef; }
			}

			[DataMember]
			public EntityRef PatientRef
			{
				get { return _owner._context.PatientRef; }
			}

			[DataMember]
			public EntityRef PatientProfileRef
			{
				get { return _owner._context.PatientProfileRef; }
			}

			[DataMember]
			public ModalityPerformedProcedureStepDetail ModalityPerformedProcedureStep
			{
				get { return _owner._context.SelectedPerformedStep; }
			}
		}

		#endregion

		private readonly IPerformedStepEditorContext _context;

		public MppsStructuredDocumentationComponent(IPerformedStepEditorContext context)
		{
			_context = context;
		}

		public override void Start()
		{
			// when the selected step changes, refresh the browser
			_context.SelectedPerformedStepChanged += delegate
			{
				SetUrl(WebResourcesSettings.Default.DetailsPageUrl);
			};

			base.Start();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return new HealthcareContext(this);
		}

		protected override IDictionary<string, string> TagData
		{
			get { return _context.SelectedPerformedStepExtendedProperties; }
		}

		#region IPerformedStepEditorPage Members

		Path IExtensionPage.Path
		{
			get { return new Path("Details", new ResourceResolver(this.GetType().Assembly)); }
		}

		IApplicationComponent IExtensionPage.GetComponent()
		{
			return this;
		}

		void IPerformedStepEditorPage.Save()
		{
			SaveData();
		}

		#endregion
	}
}
