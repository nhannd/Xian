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
	/// Extension point for views onto <see cref="UnmergeOrderComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class UnmergeOrderComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// UnmergeOrderComponent class
	/// </summary>
	[AssociateView(typeof(UnmergeOrderComponentViewExtensionPoint))]
	public class UnmergeOrderComponent : ApplicationComponent
	{
		private EnumValueInfo _selectedReason;
		private List<EnumValueInfo> _cancelReasonChoices;

		/// <summary>
		/// Constructor
		/// </summary>
		public UnmergeOrderComponent()
		{
		}

		public override void Start()
		{
			Platform.GetService<IOrderEntryService>(
				service =>
				{
					_cancelReasonChoices = service.GetCancelOrderFormData(new GetCancelOrderFormDataRequest()).CancelReasonChoices;
				});

			base.Start();
		}

		#region Presentation Model

		public IList ReasonChoices
		{
			get { return _cancelReasonChoices; }
		}

		public EnumValueInfo SelectedReason
		{
			get { return _selectedReason; }
			set { _selectedReason = value; }
		}

		#endregion

		public void Accept()
		{
			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Unmerge()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		public bool AcceptEnabled
		{
			get { return _selectedReason != null; }
		}
	}
}
