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
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="CancelOrderComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class CancelOrderComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// CancelOrderComponent class
	/// </summary>
	[AssociateView(typeof(CancelOrderComponentViewExtensionPoint))]
	public class CancelOrderComponent : ApplicationComponent
	{
		private EnumValueInfo _selectedCancelReason;
		private List<EnumValueInfo> _cancelReasonChoices;

		/// <summary>
		/// Constructor
		/// </summary>
		public CancelOrderComponent()
		{
		}

		public override void Start()
		{
			Platform.GetService<IOrderEntryService>(
				service =>
				{
					var response = service.GetCancelOrderFormData(new GetCancelOrderFormDataRequest());
					_cancelReasonChoices = response.CancelReasonChoices;
				});

			base.Start();
		}

		#region Presentation Model

		public IList CancelReasonChoices
		{
			get { return _cancelReasonChoices; }
		}

		public EnumValueInfo SelectedCancelReason
		{
			get { return _selectedCancelReason; }
			set { _selectedCancelReason = value; }
		}

		#endregion

		public void Accept()
		{
			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		public bool AcceptEnabled
		{
			get { return _selectedCancelReason != null; }
		}
	}
}
