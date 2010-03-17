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
	/// Extension point for views onto <see cref="ExternalPractitionerMergeSelectedContactPointsComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ExternalPractitionerMergeSelectedContactPointsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerMergeSelectedContactPointsComponent class.
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerMergeSelectedContactPointsComponentViewExtensionPoint))]
	public class ExternalPractitionerMergeSelectedContactPointsComponent : ApplicationComponent
	{
		private class ExternalPractitionerContactPointTable : Table<ExternalPractitionerContactPointDetail>
		{
			public ExternalPractitionerContactPointTable()
			{
				this.Columns.Add(new TableColumn<ExternalPractitionerContactPointDetail, bool>("Active",
					cp => !cp.Deactivated, (cp, value) => cp.Deactivated = !value, 0.15f));

				this.Columns.Add(new TableColumn<ExternalPractitionerContactPointDetail, string>("Name", cp => cp.Name, 0.5f));

				this.Columns.Add(new TableColumn<ExternalPractitionerContactPointDetail, string>("Description", cp => cp.Description, 0.5f));
			}
		}

		private readonly ExternalPractitionerContactPointTable _table;
		private ExternalPractitionerDetail _originalPractitioner;
		private ExternalPractitionerDetail _duplicatePractitioner;

		public ExternalPractitionerMergeSelectedContactPointsComponent()
		{
			_table = new ExternalPractitionerContactPointTable();
		}

		public override void Start()
		{
			this.Validation.Add(new ValidationRule("ContactPointTable",
				component => new ValidationResult(this.ActiveContactPoints.Count > 0, "Must have at least one contact point")));

			base.Start();
		}

		public ExternalPractitionerDetail OriginalPractitioner
		{
			get { return _originalPractitioner; }
			set
			{
				if (Equals(_originalPractitioner, value))
					return;

				if (value != null && _originalPractitioner != null && _originalPractitioner.PractitionerRef.Equals(value.PractitionerRef, true))
					return;

				_originalPractitioner = value;
				UpdateContactPointsTable();
			}
		}

		public ExternalPractitionerDetail DuplicatePractitioner
		{
			get { return _duplicatePractitioner; }
			set
			{
				if (Equals(_duplicatePractitioner, value))
					return;

				if (value != null && _duplicatePractitioner != null && _duplicatePractitioner.PractitionerRef.Equals(value.PractitionerRef, true))
					return;

				_duplicatePractitioner = value;
				UpdateContactPointsTable();
			}
		}

		public List<ExternalPractitionerContactPointDetail> ActiveContactPoints
		{
			get { return CollectionUtils.Select(_table.Items, cp => cp.Deactivated == false); }
		}

		public List<ExternalPractitionerContactPointDetail> DeactivatedContactPoints
		{
			get { return CollectionUtils.Select(_table.Items, cp => cp.Deactivated == true); }
		}

		public void Save(ExternalPractitionerDetail practitioner)
		{
			practitioner.ContactPoints.Clear();

			// Clone the contact points
			foreach (var cp in _table.Items)
			{
				var contactPoint = (ExternalPractitionerContactPointDetail)cp.Clone();
				practitioner.ContactPoints.Add(contactPoint);
			}
		}

		#region Presentation Models

		public ITable ContactPointTable
		{
			get { return _table; }
		}

		#endregion

		private void UpdateContactPointsTable()
		{
			if (_originalPractitioner == null || _duplicatePractitioner == null)
				return;

			var combinedContactPoints = new List<ExternalPractitionerContactPointDetail>();
			combinedContactPoints.AddRange(_originalPractitioner.ContactPoints);
			combinedContactPoints.AddRange(_duplicatePractitioner.ContactPoints);

			_table.Items.Clear();
			_table.Items.AddRange(combinedContactPoints);
		}
	}
}
