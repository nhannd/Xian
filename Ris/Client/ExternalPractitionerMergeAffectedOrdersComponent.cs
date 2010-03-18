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

using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Client.Formatting;

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
		private class AffectedOrderTableItem
		{
			public OrderDetail Order;

			public ResultRecipientDetail Recipient;

			public string Role
			{
				get { return this.Recipient.Practitioner.PractitionerRef.Equals(this.Order.OrderingPractitioner.PractitionerRef, false)  ? "Ordering" : "Copies To"; }
			}

			public ExternalPractitionerContactPointDetail SelectedContactPoint;
		}

		private class AffectedOrdersTable : Table<AffectedOrderTableItem>
		{
			private readonly ExternalPractitionerMergeAffectedOrdersComponent _owner;

			public AffectedOrdersTable(ExternalPractitionerMergeAffectedOrdersComponent owner)
			{
				_owner = owner;

				this.Columns.Add(new TableColumn<AffectedOrderTableItem, string>("Acc #", item => item.Order.AccessionNumber, 0.5f));
				this.Columns.Add(new TableColumn<AffectedOrderTableItem, string>("Practitioner", item => PersonNameFormat.Format(item.Recipient.Practitioner.Name), 1.0f));
				this.Columns.Add(new TableColumn<AffectedOrderTableItem, string>("Role", item => item.Role, 1.0f));
				this.Columns.Add(new TableColumn<AffectedOrderTableItem, string>("Deactivated Contact Point", item => FormatItem(item.Recipient.ContactPoint), 1.0f));

				var contactPointChoiceColumn = new TableColumn<AffectedOrderTableItem, ExternalPractitionerContactPointDetail>(
					"Active Contact Points",
					item => item.SelectedContactPoint,
					(x, value) => x.SelectedContactPoint = value,
					2.0f)
				{
					ValueFormatter = FormatItem,
					CellEditor = new ComboBoxCellEditor(GetChoices, FormatItem)
				};

				this.Columns.Add(contactPointChoiceColumn);
			}

			public bool HasUnspecifiedContactPoints
			{
				get { return CollectionUtils.Contains(this.Items, item => item.SelectedContactPoint == null); }
			}

			private IList GetChoices()
			{
				return _owner.ActiveContactPoints;
			}

			private static string FormatItem(object item)
			{
				if (item == null)
					return null;

				var cp = (ExternalPractitionerContactPointDetail)item;
				return cp.Name;
			}
		}

		private readonly AffectedOrdersTable _table;
		private ExternalPractitionerContactPointDetail _defaultContactPoint;
		private List<ExternalPractitionerContactPointDetail> _activeContactPoints;
		private List<ExternalPractitionerContactPointDetail> _deactivatedContactPoints;
		private Dictionary<EntityRef, EntityRef> _contactPointReplacementMap;

		public ExternalPractitionerMergeAffectedOrdersComponent()
		{
			_table = new AffectedOrdersTable(this);
			_contactPointReplacementMap = new Dictionary<EntityRef, EntityRef>();
		}

		public override void Start()
		{
			this.Validation.Add(new ValidationRule("AffectedOrderTable",
				component => new ValidationResult(!_table.HasUnspecifiedContactPoints, "Must specify all replacement contact points")));

			base.Start();
		}

		public ExternalPractitionerContactPointDetail DefaultContactPoint
		{
			get { return _defaultContactPoint; }
			set { _defaultContactPoint = value; }
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
				UpdateTable();
			}
		}

		public Dictionary<EntityRef, EntityRef> ContactPointReplacementMap
		{
			get
			{
				UpdateContactReplacementMap();
				return _contactPointReplacementMap;
			}
		}

		#region Presentation Models

		public ITable AffectedOrderTable
		{
			get { return _table; }
		}

		#endregion

		private void UpdateTable()
		{
			var deactivatedContactPointRefs = CollectionUtils.Map<ExternalPractitionerContactPointDetail, EntityRef>(_deactivatedContactPoints, cp => cp.ContactPointRef);
			var affectedOrders = LoadAffectedOrders(deactivatedContactPointRefs);

			UpdateContactReplacementMap();

			_table.Items.Clear();
			foreach (var order in affectedOrders)
			{
				foreach (var r in order.ResultRecipients)
				{
					var recipient = r;
					var recipientFound = CollectionUtils.Contains(deactivatedContactPointRefs, cpRef => cpRef.Equals(recipient.ContactPoint.ContactPointRef, false));

					if (!recipientFound)
						continue;

					var tableItem = new AffectedOrderTableItem
						{
							Order = order,
							Recipient = recipient,
							SelectedContactPoint = GetSelectedContactPoint(recipient.ContactPoint)
						};

					_table.Items.Add(tableItem);
				}
			}
		}

		private void UpdateContactReplacementMap()
		{
			_contactPointReplacementMap.Clear();
			foreach (var item in _table.Items)
			{
				if (item.SelectedContactPoint != null)
					_contactPointReplacementMap.Add(item.Recipient.ContactPoint.ContactPointRef, item.SelectedContactPoint.ContactPointRef);
			}
		}

		private ExternalPractitionerContactPointDetail GetSelectedContactPoint(ExternalPractitionerContactPointDetail original)
		{
			if (_contactPointReplacementMap.ContainsKey(original.ContactPointRef))
			{
				var previousContactPointSelectionRef = _contactPointReplacementMap[original.ContactPointRef];

				var previousSelection = CollectionUtils.SelectFirst(_activeContactPoints, cp => cp.ContactPointRef.Equals(previousContactPointSelectionRef));
				if (previousSelection != null)
					return previousSelection;
			}

			// Default to the first element if there is only one to choose from.
			return _activeContactPoints.Count == 1 ? CollectionUtils.FirstElement(_activeContactPoints) : null;
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
	}
}
