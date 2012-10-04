#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Reflection;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using Path=System.IO.Path;

namespace MyPlugin.TextEditor
{
	public class ValidateFilenameAttribute : ValidationAttribute
	{
		public ValidateFilenameAttribute() {}

		protected override IValidationRule CreateRule(PropertyInfo property, PropertyGetter getter, string customMessage)
		{
			base.CheckPropertyIsType(property, typeof (string));

			string message = customMessage ?? "Value is not a valid filename.";

			return new ValidationRule(property.Name,
			                          delegate(IApplicationComponent component)
			                          	{
			                          		string value = ((string) getter(component)) ?? string.Empty;

			                          		if (string.IsNullOrEmpty(value)
			                          		    || value.IndexOfAny(Path.GetInvalidPathChars()) >= 0
			                          		    || value.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			                          		{
			                          			return new ValidationResult(false, message);
			                          		}
			                          		else
			                          		{
			                          			return new ValidationResult(true, string.Empty);
			                          		}
			                          	});
		}
	}
}