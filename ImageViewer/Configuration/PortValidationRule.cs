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

			int port = dicomComponent.Port;

			if (port < 0 || port > 65535)
				return new ValidationResult(false, SR.ValidationPortOutOfRange);

			return new ValidationResult(true, "");
		}

		#endregion
	}
}
