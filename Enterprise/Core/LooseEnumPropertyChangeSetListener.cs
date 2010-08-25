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

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// This listener synchronizes values in a string property of an entity with a specified enumeration.  It is basically a trigger 
	/// to implement a loose coupling between the entity's property and the enumeration with the result that the enumeration then contains
	/// the distinct set of values for that property.  The enumeration can then be queried for the distinct set of values at a much lower
	/// cost than querying the entities themselves.
	/// </summary>
	/// <typeparam name="TChangedEntityClass"></typeparam>
	/// <typeparam name="TLooseEnumClass"></typeparam>
	public abstract class LooseEnumPropertyChangeSetListener<TChangedEntityClass, TLooseEnumClass> : IEntityChangeSetListener
		where TChangedEntityClass : Entity
		where TLooseEnumClass : EnumValue
	{
		/// <summary>
		/// Returns a string value from the specified entity which should be synchronized with this listener's enumeration class.
		/// </summary>
		/// <param name="changedEntity"></param>
		/// <returns></returns>
		public abstract string GetEnumCodeFromEntity(TChangedEntityClass changedEntity);

		#region Implementation of IEntityChangeSetListener

		public void PreCommit(EntityChangeSetPreCommitArgs args)
		{
			foreach (var entityChange in args.ChangeSet.Changes)
			{
				if (entityChange.GetEntityClass() != typeof(TChangedEntityClass))
					continue;

				var workQueueItem = args.PersistenceContext.Load<TChangedEntityClass>(entityChange.EntityRef);
				var queueItemTypeBroker = args.PersistenceContext.GetBroker<IEnumBroker>();

				var code = GetEnumCodeFromEntity(workQueueItem);
				try
				{
					var foo = queueItemTypeBroker.TryFind(typeof(TLooseEnumClass), code);
				}
				catch (EnumValueNotFoundException)
				{
					var displayOrder = queueItemTypeBroker.Load(typeof(TLooseEnumClass), true).Count;

					queueItemTypeBroker.AddValue(
						typeof(TLooseEnumClass),
						code,
						code,
						string.Empty,
						displayOrder,
						false);
				}
			}
		}

		public void PostCommit(EntityChangeSetPostCommitArgs args)
		{
		}

		#endregion
	}
}
