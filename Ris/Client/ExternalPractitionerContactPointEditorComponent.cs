#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// ExternalPractitionerContactPointEditorComponent class
	/// </summary>
	public class ExternalPractitionerContactPointEditorComponent : NavigatorComponentContainer
	{
		private readonly ExternalPractitionerContactPointDetail _contactPointDetail;

		private ExternalPractitionerContactPointDetailsEditorComponent _detailsEditor;
		private AddressesSummaryComponent _addressesSummary;
		private PhoneNumbersSummaryComponent _phoneNumbersSummary;
		private EmailAddressesSummaryComponent _emailAddressesSummary;

		private readonly List<EnumValueInfo> _addressTypeChoices;
		private readonly List<EnumValueInfo> _phoneTypeChoices;
		private readonly List<EnumValueInfo> _resultCommunicationModeChoices;

		/// <summary>
		/// Constructor
		/// </summary>
		public ExternalPractitionerContactPointEditorComponent(ExternalPractitionerContactPointDetail contactPoint, List<EnumValueInfo> addressTypeChoices, List<EnumValueInfo> phoneTypeChoices, List<EnumValueInfo> resultCommunicationModeChoices)
		{
			_contactPointDetail = contactPoint;
			_addressTypeChoices = addressTypeChoices;
			_phoneTypeChoices = phoneTypeChoices;
			_resultCommunicationModeChoices = resultCommunicationModeChoices;
		}

		public override void Start()
		{
			const string rootPath = "Contact Point";
			this.Pages.Add(new NavigatorPage(rootPath, _detailsEditor = new ExternalPractitionerContactPointDetailsEditorComponent(_contactPointDetail, _resultCommunicationModeChoices)));
			this.Pages.Add(new NavigatorPage(rootPath + "/Addresses", _addressesSummary = new AddressesSummaryComponent(_addressTypeChoices)));
			this.Pages.Add(new NavigatorPage(rootPath + "/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent(_phoneTypeChoices)));
			this.Pages.Add(new NavigatorPage(rootPath + "/Email Addresses", _emailAddressesSummary = new EmailAddressesSummaryComponent()));

			_addressesSummary.SetModifiedOnListChange = true;
			_phoneNumbersSummary.SetModifiedOnListChange = true;
			_emailAddressesSummary.SetModifiedOnListChange = true;

			this.ValidationStrategy = new AllComponentsValidationStrategy();

			_addressesSummary.Subject = _contactPointDetail.Addresses;
			_phoneNumbersSummary.Subject = _contactPointDetail.TelephoneNumbers;
			_emailAddressesSummary.Subject = _contactPointDetail.EmailAddresses;

			base.Start();
		}
	}
}
