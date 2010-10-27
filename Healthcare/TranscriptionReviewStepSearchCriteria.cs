#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
	public class TranscriptionReviewStepSearchCriteria : ReportingProcedureStepSearchCriteria
	{
		public ISearchCondition<bool> HasErrors
		{
			get
			{
				if (!this.SubCriteria.ContainsKey("HasErrors"))
				{
					this.SubCriteria["HasErrors"] = new SearchCondition<bool>("HasErrors");
				}
				return (ISearchCondition<bool>)this.SubCriteria["HasErrors"];
			}
		}
	}
}
