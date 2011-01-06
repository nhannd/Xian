#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    public class ReportingWorklistItemSearchCriteria : WorklistItemSearchCriteria
    {
        public ReportPartSearchCriteria ReportPart
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("ReportPart"))
                {
                    this.SubCriteria["ReportPart"] = new ReportPartSearchCriteria("ReportPart");
                }
                return (ReportPartSearchCriteria)this.SubCriteria["ReportPart"];
            }
        }

		public ReportSearchCriteria Report
		{
			get
			{
				if (!this.SubCriteria.ContainsKey("Report"))
				{
					this.SubCriteria["Report"] = new ReportSearchCriteria("Report");
				}
				return (ReportSearchCriteria)this.SubCriteria["Report"];
			}
		}

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
