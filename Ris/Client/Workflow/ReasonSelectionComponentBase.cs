#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using System.Collections;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="ReasonSelectionComponentBase"/>
	/// </summary>
	[ExtensionPoint]
	public class ReasonSelectionComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ProtocolReasonComponent class
	/// </summary>
	[AssociateView(typeof(ReasonSelectionComponentViewExtensionPoint))]
	public abstract class ReasonSelectionComponentBase : ApplicationComponent
	{
		private EnumValueInfo _selectedReason;
		private List<EnumValueInfo> _availableReasons;
		private string _otherReason;
		private ICannedTextLookupHandler _cannedTextLookupHandler;

		public override void Start()
		{
			_cannedTextLookupHandler = new CannedTextLookupHandler(this.Host.DesktopWindow);
			_availableReasons = GetReasonChoices();

			base.Start();
		}

		#region PresentationModel

		protected abstract List<EnumValueInfo> GetReasonChoices();

		public ICannedTextLookupHandler CannedTextLookupHandler
		{
			get { return _cannedTextLookupHandler; }
		}

		public EnumValueInfo Reason
		{
			get { return _selectedReason; }
		}

		public EnumValueInfo SelectedReasonChoice
		{
			get { return _selectedReason; }
			set
			{
				_selectedReason = value;
			}
		}

		public IList ReasonChoices
		{
			get { return _availableReasons; }
		}

		public string OtherReason
		{
			get { return _otherReason; }
			set { _otherReason = value; }
		}

		public bool AcceptEnabled
		{
			get { return _selectedReason != null; }
		}

		public void Accept()
		{
			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion
	}
}
