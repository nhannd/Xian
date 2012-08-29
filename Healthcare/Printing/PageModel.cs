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
using ClearCanvas.Enterprise.Core.Printing;

namespace ClearCanvas.Healthcare.Printing
{
	public abstract class PageModel : IPageModel
	{
		#region Facade classes

		/// <summary>
		/// Person name facade.
		/// </summary>
		public class NameFacade
		{
			private readonly PersonName _name;

			public NameFacade(PersonName name)
			{
				_name = name;
			}

			public string GivenName
			{
				get { return _name.GivenName; }
			}

			public string FamilyName
			{
				get { return _name.FamilyName; }
			}

			public string MiddleName
			{
				get { return _name.MiddleName; }
			}

			public override string ToString()
			{
				return _name.ToString();
			}
		}

		/// <summary>
		/// Patient identifier facade.
		/// </summary>
		public class IdentifierFacade
		{
			private readonly PatientIdentifier _identifier;

			public IdentifierFacade(PatientIdentifier identifier)
			{
				_identifier = identifier;
			}

			public string Id
			{
				get { return _identifier.Id; }
			}

			public override string ToString()
			{
				return _identifier.ToString();
			}
		}

		/// <summary>
		/// Address facade.
		/// </summary>
		public class AddressFacade
		{
			private readonly Address _address;

			internal AddressFacade(Address address)
			{
				_address = address;
			}

			public string Line1
			{
				get { return string.IsNullOrEmpty(_address.Unit) ? _address.Street : string.Format("{0} - {1}", _address.Unit, _address.Street); }
			}

			public string Line2
			{
				get { return string.Format("{0} {1} {2}", _address.City, _address.Province, _address.PostalCode); }
			}

			public string Street
			{
				get { return _address.Street; }
			}

			public string Unit
			{
				get { return _address.Unit; }
			}

			public string City
			{
				get { return _address.City; }
			}

			public string Province
			{
				get { return _address.Province; }
			}

			public string PostalCode
			{
				get { return _address.PostalCode; }
			}

			public string Country
			{
				get { return _address.Country; }
			}

			public override string ToString()
			{
				return _address.ToString();
			}
		}

		/// <summary>
		/// Letterhead facade.
		/// </summary>
		public class LetterheadFacade
		{
			private readonly PrintTemplateSettings _settings;
			private readonly AddressFacade _address;

			internal LetterheadFacade()
			{
				_settings = new PrintTemplateSettings();
				var address = new Address(
					_settings.LetterheadAddressStreet,
					_settings.LetterheadAddressUnit,
					_settings.LetterheadAddressCity,
					_settings.LetterheadAddressProvince,
					_settings.LetterheadAddressPostalCode,
					"",
					AddressType.B,
					null);
				_address = new AddressFacade(address);
			}

			public string FacilityTitle
			{
				get { return _settings.LetterheadFacilityTitle; }
			}

			public string FacilitySubtitle
			{
				get { return _settings.LetterheadFacilitySubtitle; }
			}

			public string LogoFile
			{
				get { return _settings.LetterheadLogoFile; }
			}

			public AddressFacade Address
			{
				get { return _address; }
			}

			public string Phone
			{
				get { return _settings.LetterheadPhone; }
			}

			public string Fax
			{
				get { return _settings.LetterheadFax; }
			}

			public string Email
			{
				get { return _settings.LetterheadEmail; }
			}
		}

		/// <summary>
		/// Patient facade.
		/// </summary>
		public class PatientFacade
		{
			private readonly PatientProfile _patientProfile;

			internal PatientFacade(PatientProfile patientProfile)
			{
				_patientProfile = patientProfile;
			}

			public NameFacade Name
			{
				get { return new NameFacade(_patientProfile.Name); }
			}

			public IdentifierFacade Mrn
			{
				get { return new IdentifierFacade(_patientProfile.Mrn); }
			}

			public string DateOfBirth
			{
				get { return FormatDateOfBirth(_patientProfile.DateOfBirth); }
			}

			public override string ToString()
			{
				return string.Format("{0} ({1})", this.Name, this.Mrn);
			}

			private string FormatDateOfBirth(DateTime? dateOfBirth)
			{
				//todo: can we centralize formatting somewhere
				return dateOfBirth == null ? "" : dateOfBirth.Value.ToString("yyyy-MM-dd");
			}
		}

		#endregion

		/// <summary>
		/// Gets the URL of the template.
		/// </summary>
		public abstract Uri TemplateUrl { get; }

		/// <summary>
		/// Gets the set of variables accessible to the template.
		/// </summary>
		public abstract Dictionary<string, object> Variables { get; }
	}
}
