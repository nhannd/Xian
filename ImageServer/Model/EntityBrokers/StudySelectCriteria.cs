#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    public partial class StudySelectCriteria
    {
        /// <summary>
        /// Used for EXISTS or NOT EXISTS subselects against the Series table.
        /// </summary>
        /// <remarks>
        /// A <see cref="SeriesSelectCriteria"/> instance is created with the subselect parameters, 
        /// and assigned to this Sub-Criteria.  Note that the link between the <see cref="Study"/>
        /// and <see cref="Series"/> tables is automatically added into the <see cref="SeriesSelectCriteria"/>
        /// instance by the broker.
        /// </remarks>
        public IRelatedEntityCondition<EntitySelectCriteria> SeriesRelatedEntityCondition
        {
            get
            {
                if (!SubCriteria.ContainsKey("SeriesRelatedEntityCondition"))
                {
                    SubCriteria["SeriesRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("SeriesRelatedEntityCondition", "Key", "StudyKey");
                }
                return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["SeriesRelatedEntityCondition"];
            }
        }

		/// <summary>
		/// Used for EXISTS or NOT EXISTS subselects against the StudyStorage table.
		/// </summary>
		/// <remarks>
		/// A <see cref="StudyStorageSelectCriteria"/> instance is created with the subselect parameters, 
		/// and assigned to this Sub-Criteria.  Note that the link between the <see cref="Study"/>
		/// and <see cref="StudyStorage"/> tables is automatically added into the <see cref="StudyStorageSelectCriteria"/>
		/// instance by the broker.
		/// </remarks>
		public IRelatedEntityCondition<EntitySelectCriteria> StudyStorageRelatedEntityCondition
		{
			get
			{
				if (!SubCriteria.ContainsKey("StudyStorageRelatedEntityCondition"))
				{
					SubCriteria["StudyStorageRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("StudyStorageRelatedEntityCondition", "StudyStorageKey", "Key");
				}
				return (IRelatedEntityCondition<EntitySelectCriteria>)SubCriteria["StudyStorageRelatedEntityCondition"];
			}
		}
    }
}
