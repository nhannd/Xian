using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Configuration
{
	internal class PortValidationRule : IValidationRule
	{
		public PortValidationRule()
		{

		}

		#region IValidationRule Members

		public string PropertyName
		{
			get { return "Port"; }
		}

		public ValidationResult GetResult(IApplicationComponent component)
		{
			DicomServerConfigurationComponent dicomComponent = component as DicomServerConfigurationComponent;

			if (String.IsNullOrEmpty(dicomComponent.Port))
				return new ValidationResult(false, SR.ValidationPortMustBeSpecified);

			int port;

			try
			{
				port = Convert.ToInt32(dicomComponent.Port);
			}
			catch (FormatException e)
			{
				return new ValidationResult(false, SR.ValidationPortMustBeNumeric);
			}
			catch (OverflowException e)
			{
				return new ValidationResult(false, SR.ValidationPortOutOfRange);
			}

			if (port < 1 || port > 65535)
				return new ValidationResult(false, SR.ValidationPortOutOfRange);

			return new ValidationResult(true, "");
		}

		#endregion
	}
}
