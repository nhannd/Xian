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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerReplaceDisabledContactPointsComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ExternalPractitionerReplaceDisabledContactPointsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerReplaceDisabledContactPointsComponent class.
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerReplaceDisabledContactPointsComponentViewExtensionPoint))]
	public class ExternalPractitionerReplaceDisabledContactPointsComponent : ApplicationComponent
	{
		private List<ExternalPractitionerContactPointDetail> _deactivatedContactPoints;
		private List<ExternalPractitionerContactPointDetail> _activeContactPoints;
		private readonly List<MergeExternalPractitionerRequest.ContactPointReplacement> _contactPointReplacements;
		private readonly List<ExternalPractitionerReplaceDisabledContactPointsTableItem> _tableItems;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExternalPractitionerReplaceDisabledContactPointsComponent()
		{
			_tableItems = new List<ExternalPractitionerReplaceDisabledContactPointsTableItem>();
			_contactPointReplacements = new List<MergeExternalPractitionerRequest.ContactPointReplacement>();
		}

		// Dummy property for binding the validation icon to.
		public ExternalPractitionerReplaceDisabledContactPointsTableItem ValidationPlaceHolder { get; set; }

		public List<ExternalPractitionerContactPointDetail> DeactivatedContactPoints
		{
			get
			{
				return _deactivatedContactPoints;
			}
			set
			{
				if (CollectionUtils.Equal<ExternalPractitionerContactPointDetail>(_deactivatedContactPoints, value, false))
					return;

				_deactivatedContactPoints = value;
				UpdateTableItems();
			}
		}

		public List<ExternalPractitionerContactPointDetail> ActiveContactPoints
		{
			get { return _activeContactPoints; }
			set { _activeContactPoints = value; }
		}

		public List<MergeExternalPractitionerRequest.ContactPointReplacement> ContactPointReplacements
		{
			get
			{
				UpdateContactPointReplacementMap();
				return _contactPointReplacements;
			}
		}

		public string Instruction
		{
			get { return SR.MessageInstructionReplacementContactPoints; }
		}

		public List<ExternalPractitionerReplaceDisabledContactPointsTableItem> ReplaceDisabledContactPointsTableItems
		{
			get { return _tableItems; }
		}

		public bool HasUnspecifiedContactPoints
		{
			get { return CollectionUtils.Contains(_tableItems, item => item.SelectedNewContactPoint == null); }
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			this.Validation.Add(new ValidationRule("ValidationPlaceHolder",
				component => new ValidationResult(!this.HasUnspecifiedContactPoints, SR.MessageValidationMustSpecifyActiveContactPoint)));

			base.Start();
		}

		public void Save()
		{
			UpdateContactPointReplacementMap();
		}

		private void UpdateContactPointReplacementMap()
		{
			_contactPointReplacements.Clear();
			foreach (var item in _tableItems)
			{
				if (item.SelectedNewContactPoint == null)
					continue;

				_contactPointReplacements.Add(new MergeExternalPractitionerRequest.ContactPointReplacement(
					item.OldContactPoint.ContactPointRef,
					item.SelectedNewContactPoint.ContactPointRef));
			}
		}

		private void UpdateTableItems()
		{
			_tableItems.Clear();
			foreach (var deactivatedContactPoint in this.DeactivatedContactPoints)
			{
				var tableItem = new ExternalPractitionerReplaceDisabledContactPointsTableItem(deactivatedContactPoint, this.ActiveContactPoints);
				tableItem.SelectedNewContactPointModified += delegate { this.ShowValidation(this.HasValidationErrors); };
				_tableItems.Add(tableItem);
			}

			NotifyAllPropertiesChanged();
		}
	}
}
