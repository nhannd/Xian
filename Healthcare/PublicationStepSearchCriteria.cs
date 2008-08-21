using System;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Search criteria for <see cref="PublicationStep"/> class.
	/// </summary>
	public class PublicationStepSearchCriteria : ReportingProcedureStepSearchCriteria
	{
		/// <summary>
		/// Constructor for top-level search criteria (no key required)
		/// </summary>
		public PublicationStepSearchCriteria()
		{
		}
	
		/// <summary>
		/// Constructor for sub-criteria (key required)
		/// </summary>
		public PublicationStepSearchCriteria(string key)
			:base(key)
		{
		}

		public ISearchCondition<int> FailureCount
		{
			get
			{
				if (!this.SubCriteria.ContainsKey("FailureCount"))
				{
					this.SubCriteria["FailureCount"] = new SearchCondition<int>("FailureCount");
				}
				return (ISearchCondition<int>)this.SubCriteria["FailureCount"];
			}
		}

		public ISearchCondition<DateTime?> LastFailureTime
		{
			get
			{
				if (!this.SubCriteria.ContainsKey("LastFailureTime"))
				{
					this.SubCriteria["LastFailureTime"] = new SearchCondition<DateTime?>("LastFailureTime");
				}
				return (ISearchCondition<DateTime?>)this.SubCriteria["LastFailureTime"];
			}
		}
	}
}
