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
