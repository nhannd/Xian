#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class ExternalPractitionerOverviewComponent : DHtmlComponent
	{
		// Internal data contract used for jscript deserialization
		[DataContract]
		public class ExternalPractitionerContext : DataContractBase
		{
			[DataMember]
			public ExternalPractitionerSummary PractitionerSummary;

			[DataMember]
			public ExternalPractitionerDetail PractitionerDetail;
		}

		private readonly ExternalPractitionerContext _context;

		public ExternalPractitionerOverviewComponent()
		{
			_context = new ExternalPractitionerContext();
		}

		public override void Start()
		{
			Refresh();
			base.Start();
		}

		public string PreviewUrl
		{
			get { return WebResourcesSettings.Default.ExternalPractitionerOverviewPageUrl; }
		}

		public void Refresh()
		{
			this.SetUrl(this.PreviewUrl);
		}

		public ExternalPractitionerSummary PractitionerSummary
		{
			set
			{
				_context.PractitionerSummary = value;
				_context.PractitionerDetail = null;
				Refresh();
			}
		}

		public ExternalPractitionerDetail PractitionerDetail
		{
			set
			{
				_context.PractitionerSummary = null;
				_context.PractitionerDetail = value;
				Refresh();
			}
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
		}
	}
}
