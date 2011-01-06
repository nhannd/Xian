#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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

		/// <summary>
		/// Constructor
		/// </summary>
		public ExternalPractitionerContactPointDetailsEditorComponent(ExternalPractitionerContactPointDetail contactPointDetail, IList<EnumValueInfo> resultCommunicationModeChoices)
		{
			_contactPointDetail = contactPointDetail;
			_resultCommunicationModeChoices = resultCommunicationModeChoices;
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

		public IList ResultCommunicationModeChoices
		{
			get { return (IList)_resultCommunicationModeChoices; }
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

		#endregion

		private bool UserLeavesContactPointDeactivated()
		{
			var activateResponse =
				this.Host.ShowMessageBox(
					"This contact point is not currently active and must be active in order to set it as the default.  Would you like to make it active?",
					MessageBoxActions.YesNo);

			if (activateResponse != DialogBoxAction.Yes)
			{
				return true;
			}

			_contactPointDetail.Deactivated = false;
			return false;
		}
	}
}
