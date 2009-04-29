using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class WorklistItemTextQueryRequest : TextQueryRequest
	{
        [DataContract]
        public class AdvancedSearchFields : DataContractBase
        {
            [DataMember]
            public string FamilyName;

            [DataMember]
            public string GivenName;

            [DataMember]
            public string Mrn;

            [DataMember]
            public string HealthcardNumber;

            [DataMember]
            public string AccessionNumber;

            [DataMember]
            public EntityRef ProcedureTypeRef;

            [DataMember]
            public EntityRef OrderingPractitionerRef;

            [DataMember]
            public DateTime? FromDate;

            [DataMember]
            public DateTime? UntilDate;
        }

        /// <summary>
		/// Constructor
		/// </summary>
		public WorklistItemTextQueryRequest()
		{

		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="textQuery"></param>
		/// <param name="specificityThreshold"></param>
		/// <param name="procedureStepClassName"></param>
		/// <param name="options"></param>
		public WorklistItemTextQueryRequest(string textQuery, int specificityThreshold, string procedureStepClassName, WorklistItemTextQueryOptions options)
			: base(textQuery, specificityThreshold)
		{
			ProcedureStepClassName = procedureStepClassName;
			Options = options;
		}

		/// <summary>
		/// Name of the procedure step class of interest.
		/// </summary>
		[DataMember]
		public string ProcedureStepClassName;

		/// <summary>
		/// Specifies options that affect how the search is executed.
		/// </summary>
		[DataMember]
		public WorklistItemTextQueryOptions Options;

        /// <summary>
        /// Specifies that "advanced" mode should be used, in which case the text query is ignored
        /// and the search is based on the content of the <see cref="SearchFields"/> member.
        /// </summary>
        [DataMember]
        public bool UseAdvancedSearch;

        /// <summary>
        /// Data used in the advanced search mode.
        /// </summary>
        [DataMember]
        public AdvancedSearchFields SearchFields;
	}
}
