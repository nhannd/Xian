#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerContactPointDetailsEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ExternalPractitionerContactPointDetailsEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerContactPointDetailsEditorComponent class
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerContactPointDetailsEditorComponentViewExtensionPoint))]
	public class ExternalPractitionerContactPointDetailsEditorComponent : ApplicationComponent
	{
		private readonly ExternalPractitionerContactPointDetail _contactPointDetail;
		private readonly IList<EnumValueInfo> _resultCommunicationModeChoices;
		private readonly IList<EnumValueInfo> _informationAuthorityChoices;
		private static readonly EnumValueInfo _nullInformationAuthorityItem = new EnumValueInfo(null, "");

		/// <summary>
		/// Constructor
		/// </summary>
		public ExternalPractitionerContactPointDetailsEditorComponent(
			ExternalPractitionerContactPointDetail contactPointDetail,
			IList<EnumValueInfo> resultCommunicationModeChoices,
			IList<EnumValueInfo> informationAuthorityChoices)
		{
			_contactPointDetail = contactPointDetail;
			_resultCommunicationModeChoices = resultCommunicationModeChoices;
			_informationAuthorityChoices = new List<EnumValueInfo>(informationAuthorityChoices);
			_informationAuthorityChoices.Insert(0, _nullInformationAuthorityItem);
		}

		#region Presentation Model

		[ValidateNotNull]
		public string ContactPointName
		{
			get { return _contactPointDetail.Name; }
			set
			{
				_contactPointDetail.Name = value;
				this.Modified = true;
			}
		}

		public string ContactPointDescription
		{
			get { return _contactPointDetail.Description; }
			set
			{
				_contactPointDetail.Description = value;
				this.Modified = true;
			}
		}

		public bool IsDefaultContactPoint
		{
			get { return _contactPointDetail.IsDefaultContactPoint; }
			set
			{
				if (_contactPointDetail.IsDefaultContactPoint == value)
					return;

				if (value && _contactPointDetail.Deactivated && UserLeavesContactPointDeactivated())
				{
					NotifyPropertyChanged("IsDefaultContactPoint");
					return;
				}

				_contactPointDetail.IsDefaultContactPoint = value;
				this.Modified = true;
			}
		}

		public bool HasWarning
		{
			get { return _contactPointDetail.IsMerged; }
		}

		public string WarningMessage
		{
			get
			{
				var destination = _contactPointDetail.MergeDestination.Name;
				return string.Format(SR.WarnEditMergedContactPoint, destination);
			}
		}

		public IList ResultCommunicationModeChoices
		{
			get { return (IList)_resultCommunicationModeChoices; }
		}

		public IList InformationAuthorityChoices
		{
			get { return (IList)_informationAuthorityChoices; }
		}

		[ValidateNotNull]
		public EnumValueInfo SelectedResultCommunicationMode
		{
			get { return _contactPointDetail.PreferredResultCommunicationMode; }
			set
			{
				_contactPointDetail.PreferredResultCommunicationMode = value;
				this.Modified = true;
			}
		}

		public EnumValueInfo SelectedInformationAuthority
		{
			get { return _contactPointDetail.InformationAuthority ?? _nullInformationAuthorityItem; }
			set
			{
				_contactPointDetail.InformationAuthority = value == _nullInformationAuthorityItem ? null : value;
				this.Modified = true;
			}
		}

		#endregion

		private bool UserLeavesContactPointDeactivated()
		{
			var activateResponse = this.Host.ShowMessageBox(SR.MessageDefaultContactPointMustBeActive, MessageBoxActions.YesNo);
			if (activateResponse != DialogBoxAction.Yes)
			{
				return true;
			}

			_contactPointDetail.Deactivated = false;
			return false;
		}
	}
}
