using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Configuration
{
	internal class AETitleValidatonRule : IValidationRule
	{
		public AETitleValidatonRule()
		{

		}

		#region IValidationRule Members

		public string PropertyName
		{
			get { return "AETitle"; }
		}

		public ValidationResult GetResult(IApplicationComponent component)
		{
			DicomServerConfigurationComponent dicomComponent = component as DicomServerConfigurationComponent;

			string aeTitle = dicomComponent.AETitle;

			if (String.IsNullOrEmpty(aeTitle))
				return new ValidationResult(false, SR.ValidationAETitleMustBeSpecified);

			if (aeTitle.Length < 1 || aeTitle.Length > 16)
				return new ValidationResult(false, SR.ValidationAETitleLengthIncorrect);

			return new ValidationResult(true, "");
		}

		#endregion
	}
}
