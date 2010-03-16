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
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerMergeSelectDefaultContactPointComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ExternalPractitionerMergeSelectDefaultContactPointComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerMergeSelectDefaultContactPointComponent class.
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerMergeSelectDefaultContactPointComponentViewExtensionPoint))]
	public class ExternalPractitionerMergeSelectDefaultContactPointComponent : ApplicationComponent
	{
		private class ExternalPractitionerContactPointsTable : Table<ExternalPractitionerContactPointDetail>
		{
			public ExternalPractitionerContactPointsTable()
			{
				this.Columns.Add(new TableColumn<ExternalPractitionerContactPointDetail, bool>("Default",
					cp => cp.IsDefaultContactPoint,
					OnItemChecked,
					0.15f));

				this.Columns.Add(new TableColumn<ExternalPractitionerContactPointDetail, string>("Name",
					cp => cp.Name, 0.5f));

				this.Columns.Add(new TableColumn<ExternalPractitionerContactPointDetail, string>("Description",
					cp => cp.Description, 0.5f));
			}

			private void OnItemChecked(ExternalPractitionerContactPointDetail item, bool value)
			{
				// Uncheck every other item, except the checked item
				foreach (var cp in this.Items)
				{
					cp.IsDefaultContactPoint = cp.ContactPointRef.Equals(item.ContactPointRef, false) ? value : false;
					this.Items.NotifyItemUpdated(cp);
				}
			}
		}

		private readonly ExternalPractitionerContactPointsTable _table;

		public ExternalPractitionerMergeSelectDefaultContactPointComponent()
		{
			_table = new ExternalPractitionerContactPointsTable();
		}

		public override void Start()
		{
			this.Validation.Add(new ValidationRule("ContactPointTable",
				component => new ValidationResult(this.DefaultContactPoint != null, "Must have at least one default contact point")));

			base.Start();
		}

		public IList<ExternalPractitionerContactPointDetail> ActiveContactPoints
		{
			get { return _table.Items; }
			set { UpdateContactPointsTable(value); }
		}

		public ExternalPractitionerContactPointDetail DefaultContactPoint
		{
			get { return CollectionUtils.SelectFirst(_table.Items, cp => cp.IsDefaultContactPoint); }
		}

		public void Save(ExternalPractitionerDetail practitioner)
		{
			var defaultContactPoint = this.DefaultContactPoint;

			// Update IsDefaultContactPoint property of all contact points.
			if (defaultContactPoint == null)
			{
				foreach (var cp in practitioner.ContactPoints)
					cp.IsDefaultContactPoint = false;
			}
			else
			{
				foreach (var cp in practitioner.ContactPoints)
					cp.IsDefaultContactPoint = cp.ContactPointRef.Equals(defaultContactPoint.ContactPointRef, false);
			}
		}

		#region Presentation Models

		public ITable ContactPointTable
		{
			get { return _table; }
		}

		#endregion

		private void UpdateContactPointsTable(IEnumerable<ExternalPractitionerContactPointDetail> contactPoints)
		{
			var previousDefault = this.DefaultContactPoint;

			_table.Items.Clear();
			_table.Items.AddRange(contactPoints);

			var currentDefault = previousDefault ?? this.DefaultContactPoint;
			if (currentDefault == null)
				return;

			// There may be two default contact points from both practitioner
			// Make sure the previously selected default contact point is maintained
			// Make sure there can only be one default
			foreach (var cp in _table.Items)
			{
				cp.IsDefaultContactPoint = cp.ContactPointRef.Equals(currentDefault.ContactPointRef, false);
			}
		}
	}
}
