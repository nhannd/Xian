using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common
{
    [DataContract]
    public class EntityChangeSet
    {
        [DataMember]
        private EntityChange[] _changes;

        public EntityChangeSet(EntityChange[] changes)
        {
            _changes = changes;
        }
    }
}
