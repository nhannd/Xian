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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
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
		private ExternalPractitionerContactPointDetail _defaultContactPoint;
		private List<ExternalPractitionerContactPointDetail> _activeContactPoints;

		public override void Start()
		{
			this.Validation.Add(new ValidationRule("DefaultContactPoint",
				component => new ValidationResult(this.DefaultContactPoint != null, "Must have at least one default contact point")));

			base.Start();
		}

		public List<ExternalPractitionerContactPointDetail> ActiveContactPoints
		{
			get { return _activeContactPoints; }
			set { UpdateContactPoints(value); }
		}

		public ExternalPractitionerContactPointDetail DefaultContactPoint
		{
			get { return _defaultContactPoint; }
			set { UpdateDefaultContactPoint(value); }
		}

		public string FormatContactPoint(object item)
		{
			var cp = (ExternalPractitionerContactPointDetail) item;
			return cp.Name;
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

		private void UpdateDefaultContactPoint(ExternalPractitionerContactPointDetail contactPoint)
		{
			_defaultContactPoint = contactPoint;

			if (_defaultContactPoint != null)
			{
				// Make sure the previously selected default contact point is maintained and there can only be one default
				foreach (var cp in _activeContactPoints)
				{
					cp.IsDefaultContactPoint = cp.ContactPointRef.Equals(_defaultContactPoint.ContactPointRef, false);
				}
			}

			NotifyPropertyChanged("DefaultContactPoint");
		}

		private void UpdateContactPoints(List<ExternalPractitionerContactPointDetail> contactPoints)
		{
			var previousDefault = _defaultContactPoint;
			_activeContactPoints = contactPoints;
			NotifyAllPropertiesChanged();

			var previousDefaultExist = previousDefault != null && CollectionUtils.Contains(_activeContactPoints, cp => cp.ContactPointRef.Equals(previousDefault.ContactPointRef, false));
			if (previousDefaultExist)
			{
				UpdateDefaultContactPoint(previousDefault);
			}
			else
			{
				// There may be two default contact points from both practitioner, find the first default, or set to the first element if there is no default
				var newDefault = CollectionUtils.SelectFirst(_activeContactPoints, cp => cp.IsDefaultContactPoint) ??
								CollectionUtils.FirstElement(_activeContactPoints);

				UpdateDefaultContactPoint(newDefault);
			}
		}

	}
}
