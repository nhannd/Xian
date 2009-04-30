using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
	public class WorklistItemSearchArgs
	{
		private readonly bool _includeDegeneratePatientItems;
		private readonly bool _includeDegenerateProcedureItems;
		private readonly int _threshold;
		private readonly WorklistItemSearchCriteria[] _searchCriteria;


		public WorklistItemSearchArgs(
			WorklistItemSearchCriteria[] searchCriteria,
			bool includeDegeneratePatientItems,
			bool includeDegenerateProcedureItems,
			int threshold)
		{
			_includeDegeneratePatientItems = includeDegeneratePatientItems;
			_includeDegenerateProcedureItems = includeDegenerateProcedureItems;
			_threshold = threshold;
			_searchCriteria = searchCriteria;
		}

		public WorklistItemSearchArgs(
			WorklistItemSearchCriteria[] searchCriteria,
			bool includeDegeneratePatientItems,
			bool includeDegenerateProcedureItems)
			:this(searchCriteria, includeDegeneratePatientItems, includeDegenerateProcedureItems, 0)
		{
		}

		/// <summary>
		/// Gets a value indicating whether to include results for patients that meet the criteria but do not have any procedures.
		/// </summary>
		public bool IncludeDegeneratePatientItems
		{
			get { return _includeDegeneratePatientItems; }
		}

		/// <summary>
		/// Gets a value indicating whether to include results for procedures that meet the criteria but do not have an active procedure step.
		/// </summary>
		public bool IncludeDegenerateProcedureItems
		{
			get { return _includeDegenerateProcedureItems; }
		}

		/// <summary>
		/// Gets the maximum number of items that the search may return.
		/// </summary>
		public int Threshold
		{
			get { return _threshold; }
		}

		/// <summary>
		/// Gets the search criteria.
		/// </summary>
		public WorklistItemSearchCriteria[]  SearchCriteria
		{
			get { return _searchCriteria; }
		}
	}
}
