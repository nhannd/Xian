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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using AffectedOrderRecipientSummary = ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin.MergeExternalPractitionerRequest.AffectedOrderRecipientSummary;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerMergeAffectedOrdersComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ExternalPractitionerMergeAffectedOrdersComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerMergeAffectedOrdersComponent class.
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerMergeAffectedOrdersComponentViewExtensionPoint))]
	public class ExternalPractitionerMergeAffectedOrdersComponent : ApplicationComponent
	{
		private readonly List<ExternalPractitionerMergeAffectedOrderTableItem> _affectedOrderTableItems;
		private readonly Dictionary<AffectedOrderRecipientSummary, EntityRef> _contactPointReplacementMap;

		private List<ExternalPractitionerContactPointDetail> _activeContactPoints;
		private List<ExternalPractitionerContactPointDetail> _deactivatedContactPoints;

		public ExternalPractitionerMergeAffectedOrdersComponent()
		{
			_affectedOrderTableItems = new List<ExternalPractitionerMergeAffectedOrderTableItem>();
			_contactPointReplacementMap = new Dictionary<AffectedOrderRecipientSummary, EntityRef>();
		}

		public override void Start()
		{
			this.Validation.Add(new ValidationRule("SummarySelection",
				component => new ValidationResult(!this.HasUnspecifiedContactPoints, SR.MessageValidationMustSpecifyActiveContactPoint)));

			base.Start();
		}

		public void Save()
		{
			UpdateRecipientReplacementMap();
		}

		public List<ExternalPractitionerContactPointDetail> ActiveContactPoints
		{
			get { return _activeContactPoints; }
			set { _activeContactPoints = value; }
		}

		public List<ExternalPractitionerContactPointDetail> DeactivatedContactPoints
		{
			get { return _deactivatedContactPoints; }
			set
			{
				_deactivatedContactPoints = value;
				UpdateAffectedOrderTableItems();
			}
		}

		public Dictionary<AffectedOrderRecipientSummary, EntityRef> ContactPointReplacementMap
		{
			get
			{
				UpdateRecipientReplacementMap();
				return _contactPointReplacementMap;
			}
		}

		public bool HasUnspecifiedContactPoints
		{
			get { return CollectionUtils.Contains(_affectedOrderTableItems, item => item.SelectedContactPoint == null); }
		}

		#region Presentation Models

		public string Instruction
		{
			get { return SR.MessageInstructionAffectedOrders; }
		}

		public List<ExternalPractitionerMergeAffectedOrderTableItem> AffectedOrderTableItems
		{
			get { return _affectedOrderTableItems; }
		}

		#endregion

		private void UpdateAffectedOrderTableItems()
		{
			var deactivatedContactPointRefs = CollectionUtils.Map<ExternalPractitionerContactPointDetail, EntityRef>(_deactivatedContactPoints, cp => cp.ContactPointRef);
			var affectedOrders = LoadAffectedOrders(deactivatedContactPointRefs);

			UpdateRecipientReplacementMap();

			// From each affected recipient in each affected order, build AffectedOrderTableItem
			_affectedOrderTableItems.Clear();
			CollectionUtils.ForEach(affectedOrders, order => 
				CollectionUtils.ForEach(order.ResultRecipients,
					delegate(ResultRecipientDetail recipient)
					{
						var recipientFound = CollectionUtils.Contains(deactivatedContactPointRefs,
							cpRef => cpRef.Equals(recipient.ContactPoint.ContactPointRef, false));

						if (!recipientFound)
							return;

						var tableItem = new ExternalPractitionerMergeAffectedOrderTableItem(order, recipient, ShowOrderPreview, ShowPractitionerPreview)
							{
								SelectedContactPoint = GetDefaultOrPreviousSelection(order, recipient),
								ContactPointChoices = this.ActiveContactPoints
							};

						_affectedOrderTableItems.Add(tableItem);
					}));

			NotifyAllPropertiesChanged();
		}

		private void UpdateRecipientReplacementMap()
		{
			_contactPointReplacementMap.Clear();
			foreach (var item in _affectedOrderTableItems)
			{
				if (item.SelectedContactPoint == null)
					continue;

				var affectedOrderRecipientSummary = new AffectedOrderRecipientSummary(item.Order.OrderRef, item.Recipient.ContactPoint.ContactPointRef);
				_contactPointReplacementMap[affectedOrderRecipientSummary] = item.SelectedContactPoint.ContactPointRef;
			}
		}

		private ExternalPractitionerContactPointDetail GetDefaultOrPreviousSelection(OrderDetail order, ResultRecipientDetail recipient)
		{
			// Find out which contact point was picked previously
			EntityRef previousContactPointSelectionRef = null;
			CollectionUtils.ForEach(_contactPointReplacementMap.Keys, key =>
					{
						if (key.Equals(order.OrderRef, recipient.ContactPoint.ContactPointRef, true))
							previousContactPointSelectionRef = _contactPointReplacementMap[key];
					});

			// Find out if the previous picked contact point exist in the list of contact point showing.
			if (previousContactPointSelectionRef != null)
			{
				var previousSelection = CollectionUtils.SelectFirst(_activeContactPoints, cp => cp.ContactPointRef.Equals(previousContactPointSelectionRef, true));
				if (previousSelection != null)
					return previousSelection;
			}

			// If not, either nothing was selected before or the previous contact point is no longer available.
			// Default to the first element if there is only one to choose from.  Otherwise use the default contact point.
			return _activeContactPoints.Count == 1 
				? CollectionUtils.FirstElement(_activeContactPoints) 
				: CollectionUtils.SelectFirst(_activeContactPoints, cp => cp.IsDefaultContactPoint);
		}

		private static List<OrderDetail> LoadAffectedOrders(List<EntityRef> deactivatedContactPointRefs)
		{
			var affectedOrders = new List<OrderDetail>();

			if (deactivatedContactPointRefs != null && deactivatedContactPointRefs.Count > 0)
			{
				Platform.GetService(
					delegate(IExternalPractitionerAdminService service)
					{
						var request = new LoadMergeExternalPractitionerFormDataRequest { DeactivatedContactPointRefs = deactivatedContactPointRefs };
						var response = service.LoadMergeExternalPractitionerFormData(request);

						affectedOrders = response.AffectedOrders;
					});
			}

			return affectedOrders;
		}

		private void ShowOrderPreview(OrderDetail order)
		{
			var component = new BiographyOrderDetailViewComponent(order.OrderRef);
			LaunchAsDialog(this.Host.DesktopWindow, component, "Order");
		}

		private void ShowPractitionerPreview(ExternalPractitionerSummary practitioner)
		{
			var component = new ExternalPractitionerOverviewComponent {PractitionerSummary = practitioner};
			LaunchAsDialog(this.Host.DesktopWindow, component, "Practitioner");
		}
	}
}
