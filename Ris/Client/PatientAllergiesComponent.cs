#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class PatientAllergiesComponent : DHtmlComponent
	{
		// Internal data contract used for jscript deserialization
		[DataContract]
		public class AllergyContext : DataContractBase
		{
			public AllergyContext()
			{
				this.Allergies = new List<PatientAllergyDetail>();
				this.AllergenTypeChoices = new List<EnumValueInfo>();
				this.SeverityChoices = new List<EnumValueInfo>();
				this.SensitivityTypeChoices = new List<EnumValueInfo>();
				this.PersonRelationshipTypeChoices = new List<EnumValueInfo>();
			}

			[DataMember]
			public List<PatientAllergyDetail> Allergies;

			[DataMember]
			public List<EnumValueInfo> AllergenTypeChoices;

			[DataMember]
			public List<EnumValueInfo> SeverityChoices;

			[DataMember]
			public List<EnumValueInfo> SensitivityTypeChoices;

			[DataMember]
			public List<EnumValueInfo> PersonRelationshipTypeChoices;
		}

		private readonly bool _readOnly;
		private readonly AllergyContext _context;

		/// <summary>
		/// Constructor for readonly allergies information.
		/// </summary>
		public PatientAllergiesComponent()
		{
			_readOnly = true;
			_context = new AllergyContext();
		}

		/// <summary>
		/// Constructor for editing allergies.
		/// </summary>
		/// <param name="allergenTypeChoices"></param>
		/// <param name="severityChoices"></param>
		/// <param name="sensitivityTypeChoices"></param>
		/// <param name="personRelationshipTypeChoices"></param>
		public PatientAllergiesComponent(
			List<EnumValueInfo> allergenTypeChoices,
			List<EnumValueInfo> severityChoices,
			List<EnumValueInfo> sensitivityTypeChoices,
			List<EnumValueInfo> personRelationshipTypeChoices)
		{
			_readOnly = false;
			_context = new AllergyContext
				{
					AllergenTypeChoices = allergenTypeChoices,
					SeverityChoices = severityChoices,
					SensitivityTypeChoices = sensitivityTypeChoices,
					PersonRelationshipTypeChoices = personRelationshipTypeChoices
				};
		}

		public override void Start()
		{
			SetUrl(WebResourcesSettings.Default.BiographyAllergyDetailPageUrl);
			base.Start();
		}

		public List<PatientAllergyDetail> Allergies
		{
			get { return _context.Allergies; }
			set
			{
				_context.Allergies = value;
				NotifyAllPropertiesChanged();
			}
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
		}

		protected override string GetTag(string tag)
		{
			if (string.Equals("ReadOnly", tag))
			{
				return _readOnly ? "true" : "false";
			}

			return base.GetTag(tag);
		}

		protected override void SetTag(string tag, string data)
		{
			if (string.Equals("Allergies", tag))
			{
				this.Allergies.Clear();
				this.Allergies.AddRange(JsmlSerializer.Deserialize<List<PatientAllergyDetail>>(data));
				return;
			}

			base.SetTag(tag, data);
		}
	}
}
