#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Healthcare.Printing
{
	public class DowntimeFormPageModel : PageModel
	{
		private readonly string _accessionNumber;

		public DowntimeFormPageModel(string accessionNumber)
			:base(new PrintTemplateSettings().DowntimeFormTemplateUrl)
		{
			_accessionNumber = accessionNumber;
		}

		public override Dictionary<string, object> Variables
		{
			get { return new Dictionary<string, object> {{"AccessionNumber", _accessionNumber}}; }
		}
	}
}
