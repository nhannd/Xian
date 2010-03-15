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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerMergePropertiesComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ExternalPractitionerMergePropertiesComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerMergePropertiesComponent class.
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerMergePropertiesComponentViewExtensionPoint))]
	public class ExternalPractitionerMergePropertiesComponent : ApplicationComponent
	{
		class PersonNameComparer : IEqualityComparer<PersonNameDetail>
		{
			public bool Equals(PersonNameDetail x, PersonNameDetail y)
			{
				return Equals(x.FamilyName, y.FamilyName)
					&& Equals(x.GivenName, y.GivenName)
					&& Equals(x.MiddleName, y.MiddleName)
					&& Equals(x.Prefix, y.Prefix)
					&& Equals(x.Suffix, y.Suffix)
					&& Equals(x.Degree, y.Degree);
			}

			public int GetHashCode(PersonNameDetail name)
			{
				var result = name.FamilyName != null ? name.FamilyName.GetHashCode() : 0;
				result = 29 * result + (name.GivenName != null ? name.GivenName.GetHashCode() : 0);
				result = 29 * result + (name.MiddleName != null ? name.MiddleName.GetHashCode() : 0);
				result = 29 * result + (name.Prefix != null ? name.Prefix.GetHashCode() : 0);
				result = 29 * result + (name.Suffix != null ? name.Suffix.GetHashCode() : 0);
				result = 29 * result + (name.Degree != null ? name.Degree.GetHashCode() : 0);
				return result;
			}
		}

		private ExternalPractitionerDetail _practitioner1;
		private ExternalPractitionerDetail _practitioner2;
		private readonly ExternalPractitionerDetail _merged;

		public ExternalPractitionerMergePropertiesComponent()
		{
			_merged = new ExternalPractitionerDetail();
		}

		public ExternalPractitionerDetail MergedPractitioner
		{
			get { return _merged; }
		}

		public ExternalPractitionerDetail Practitioner1
		{
			get { return _practitioner1; }
			set
			{
				_practitioner1 = value; 
				NotifyAllPropertiesChanged();
			}
		}

		public ExternalPractitionerDetail Practitioner2
		{
			get { return _practitioner2; }
			set
			{
				_practitioner2 = value;
				NotifyAllPropertiesChanged();
			}
		}

		public PersonNameDetail Name
		{
			get { return _merged.Name; }
			set { _merged.Name = value; }
		}

		public List<PersonNameDetail> NameChoices
		{
			get
			{
				if (this.Practitioner1 == null || this.Practitioner2 == null)
					return new List<PersonNameDetail>();

				var choices = new List<PersonNameDetail> {this.Practitioner1.Name, this.Practitioner2.Name};
				return CollectionUtils.Unique(choices, new PersonNameComparer());
			}
		}

		public string FormatName(object item)
		{
			var name = (PersonNameDetail) item;
			return PersonNameFormat.Format(name);
		}

		public string LicenseNumber
		{
			get { return _merged.LicenseNumber; }
			set { _merged.LicenseNumber = value; }
		}

		public List<string> LicenseNumberChoices
		{
			get
			{
				if (this.Practitioner1 == null || this.Practitioner2 == null)
					return new List<string>();

				var choices = new List<string> { this.Practitioner1.LicenseNumber, this.Practitioner2.LicenseNumber };
				return CollectionUtils.Unique(choices);
			}
		}

		public string BillingNumber
		{
			get { return _merged.BillingNumber; }
			set { _merged.BillingNumber = value; }
		}

		public List<string> BillingNumberChoices
		{
			get
			{
				if (this.Practitioner1 == null || this.Practitioner2 == null)
					return new List<string>();

				var choices = new List<string> { this.Practitioner1.BillingNumber, this.Practitioner2.BillingNumber };
				return CollectionUtils.Unique(choices);
			}
		}

	}
}
