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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using Iesi.Collections;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Helper class used by <see cref="UpdateContextInterceptor"/> to track changes to entities during usage
    /// of an update context.
    /// </summary>
    internal class ChangeTracker
    {

        // keeps a record of changes made to entities
        // a given entity instance will only appear once in this list
        // (a dictionary is not used because we cannot control the implemention of the entity's Equals/GetHashCode methods,
        // and we want to deal strictly with reference equality here)
        private readonly List<ChangeRecord> _changeRecords = new List<ChangeRecord>();

        /// <summary>
        /// Records that the specified change occured to the specified entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="changeType"></param>
        /// <param name="propertyDiffs"></param>
        public void RecordChange(Entity entity, EntityChangeType changeType, PropertyDiff[] propertyDiffs)
        {
            ChangeRecord thisChange = new ChangeRecord(entity, changeType, propertyDiffs);

            // check if this entity was already recorded in the list
            // note that reference equality is used because we do not know how the Equals method of the entity may be implemented
            // for example, it may define equality based on a property whose value has changed, which would break this logic
            int prevRecordIndex = _changeRecords.FindIndex(delegate(ChangeRecord r) { return ReferenceEquals(r.Entity, entity); });
            if (prevRecordIndex > -1)
            {
                // this entity was already marked as changed
                ChangeRecord previousChange = _changeRecords[prevRecordIndex];

                // compound the previous change record with this one
                _changeRecords[prevRecordIndex] = thisChange.Compound(previousChange);
            }
            else
            {
                // record this change in the change set
                _changeRecords.Add(thisChange);
            }
        }

        /// <summary>
        /// Gets the set of <see cref="EntityChange"/> objects representing the cumulative changes made.
        /// </summary>
        public EntityChange[] EntityChangeSet
        {
            get
            {
                return CollectionUtils.Map<ChangeRecord, EntityChange>(_changeRecords,
                    delegate(ChangeRecord changeRecord)
                    {
                        return changeRecord.AsEntityChange();
                    }).ToArray();
            }
        }
    }
}
