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
    public class WorklistSearchCriteria : EntitySearchCriteria<Worklist>
    {
        /// <summary>
        /// Constructor for top-level search criteria (no key required)
        /// </summary>
        public WorklistSearchCriteria()
        {
        }

        /// <summary>
        /// Constructor for sub-criteria (key required)
        /// </summary>
        public WorklistSearchCriteria(string key)
            : base(key)
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected WorklistSearchCriteria(WorklistSearchCriteria other)
            : base(other)
        {
        }

        public override object Clone()
        {
            return new WorklistSearchCriteria(this);
        }

        public ISearchCondition<string> Name
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("Name"))
                {
                    this.SubCriteria["Name"] = new SearchCondition<string>("Name");
                }
                return (ISearchCondition<string>)this.SubCriteria["Name"];
            }
        }

        public ISearchCondition<string> FullClassName
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("FullClassName"))
                {
                    this.SubCriteria["FullClassName"] = new SearchCondition<string>("FullClassName");
                }
                return (ISearchCondition<string>)this.SubCriteria["FullClassName"];
            }
        }
    }
}
