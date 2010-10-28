#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="EmailAddressEditorComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class EmailAddressEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// EmailAddressEditorComponent class
	/// </summary>
	[AssociateView(typeof(EmailAddressEditorComponentViewExtensionPoint))]
	public class EmailAddressEditorComponent : ApplicationComponent
	{
		private readonly EmailAddressDetail _emailAddress;

		/// <summary>
		/// Constructor
		/// </summary>
		public EmailAddressEditorComponent(EmailAddressDetail emailAddress)
		{
			_emailAddress = emailAddress;
		}

		public override void Start()
		{
			this.Validation.Add(new ValidationRule("ValidUntil",
				delegate
				{
					// only need to validate the if both ValidFrom and ValidUntil are specified
					if (!_emailAddress.ValidRangeFrom.HasValue || !_emailAddress.ValidRangeUntil.HasValue)
						return new ValidationResult(true, "");

					var ok = DateTime.Compare(_emailAddress.ValidRangeUntil.Value, _emailAddress.ValidRangeFrom.Value) >= 0;
					return new ValidationResult(ok, SR.MessageValidUntilMustBeLaterOrEqualValidFrom);
				}));
			
			base.Start();
		}

		#region Presentation Model

		[ValidateNotNull]
		[ValidateRegex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", AllowNull = true)]
		public string Address
		{
			get { return _emailAddress.Address; }
			set
			{
				_emailAddress.Address = value;
				this.Modified = true;
			}
		}


		public DateTime? ValidFrom
		{
			get { return _emailAddress.ValidRangeFrom; }
			set
			{
				_emailAddress.ValidRangeFrom = value == null ? value : value.Value.Date;
				this.Modified = true;
			}
		}

		public DateTime? ValidUntil
		{
			get { return _emailAddress.ValidRangeUntil; }
			set
			{
				_emailAddress.ValidRangeUntil = value == null ? value : value.Value.Date;
				this.Modified = true;
			}
		}

		public void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
			}
			else
			{
				this.ExitCode = ApplicationComponentExitCode.Accepted;
				Host.Exit();
			}
		}

		public void Cancel()
		{
			this.ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}

		public bool AcceptEnabled
		{
			get { return this.Modified; }
		}

		public event EventHandler AcceptEnabledChanged
		{
			add { this.ModifiedChanged += value; }
			remove { this.ModifiedChanged -= value; }
		}

		#endregion
	}
}
