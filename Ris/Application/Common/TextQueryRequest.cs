#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
	public class TextQueryRequest : ListRequestBase
    {
		/// <summary>
		/// Constructor
		/// </summary>
		public TextQueryRequest()
		{
				
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="textQuery"></param>
		/// <param name="specificityThreshold"></param>
		public TextQueryRequest(string textQuery, int specificityThreshold)
			:this(textQuery, specificityThreshold, false)
		{
		}

    	/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="textQuery"></param>
		/// <param name="specificityThreshold"></param>
    	/// <param name="includeDeactivated"></param>
    	public TextQueryRequest(string textQuery, int specificityThreshold, bool includeDeactivated)
    	{
    		TextQuery = textQuery;
    		SpecificityThreshold = specificityThreshold;
			IncludeDeactivated = includeDeactivated;
    	}

    	/// <summary>
        /// The query text.
        /// </summary>
        [DataMember]
        public string TextQuery;

        /// <summary>
        /// The maximum number of allowed matches for which results should be returned.  If the query results in more
        /// matches, it is considered to be not specific enough, and no results are returned. If this value is 0,
        /// it is ignored.
        /// </summary>
        [DataMember]
        public int SpecificityThreshold;
    }
}
