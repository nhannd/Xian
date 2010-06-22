#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Abstract base class for entity search criteria classes.
	/// </summary>
	public abstract class EntitySearchCriteria : SearchCriteria
	{
		/// <summary>
		/// Constructor
		/// </summary>
		protected EntitySearchCriteria()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		protected EntitySearchCriteria(string key)
			: base(key)
		{
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="other"></param>
		protected EntitySearchCriteria(EntitySearchCriteria other)
			: base(other)
		{
		}

		/// <summary>
		/// Gets the search condition on OID.
		/// </summary>
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

	/// <summary>
	/// Abstract base class for entity search criteria classes.
	/// </summary>
	public abstract class EntitySearchCriteria<TEntity> : EntitySearchCriteria
		where TEntity : Entity
	{
		/// <summary>
		/// Constructor
		/// </summary>
		protected EntitySearchCriteria(string key)
			: base(key)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		protected EntitySearchCriteria()
		{
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="other"></param>
		protected EntitySearchCriteria(EntitySearchCriteria<TEntity> other)
			: base(other)
		{
		}

		/// <summary>
		/// Specifies that the entity is equal to the specified entity.
		/// </summary>
		/// <param name="entity"></param>
		public void EqualTo(TEntity entity)
		{
			this.OID.EqualTo(entity.OID);
		}

		/// <summary>
		/// Specifies that the entity is not equal to the specified entity.
		/// </summary>
		/// <param name="entity"></param>
		public void NotEqualTo(TEntity entity)
		{
			this.OID.NotEqualTo(entity.OID);
		}

		/// <summary>
		/// Specifies that the entity is in the specified set of entities.
		/// </summary>
		/// <param name="entities"></param>
		public void In(IEnumerable<TEntity> entities)
		{
			this.OID.In(CollectionUtils.Map(entities, (TEntity item) => item.OID));
		}

		/// <summary>
		/// Specifies that the entity reference is null.
		/// </summary>
		public void IsNull()
		{
			this.OID.IsNull();
		}

		/// <summary>
		/// Specifies that the entity reference is non null.
		/// </summary>
		public void IsNotNull()
		{
			this.OID.IsNotNull();
		}

		/// <summary>
		/// Specifies that the entity be less than the specified entity (by OID).
		/// </summary>
		public void LessThan(TEntity entity)
		{
			this.OID.LessThan(entity.OID);
		}

		/// <summary>
		/// Specifies that the entity be less than or equal to the specified entity (by OID).
		/// </summary>
		public void LessThanOrEqualTo(TEntity entity)
		{
			this.OID.LessThanOrEqualTo(entity.OID);
		}

		/// <summary>
		/// Specifies that the entity be more than the specified entity (by OID).
		/// </summary>
		public void MoreThan(TEntity entity)
		{
			this.OID.MoreThan(entity.OID);
		}

		/// <summary>
		/// Specifies that the entity be more than or equal to the specified entity (by OID).
		/// </summary>
		public void MoreThanOrEqualTo(TEntity entity)
		{
			this.OID.MoreThanOrEqualTo(entity.OID);
		}

		/// <summary>
		/// Specifies that the entity be used to sort the results in ascending order (by OID).
		/// </summary>
		/// <param name="position">Specifies the priority of this sort constraint relative to other sort constraints</param>
		public void SortAsc(int position)
		{
			this.OID.SortAsc(position);
		}

		/// <summary>
		/// Specifies that the entity be used to sort the results in descending order (by OID).
		/// </summary>
		/// <param name="position">Specifies the priority of this sort constraint relative to other sort constraints</param>
		public void SortDesc(int position)
		{
			this.OID.SortDesc(position);
		}

	}
}
