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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core
{
    public abstract class EntitySearchCriteria<TEntity> : SearchCriteria
        where TEntity : Entity
    {
        public EntitySearchCriteria(string key)
            :base(key)
        {
        }

        public EntitySearchCriteria()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected EntitySearchCriteria(EntitySearchCriteria<TEntity> other)
            :base(other)
        {
        }

        public void EqualTo(TEntity entity)
        {
            this.OID.EqualTo(entity.OID);
        }

        public void NotEqualTo(TEntity entity)
        {
            this.OID.NotEqualTo(entity.OID);
        }

        public void In(IEnumerable<TEntity> entities)
        {
            this.OID.In(
				CollectionUtils.Map<TEntity, object>(entities,
					delegate (TEntity item) { return item.OID; }));
        }

        public void IsNull()
        {
            this.OID.IsNull();
        }

        public void IsNotNull()
        {
            this.OID.IsNotNull();
        }

		/// <summary>
		/// Specifies that the condition variable be less than the specified value.
		/// </summary>
		public void LessThan(TEntity entity)
		{
			this.OID.LessThan(entity.OID);
		}

		/// <summary>
		/// Specifies that the condition variable be less than or equal to the specified value.
		/// </summary>
		public void LessThanOrEqualTo(TEntity entity)
		{
			this.OID.LessThanOrEqualTo(entity.OID);
		}

    	/// <summary>
		/// Specifies that the condition variable be more than the specified value.
		/// </summary>
		public void MoreThan(TEntity entity)
    	{
			this.OID.MoreThan(entity.OID);
		}

    	/// <summary>
		/// Specifies that the condition variable be more than or equal to the specified value.
		/// </summary>
		public void MoreThanOrEqualTo(TEntity entity)
    	{
			this.OID.MoreThanOrEqualTo(entity.OID);
		}

		/// <summary>
		/// Specifies that the condition variable be used to sort the results in ascending order.
		/// </summary>
		/// <param name="position">Specifies the priority of this sort constraint relative to other sort constraints</param>
		public void SortAsc(int position)
		{
	    	this.OID.SortAsc(position);
		}

		/// <summary>
		/// Specifies that the condition variable be used to sort the results in descending order.
		/// </summary>
		/// <param name="position">Specifies the priority of this sort constraint relative to other sort constraints</param>
		public void SortDesc(int position)
		{
			this.OID.SortDesc(position);
		}

    	public ISearchCondition<object> OID
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("OID"))
                {
					this.SubCriteria["OID"] = new SearchCondition<object>("OID");
                }
				return (ISearchCondition<object>)this.SubCriteria["OID"];
            }
        }
   }
}
