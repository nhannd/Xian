#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Search criteria for <see cref="PersistentStoreVersion"/> class.
    /// </summary>
    public class PersistentStoreVersionSearchCriteria : EntitySearchCriteria<PersistentStoreVersion>
    {
        /// <summary>
        /// Constructor for top-level search criteria (no key required)
        /// </summary>
        public PersistentStoreVersionSearchCriteria()
        {
        }
	
        /// <summary>
        /// Constructor for sub-criteria (key required)
        /// </summary>
        public PersistentStoreVersionSearchCriteria(string key)
            :base(key)
        {
        }
		
        /// <summary>
        /// Copy constructor
        /// </summary>
        protected PersistentStoreVersionSearchCriteria(PersistentStoreVersionSearchCriteria other)
            :base(other)
        {
        }
		
        public override object Clone()
        {
            return new PersistentStoreVersionSearchCriteria(this);
        }
		
        public ISearchCondition<string> Major
        {
            get
            {
                if(!SubCriteria.ContainsKey("Major"))
                {
                    SubCriteria["Major"] = new SearchCondition<string>("Major");
                }
                return (ISearchCondition<string>)SubCriteria["Major"];
            }
        }
	  	
        public ISearchCondition<string> Minor
        {
            get
            {
                if(!SubCriteria.ContainsKey("Minor"))
                {
                    SubCriteria["Minor"] = new SearchCondition<string>("Minor");
                }
                return (ISearchCondition<string>)SubCriteria["Minor"];
            }
        }
	  	
        public ISearchCondition<string> Build
        {
            get
            {
                if(!SubCriteria.ContainsKey("Build"))
                {
                    SubCriteria["Build"] = new SearchCondition<string>("Build");
                }
                return (ISearchCondition<string>)SubCriteria["Build"];
            }
        }
	  	
        public ISearchCondition<string> Revision
        {
            get
            {
                if(!SubCriteria.ContainsKey("Revision"))
                {
                    SubCriteria["Revision"] = new SearchCondition<string>("Revision");
                }
                return (ISearchCondition<string>)SubCriteria["Revision"];
            }
        }	  	
    }
}